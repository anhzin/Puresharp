using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Puresharp.Composition
{
    /// <summary>
    /// Container
    /// </summary>
    public sealed partial class Container : IContainer
    {
        static private object m_Handle = new object();
        static private List<Action> m_Instantiation = new List<Action>();
        static private int m_Sequence = 0;

        private int m_Index;
        
        /// <summary>
        /// Instantiate a container.
        /// </summary>
        public Container()
        {
            lock (Container.m_Handle)
            {
                this.m_Index = Container.m_Sequence++;
                for (var _index = 0; _index < Container.m_Instantiation.Count; _index++) { Container.m_Instantiation[_index](); }
            }
        }

        /// <summary>
        /// Add instance as singleton.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="instance">Instance</param>
        public void Add<T>(T instance)
            where T : class
        {
            this.Add(() => instance);
        }

        /// <summary>
        /// Add enumerable of instances as multiton.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="enumerable">Enumerable</param>
        public void Add<T>(IEnumerable<T> enumerable)
            where T : class
        {
            if (enumerable == null) { return; }
            foreach (var _instance in enumerable) { this.Add<T>(new Func<T>(() => _instance)); }
        }

        private void Add<T>(Func<T> function)
            where T : class
        {
            var _container = Container.Lookup<T>.Buffer[this.m_Index];
            while (true)
            {
                var _instance = _container.Instance;
                if (_instance == Container.Store<T>.None)
                {
                    if (Interlocked.CompareExchange(ref _container.Instance, function, _instance) == _instance)
                    {
                        Linkup<Func<T>>.Update(ref _container.Linkup, function);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Container.Store<T>.Empty, _array) == _array) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref _container.Array, new Func<T[]>(() => { var _buffer = new T[_activation.Length]; for (var _index = 0; _index < _buffer.Length; _index++) { _buffer[_index] = _activation[_index](); } return _buffer; }), _array) == _array) { return; }
                            }
                        }
                    }
                }
                else
                {
                    if (Interlocked.CompareExchange(ref _container.Instance, Container.Store<T>.Multiple, _instance) == _instance)
                    {
                        Linkup<Func<T>>.Update(ref _container.Linkup, function);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Container.Store<T>.Empty, _array) == _array) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref _container.Array, new Func<T[]>(() => { var _buffer = new T[_activation.Length]; for (var _index = 0; _index < _buffer.Length; _index++) { _buffer[_index] = _activation[_index](); } return _buffer; }), _array) == _array) { return; }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add factory to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="function">Factory</param>
        /// <param name="lifetime">Lifetime</param>
        public void Add<T>(Func<T> function, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, null) || object.ReferenceEquals(lifetime, Lifetime.Volatile)) { this.Add<T>(function); }
            else if (object.ReferenceEquals(lifetime, Lifetime.Singleton))
            {
                var _container = Container.Lookup<T>.Buffer[this.m_Index];
                var _handle = new object();
                var _return = null as Func<T>;
                var _instance = null as Func<T>;
                this.Add<T>(_instance = new Func<T>(() =>
                {
                    lock (_handle)
                    {
                        if (object.Equals(_return, null))
                        {
                            var _singleton = function();
                            _return = new Func<T>(() => _singleton);
                            Interlocked.CompareExchange(ref _container.Instance, _return, _instance);
                            var _linkup = _container.Linkup;
                            if (_linkup != null) { _linkup.Update(function, _return); }
                            return _singleton;
                        }
                        return _return();
                    }
                }));
            }
            else if (lifetime is Lifetime.IStore)
            {
                throw new NotImplementedException();
            }
            else { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Add constructor to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="constructor">Constructor</param>
        /// <param name="lifetime">Lifetime</param>
        public void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile(), lifetime); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Container.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile(), lifetime);
            }
        }

        /// <summary>
        /// Add a static method to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="method">Method</param>
        /// <param name="lifetime">Lifetime</param>
        public void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class
        {
            if (method.IsStatic && Metadata<T>.Type.IsAssignableFrom(method.ReturnType))
            {
                var _signature = method.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Container.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                this.Add<T>(Expression.Lambda<Func<T>>(Expression.Call(method, _arguments)).Compile(), lifetime);
            }
            else { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Obtain a single instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <returns>Instance</returns>
        public T Instance<T>()
            where T : class
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Instance();
        }

        /// <summary>
        /// Obtain enumerable of all instances registered.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <returns>Enumerable</returns>
        public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Array();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
