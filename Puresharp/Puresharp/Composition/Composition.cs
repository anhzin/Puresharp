using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Puresharp
{
    public sealed class Composition : Composition<Neptune>
    {
        private Composition()
        {
        }
    }

    public partial class Composition<X>
        where X : class
    {
        static public void Add<T>(Func<T> instance)
            where T : class
        {
            while (true)
            {
                var _instance = Composition<X>.Lookup<T>.m_Instance;
                if (_instance == Composition<X>.Lookup<T>.None)
                {
                    if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Instance, instance, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Update(ref Composition<X>.Lookup<T>.Linkup, instance);
                        while (true)
                        {
                            var _enumerable = Composition<X>.Lookup<T>.m_Enumerable;
                            var _linkup = Composition<X>.Lookup<T>.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Enumerable, Composition<X>.Lookup<T>.Empty, _enumerable) == _enumerable) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Enumerable, new Func<IEnumerable<T>>(() => { var _array = new T[_activation.Length]; for (var _index = 0; _index < _array.Length; _index++) { _array[_index] = _activation[_index](); } return _array; }), _enumerable) == _enumerable) { return; }
                            }
                        }
                    }
                }
                else
                {
                    if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Instance, Composition<X>.Lookup<T>.Multiple, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Update(ref Composition<X>.Lookup<T>.Linkup, instance);
                        while (true)
                        {
                            var _enumerable = Composition<X>.Lookup<T>.m_Enumerable;
                            var _linkup = Composition<X>.Lookup<T>.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Enumerable, Composition<X>.Lookup<T>.Empty, _enumerable) == _enumerable) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Enumerable, new Func<IEnumerable<T>>(() => { var _array = new T[_activation.Length]; for (var _index = 0; _index < _array.Length; _index++) { _array[_index] = _activation[_index](); } return _array; }), _enumerable) == _enumerable) { return; }
                            }
                        }
                    }
                }
            }
        }

        static public void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, null) || object.ReferenceEquals(lifetime, Lifetime.Volatile)) { Composition<X>.Add<T>(instance); }
            else if (object.ReferenceEquals(lifetime, Lifetime.Singleton))
            {
                var _handle = new object();
                var _return = null as Func<T>;
                var _instance = null as Func<T>;
                Composition<X>.Add<T>(_instance = new Func<T>(() =>
                {
                    lock (_handle)
                    {
                        if (object.Equals(_return, null))
                        {
                            var _singleton = instance();
                            _return = new Func<T>(() => _singleton);
                            Composition<X>.Lookup<T>.Dictionary.Add(instance, _return);
                            Interlocked.CompareExchange(ref Composition<X>.Lookup<T>.m_Instance, _return, _instance);
                            var _linkup = Composition<X>.Lookup<T>.Linkup;
                            if (_linkup != null) { _linkup.Update(instance, _return); }
                            return _singleton;
                        }
                        return _return();
                    }
                }));
            }
            else if (lifetime is Lifetime.ICycle)
            {
                Composition<X>.Scope<T> _scope = null;
                (lifetime as Lifetime.ICycle).Establish<Composition<X>.Scope<T>>(() => _scope = new Composition<X>.Scope<T>(instance));
                Composition<X>.Add<T>(new Func<T>(() => _scope.Instance));
            }
            else
            {
                //TODO!
                throw new NotSupportedException();
            }
        }

        static public void Add<T>(string directory, SearchOption option)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly);
                }
            }
        }

        static public void Add<T>(string directory, SearchOption option, Func<Type, bool> predicate)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, predicate);
                }
            }
        }

        static public void Add<T>(string directory)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly);
                }
            }
        }

        static public void Add<T>(string directory, Func<Type, bool> predicate)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, predicate);
                }
            }
        }

        static public void Add<T>(IEnumerable<Assembly> assemblies)
            where T : class
        {
            if (assemblies == null) { return; }
            foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly); }
        }

        static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
            where T : class
        {
            if (assemblies == null) { return; }
            foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, predicate); }
        }

        static public void Add<T>(Assembly assembly)
            where T : class
        {
            var _types = assembly.GetTypes();
            for (var _index = 0; _index < _types.Length; _index++)
            {
                var _type = _types[_index];
                if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type)) { Composition<X>.Add<T>(_type); }
            }
        }

        static public void Add<T>(Assembly assembly, Func<Type, bool> predicate)
            where T : class
        {
            var _types = assembly.GetTypes();
            for (var _index = 0; _index < _types.Length; _index++)
            {
                var _type = _types[_index];
                if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type) && ( predicate == null || predicate(_type))) { Composition<X>.Add<T>(_type); }
            }
        }

        static public void Add<T>(IEnumerable<Type> types)
            where T : class
        {
            if (types == null) { return; }
            foreach (var _type in types) { Composition<X>.Add<T>(_type); }
        }

        static public void Add<T>(Type type)
            where T : class
        {
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.Constructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile());
            }
            else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile()); }
        }

        static public void Add<T>(ConstructorInfo constructor)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile()); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile());
            }
        }

        static public void Add<T>(T singleton)
            where T : class
        {
            Composition<X>.Add<T>(new Func<T>(() => singleton));
        }

        static public void Add<T>(params T[] multiton)
            where T : class
        {
            if (multiton == null || multiton.Length == 0) { return; }
            for (var _index = 0; _index < multiton.Length; _index++)
            {
                var _instance = multiton[_index];
                Composition<X>.Add<T>(new Func<T>(() => _instance));
            }
        }

        static public void Add<T>(IEnumerable<T> multiton)
            where T : class
        {
            if (multiton == null) { return; }
            foreach (var _instance in multiton) { Composition<X>.Add<T>(new Func<T>(() => _instance)); }
        }

        static public void Add<T>(string directory, SearchOption option, Func<Type, Lifetime> lifetime)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, lifetime);
                }
            }
        }

        static public void Add<T>(string directory, SearchOption option, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, predicate, lifetime);
                }
            }
        }

        static public void Add<T>(string directory, Func<Type, Lifetime> lifetime)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, lifetime);
                }
            }
        }

        static public void Add<T>(string directory, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
            where T : class
        {
            foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
            {
                var _extension = Path.GetExtension(_filename);
                if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Assembly _assembly = null;
                    try { _assembly = Assembly.LoadFrom(_filename); }
                    catch { }
                    if (_assembly == null) { continue; }
                    Composition<X>.Add<T>(_assembly, predicate, lifetime);
                }
            }
        }

        static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, Lifetime> lifetime)
            where T : class
        {
            if (assemblies == null) { return; }
            foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, lifetime); }
        }

        static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
            where T : class
        {
            if (assemblies == null) { return; }
            foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, predicate, lifetime); }
        }

        static public void Add<T>(Assembly assembly, Func<Type, Lifetime> lifetime)
            where T : class
        {
            var _types = assembly.GetTypes();
            for (var _index = 0; _index < _types.Length; _index++)
            {
                var _type = _types[_index];
                if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type)) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
            }
        }

        static public void Add<T>(Assembly assembly, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
            where T : class
        {
            var _types = assembly.GetTypes();
            for (var _index = 0; _index < _types.Length; _index++)
            {
                var _type = _types[_index];
                if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type) && (predicate == null || predicate(_type))) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
            }
        }

        static public void Add<T>(IEnumerable<Type> types, Func<Type, Lifetime> lifetime)
            where T : class
        {
            if (types == null) { return; }
            foreach (var _type in types) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
        }

        static public void Add<T>(Type type, Lifetime lifetime)
            where T : class
        {
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.Constructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile(), lifetime);
            }
            else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile(), lifetime); }
        }

        static public void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile(), lifetime); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile(), lifetime);
            }
        }

        static public void Clear<T>()
            where T : class
        {
            lock (Composition<X>.Lookup<T>.Handle)
            {
                Composition<X>.Lookup<T>.m_Instance = Composition<X>.Lookup<T>.None;
                Composition<X>.Lookup<T>.m_Enumerable = Composition<X>.Lookup<T>.Empty;
                Composition<X>.Lookup<T>.Dictionary.Clear();
            }
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T Instance<T>()
            where T : class
        {
            return Composition<X>.Lookup<T>.m_Instance();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Composition<X>.Lookup<T>.m_Enumerable();
        }

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// ReferenceEquals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        protected Composition()
        {
            throw new NotSupportedException();
        }
    }
}
