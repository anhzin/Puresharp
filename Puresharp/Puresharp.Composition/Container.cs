using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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

        /// <summary>
        /// Obtain index of container.
        /// </summary>
        /// <param name="container">Container</param>
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        static public implicit operator int(Container container)
        {
            return container.m_Index;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        private int m_Index;

        public Container()
        {
            lock (Container.m_Handle)
            {
                this.m_Index = Container.m_Sequence++;
                for (var _index = 0; _index < Container.m_Instantiation.Count; _index++) { Container.m_Instantiation[_index](); }
            }
        }

        public void Add<T>(T instance)
            where T : class
        {
            this.Add(() => instance);
        }

        public void Add<T>(params T[] array)
            where T : class
        {
            if (array == null || array.Length == 0) { return; }
            for (var _index = 0; _index < array.Length; _index++)
            {
                var _instance = array[_index];
                this.Add<T>(new Func<T>(() => _instance));
            }
        }

        public void Add<T>(IEnumerable<T> enumerable)
            where T : class
        {
            if (enumerable == null) { return; }
            foreach (var _instance in enumerable) { this.Add<T>(new Func<T>(() => _instance)); }
        }

        public void Add<T>(Func<T> instance)
            where T : class
        {
            var _container = Container.Lookup<T>.Buffer[this.m_Index];
            while (true)
            {
                var _instance = _container.Instance;
                if (_instance == Container.Container1<T>.None)
                {
                    if (Interlocked.CompareExchange(ref _container.Instance, instance, _instance) == _instance)
                    {
                        Linkup<Func<T>>.Update(ref _container.Linkup, instance);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Container.Container1<T>.Empty, _array) == _array) { return; } }
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
                    if (Interlocked.CompareExchange(ref _container.Instance, Container.Container1<T>.Multiple, _instance) == _instance)
                    {
                        Linkup<Func<T>>.Update(ref _container.Linkup, instance);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Container.Container1<T>.Empty, _array) == _array) { return; } }
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

        public void Add<T>(Type type)
            where T : class
        {
            var _container = Container.Lookup<T>.Buffer[this.m_Index];
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.GetConstructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Container.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile());
            }
            else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile()); }
        }

        public void Add<T>(ConstructorInfo constructor)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile()); }
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
                this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile());
            }
        }

        public void Add<T>(MethodInfo method)
            where T : class
        {
            var _signature = method.GetParameters();
            if (_signature.Length == 0) { throw new NotSupportedException(); }
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
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Metadata<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, Metadata<IDeserializationCallback>.Type), Metadata<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Argument<object>.Value)), Expression.Constant(null, Metadata<object>.Type)), Expression<T>.Parameter)).Compile()); }
                    else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile()); }
                }
                else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.New(_constructor), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile()); }
            }
        }

        public void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, null) || object.ReferenceEquals(lifetime, Lifetime.Volatile)) { this.Add<T>(instance); }
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
                            var _singleton = instance();
                            _return = new Func<T>(() => _singleton);
                            Interlocked.CompareExchange(ref _container.Instance, _return, _instance);
                            var _linkup = _container.Linkup;
                            if (_linkup != null) { _linkup.Update(instance, _return); }
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

        public void Add<T>(Type type, Lifetime lifetime)
            where T : class
        {
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.GetConstructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Container.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile(), lifetime);
            }
            else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile(), lifetime); }
        }

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

        public void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class
        {
            var _signature = method.GetParameters();
            if (_signature.Length == 0) { throw new NotSupportedException(); }
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
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Metadata<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, Metadata<IDeserializationCallback>.Type), Metadata<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Argument<object>.Value)), Expression.Constant(null, Metadata<object>.Type)), Expression<T>.Parameter)).Compile(), lifetime); }
                    else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile(), lifetime); }
                }
                else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.New(_constructor), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile(), lifetime); }
            }
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Instance<T>()
            where T : class
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Instance();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        T IContainer.Instance<T>()
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Instance();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Array();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T[] Array<T>()
            where T : class
        {
            return Container.Lookup<T>.Buffer[this.m_Index].Array();
        }

        public void Dispose()
        {
        }
    }

    static public partial class Container<T>
        where T : class
    {
        static public void Add<T>(T instance)
            where T : class
        {
            Add(() => instance);
        }

        static public void Add<T>(IEnumerable<T> enumerable)
            where T : class
        {
            if (enumerable == null) { return; }
            foreach (var _instance in enumerable) { Add<T>(new Func<T>(() => _instance)); }
        }
        
        static public void Add<T>(Func<T> instance)
            where T : class
        {
            lock (Lookup<T>.Handle)
            {
                var _type = Lookup<T>.Module.DefineType(Lookup<T>.Type.Length.ToString(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
                var _array = null as ILGenerator;
                var _enumerable = null as ILGenerator;
                var _field = null as FieldBuilder;
                var _fields = null as FieldBuilder[];
                var _constructor = null as ILGenerator;
                var _factory = null as Type;
                var _method = null as DynamicMethod;
                var _delegate = null as Func<object, Func<T>, Func<T[]>>;
                if (Lookup<T>.m_Instance == Lookup<T>.None)
                {
                    Lookup<T>.m_Instance = instance;
                    _fields = new FieldBuilder[1];
                    _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Metadata<Func<T>>.Type }).GetILGenerator();
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Ldarg_1);
                    _constructor.Emit(OpCodes.Stfld, _field = _fields[0] = _type.DefineField("0", Metadata<Func<T>>.Type, FieldAttributes.Public));
                    _constructor.Emit(OpCodes.Ret);
                    _array = _type.DefineMethod("Array", MethodAttributes.Public, CallingConventions.HasThis, Metadata<T[]>.Type, Type.EmptyTypes).GetILGenerator();
                    _array.Emit(OpCodes.Ldc_I4_1);
                    _array.Emit(OpCodes.Newarr, Metadata<T>.Type);
                    _array.Emit(OpCodes.Dup);
                    _array.Emit(OpCodes.Ldc_I4_0);
                    _array.Emit(OpCodes.Ldarg_0);
                    _array.Emit(OpCodes.Ldfld, _field);
                    _array.Emit(OpCodes.Call, Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
                    _array.Emit(OpCodes.Stelem_Ref);
                    _array.Emit(OpCodes.Ret);
                    _factory = _type.CreateType();
                    Lookup<T>.Type = new KeyValuePair<Type, FieldBuilder[]>[] { new KeyValuePair<Type, FieldBuilder[]>(_type, _fields) };
                    Lookup<T>.m_Array = Delegate.CreateDelegate(Metadata<Func<T[]>>.Type, _factory.GetConstructors()[0].Invoke(new object[] { instance }), _factory.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]) as Func<T[]>;
                }
                else
                {
                    Lookup<T>.m_Instance = Lookup<T>.Multiple;
                    _fields = new FieldBuilder[Lookup<T>.Type.Length + 1];
                    _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Lookup<T>.Type[Lookup<T>.Type.Length - 1].Key, Metadata<Func<T>>.Type }).GetILGenerator();
                    _array = _type.DefineMethod("Array", MethodAttributes.Public, CallingConventions.HasThis, Metadata<T[]>.Type, Type.EmptyTypes).GetILGenerator();
                    _array.Emit(OpCodes.Ldc_I4, Lookup<T>.Type.Length + 1);
                    _array.Emit(OpCodes.Newarr, Metadata<T>.Type);
                    for (var _index = 0; _index < Lookup<T>.Type.Length; _index++)
                    {
                        _constructor.Emit(OpCodes.Ldarg_0);
                        _constructor.Emit(OpCodes.Ldarg_1);
                        _constructor.Emit(OpCodes.Ldfld, Lookup<T>.Module.GetType((Lookup<T>.Type.Length - 1).ToString()).GetField(_index.ToString()));
                        _constructor.Emit(OpCodes.Stfld, _field = _fields[_index] = _type.DefineField(_index.ToString(), Metadata<Func<T>>.Type, FieldAttributes.Public));
                        _array.Emit(OpCodes.Dup);
                        switch (_index)
                        {
                            case 0: _array.Emit(OpCodes.Ldc_I4_0); break;
                            case 1: _array.Emit(OpCodes.Ldc_I4_1); break;
                            case 2: _array.Emit(OpCodes.Ldc_I4_2); break;
                            case 3: _array.Emit(OpCodes.Ldc_I4_3); break;
                            case 4: _array.Emit(OpCodes.Ldc_I4_4); break;
                            case 5: _array.Emit(OpCodes.Ldc_I4_5); break;
                            case 6: _array.Emit(OpCodes.Ldc_I4_6); break;
                            case 7: _array.Emit(OpCodes.Ldc_I4_7); break;
                            case 8: _array.Emit(OpCodes.Ldc_I4_8); break;
                            default: _array.Emit(OpCodes.Ldc_I4_S, _index); break;
                        }
                        _array.Emit(OpCodes.Ldarg_0);
                        _array.Emit(OpCodes.Ldfld, _field);
                        _array.Emit(OpCodes.Call, Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
                        _array.Emit(OpCodes.Stelem_Ref);
                    }
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Ldarg_2);
                    _constructor.Emit(OpCodes.Stfld, _field = _fields[Lookup<T>.Type.Length] = _type.DefineField(Lookup<T>.Type.Length.ToString(), Metadata<Func<T>>.Type, FieldAttributes.Public));
                    _constructor.Emit(OpCodes.Ret);
                    _array.Emit(OpCodes.Dup);
                    _array.Emit(OpCodes.Ldc_I4, Lookup<T>.Type.Length);
                    _array.Emit(OpCodes.Ldarg_0);
                    _array.Emit(OpCodes.Ldfld, _field);
                    _array.Emit(OpCodes.Call, Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
                    _array.Emit(OpCodes.Stelem_Ref);
                    _array.Emit(OpCodes.Ret);
                    _factory = _type.CreateType();
                    var _buffer = new KeyValuePair<Type, FieldBuilder[]>[Lookup<T>.Type.Length + 1];
                    for (var _index = 0; _index < Lookup<T>.Type.Length; _index++) { _buffer[_index] = Lookup<T>.Type[_index]; }
                    _buffer[Lookup<T>.Type.Length] = new KeyValuePair<Type, FieldBuilder[]>(_type, _fields);
                    Lookup<T>.Type = _buffer;
                    Lookup<T>.m_Array = Delegate.CreateDelegate(Metadata<Func<T[]>>.Type, _factory.GetConstructors()[0].Invoke(new object[] { Lookup<T>.Array.Target, instance }), _factory.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]) as Func<T[]>;
                }
            }
        }

        static public void Add<T>(Type type)
            where T : class
        {
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.GetConstructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile());
            }
            else { Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile()); }
        }

        static public void Add<T>(ConstructorInfo constructor)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile()); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile());
            }
        }

        static public void Add<T>(MethodInfo method)
            where T : class
        {
            var _signature = method.GetParameters();
            if (_signature.Length == 0) { throw new NotSupportedException(); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Metadata<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, Metadata<IDeserializationCallback>.Type), Metadata<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Argument<object>.Value)), Expression.Constant(null, Metadata<object>.Type)), Expression<T>.Parameter)).Compile()); }
                    else { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile()); }
                }
                else { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.New(_constructor), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile()); }
            }
        }

        static public void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, null) || object.ReferenceEquals(lifetime, Lifetime.Volatile)) { Add<T>(instance); }
            else if (object.ReferenceEquals(lifetime, Lifetime.Singleton))
            {
                var _handle = new object();
                var _return = null as Func<T>;
                var _instance = null as Func<T>;
                Add<T>(_instance = new Func<T>(() =>
                {
                    lock (_handle)
                    {
                        if (object.Equals(_return, null))
                        {
                            var _singleton = instance();
                            _return = new Func<T>(() => _singleton);
                            Interlocked.CompareExchange(ref Lookup<T>.m_Instance, _return, _instance);
                            var _linkup = Lookup<T>.Linkup;
                            if (_linkup != null) { _linkup.Update(instance, _return); }
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
        
        static public void Add<T>(Type type, Lifetime lifetime)
            where T : class
        {
            var _constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (_constructor == null)
            {
                _constructor = type.GetConstructors().SingleOrDefault();
                if (_constructor == null) { throw new NotSupportedException(); }
                var _signature = _constructor.GetParameters();
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor, _arguments)).Compile(), lifetime);
            }
            else { Add<T>(Expression.Lambda<Func<T>>(Expression.New(_constructor)).Compile(), lifetime); }
        }

        static public void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class
        {
            var _signature = constructor.GetParameters();
            if (_signature.Length == 0) { Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile(), lifetime); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile(), lifetime);
            }
        }

        static public void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class
        {
            var _signature = method.GetParameters();
            if (_signature.Length == 0) { throw new NotSupportedException(); }
            else
            {
                var _arguments = new Expression[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    var _parameter = _signature[_index];
                    var _type = _parameter.ParameterType;
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue() ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Metadata.Method(() => Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Metadata<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, Metadata<IDeserializationCallback>.Type), Metadata<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Argument<object>.Value)), Expression.Constant(null, Metadata<object>.Type)), Expression<T>.Parameter)).Compile(), lifetime); }
                    else { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.Call(Metadata.Method(() => FormatterServices.GetUninitializedObject(Argument<Type>.Value)), Expression.Field(null, Metadata.Field(() => Metadata<T>.Type))), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile(), lifetime); }
                }
                else { Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Expression<T>.Parameter }, Expression.Assign(Expression<T>.Parameter, Expression.TypeAs(Expression.New(_constructor), Metadata<T>.Type)), Expression.Call(Expression.TypeAs(Expression<T>.Parameter, method.DeclaringType), method, _arguments), Expression<T>.Parameter)).Compile(), lifetime); }
            }
        }
        
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        static public T Instance<T>()
            where T : class
        {
            return Lookup<T>.m_Instance();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        static public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Lookup<T>.m_Array();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        static public T[] Array<T>()
            where T : class
        {
            return Lookup<T>.m_Array();
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
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }
}
