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

namespace Puresharp
{
    public sealed partial class Composition : IComposition
    {
        //static public IComposition Create()
        //{
        //    var _type = AppDomain.CurrentDomain.DefineDynamicModule().DefineType(Guid.NewGuid().ToString("N")).CreateType();
        //    return Activator.CreateInstance(typeof(PPP<>).MakeGenericType(_type)) as IComposition;
        //}

        //internal class PPP<T> : IComposition
        //    where T : class
        //{
        //    public void Add<T1>(IEnumerable<T1> enumerable) where T1 : class
        //    {
        //        Composition<T>.Add(enumerable);
        //    }

        //    public void Add<T1>(Type type) where T1 : class
        //    {
        //        Composition<T>.Add(type);
        //    }

        //    public void Add<T1>(MethodInfo method) where T1 : class
        //    {
        //        Composition<T>.Add(method);
        //    }

        //    public void Add<T1>(ConstructorInfo constructor) where T1 : class
        //    {
        //        Composition<T>.Add(constructor);
        //    }

        //    public void Add<T1>(Func<T1> instance) where T1 : class
        //    {
        //        Composition<T>.Add(instance);
        //    }

        //    public void Add<T1>(params T1[] array) where T1 : class
        //    {
        //        Composition<T>.Add(array);
        //    }

        //    public void Add<T1>(T1 instance) where T1 : class
        //    {
        //        Composition<T>.Add(instance);
        //    }

        //    public void Add<T1>(MethodInfo method, Lifetime lifetime) where T1 : class
        //    {
        //        //Composition<T>.Add(method, lifetime);
        //    }

        //    public void Add<T1>(ConstructorInfo constructor, Lifetime lifetime) where T1 : class
        //    {
        //        //Composition<T>.Add(constructor, lifetime);
        //    }

        //    public void Add<T1>(Type type, Lifetime lifetime) where T1 : class
        //    {
        //        //Composition<T>.Add(array);
        //    }

        //    public void Add<T1>(Func<T1> instance, Lifetime lifetime) where T1 : class
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public T1[] Array<T1>() where T1 : class
        //    {
        //        return Composition<T>.Lookup<T1>.Array();
        //    }

        //    public void Dispose()
        //    {
        //    }

        //    public IEnumerable<T1> Enumerable<T1>() where T1 : class
        //    {
        //        return Composition<T>.Lookup<T1>.Enumerable();
        //    }

        //    [DebuggerNonUserCode]
        //    [DebuggerStepThrough]
        //    [DebuggerHidden]
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    public T1 Instance<T1>() where T1 : class
        //    {
        //        return Composition<T>.Lookup<T1>.Instance();
        //    }
        //}

        //static public


        static private object m_Handle = new object();
        static private List<Action> m_Instantiation = new List<Action>();
        static private int m_Sequence = 0;

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator int(Composition composition)
        {
            return composition.m_Index;
        }

        private int m_Index;

        public Composition()
        {
            lock (Composition.m_Handle)
            {
                this.m_Index = Composition.m_Sequence++;
                for (var _index = 0; _index < Composition.m_Instantiation.Count; _index++) { Composition.m_Instantiation[_index](); }
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
            var _container = Composition.Lookup<T>.Buffer[this.m_Index];
            while (true)
            {
                var _instance = _container.Instance;
                if (_instance == Composition.Container<T>.None)
                {
                    if (Interlocked.CompareExchange(ref _container.Instance, instance, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Update(ref _container.Linkup, instance);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Composition.Container<T>.Empty, _array) == _array) { return; } }
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
                    if (Interlocked.CompareExchange(ref _container.Instance, Composition.Container<T>.Multiple, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Update(ref _container.Linkup, instance);
                        while (true)
                        {
                            var _array = _container.Array;
                            var _linkup = _container.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref _container.Array, Composition.Container<T>.Empty, _array) == _array) { return; } }
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
            var _container = Composition.Lookup<T>.Buffer[this.m_Index];
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
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Runtime<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, Runtime<IDeserializationCallback>.Type), Runtime<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Runtime<object>.Value)), Expression.Constant(null, Runtime<object>.Type)), Parameter<T>.Expression)).Compile()); }
                    else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile()); }
                }
                else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.New(_constructor), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile()); }
            }
        }

        public void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, null) || object.ReferenceEquals(lifetime, Lifetime.Volatile)) { this.Add<T>(instance); }
            else if (object.ReferenceEquals(lifetime, Lifetime.Singleton))
            {
                var _container = Composition.Lookup<T>.Buffer[this.m_Index];
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
            else if (lifetime is Lifetime.ICycle)
            {
                Composition.Scope<T> _scope = null;
                (lifetime as Lifetime.ICycle).Establish<Composition.Scope<T>>(() => _scope = new Composition.Scope<T>(instance));
                this.Add<T>(new Func<T>(() => _scope.Instance));
            }
            else
            {
                //TODO!
                throw new NotSupportedException();
            }
        }

        public void Add<T>(Type type, Lifetime lifetime)
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
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition.Linq.Instance<object>(this.m_Index)).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Runtime<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, Runtime<IDeserializationCallback>.Type), Runtime<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Runtime<object>.Value)), Expression.Constant(null, Runtime<object>.Type)), Parameter<T>.Expression)).Compile(), lifetime); }
                    else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile(), lifetime); }
                }
                else { this.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.New(_constructor), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile(), lifetime); }
            }
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instance<T>()
            where T : class
        {
            return Composition.Lookup<T>.Buffer[this.m_Index].Instance();
        }

        T IComposition.Instance<T>()
        {
            return Composition.Lookup<T>.Buffer[this.m_Index].Instance();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Composition.Lookup<T>.Buffer[this.m_Index].Array();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] Array<T>()
            where T : class
        {
            return Composition.Lookup<T>.Buffer[this.m_Index].Array();
        }

        public void Dispose()
        {
        }
    }

    static public partial class Composition<X>
        where X : class
    {
        static public void Add<T>(T instance)
            where T : class
        {
            Composition<X>.Add(() => instance);
        }

        static public void Add<T>(IEnumerable<T> enumerable)
            where T : class
        {
            if (enumerable == null) { return; }
            foreach (var _instance in enumerable) { Composition<X>.Add<T>(new Func<T>(() => _instance)); }
        }
        
        static public void Add<T>(Func<T> instance)
            where T : class
        {
            lock (Composition<X>.Lookup<T>.Handle)
            {
                var _type = Composition<X>.Lookup<T>.Module.DefineType(Composition<X>.Lookup<T>.Type.Length.ToString(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
                var _array = null as ILGenerator;
                var _enumerable = null as ILGenerator;
                var _field = null as FieldBuilder;
                var _fields = null as FieldBuilder[];
                var _constructor = null as ILGenerator;
                var _factory = null as Type;
                var _method = null as DynamicMethod;
                var _delegate = null as Func<object, Func<T>, Func<T[]>>;
                if (Composition<X>.Lookup<T>.m_Instance == Composition<X>.Lookup<T>.None)
                {
                    Composition<X>.Lookup<T>.m_Instance = instance;
                    _fields = new FieldBuilder[1];
                    _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Runtime<Func<T>>.Type }).GetILGenerator();
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Ldarg_1);
                    _constructor.Emit(OpCodes.Stfld, _field = _fields[0] = _type.DefineField("0", Runtime<Func<T>>.Type, FieldAttributes.Public));
                    _constructor.Emit(OpCodes.Ret);
                    _array = _type.DefineMethod("Array", MethodAttributes.Public, CallingConventions.HasThis, Runtime<T[]>.Type, Type.EmptyTypes).GetILGenerator();
                    _array.Emit(OpCodes.Ldc_I4_1);
                    _array.Emit(OpCodes.Newarr, Runtime<T>.Type);
                    _array.Emit(OpCodes.Dup);
                    _array.Emit(OpCodes.Ldc_I4_0);
                    _array.Emit(OpCodes.Ldarg_0);
                    _array.Emit(OpCodes.Ldfld, _field);
                    _array.Emit(OpCodes.Call, Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
                    _array.Emit(OpCodes.Stelem_Ref);
                    _array.Emit(OpCodes.Ret);
                    _factory = _type.CreateType();
                    Composition<X>.Lookup<T>.Type = new KeyValuePair<Type, FieldBuilder[]>[] { new KeyValuePair<Type, FieldBuilder[]>(_type, _fields) };
                    Composition<X>.Lookup<T>.m_Array = Delegate.CreateDelegate(Runtime<Func<T[]>>.Type, _factory.GetConstructors()[0].Invoke(new object[] { instance }), _factory.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]) as Func<T[]>;
                }
                else
                {
                    Composition<X>.Lookup<T>.m_Instance = Composition<X>.Lookup<T>.Multiple;
                    _fields = new FieldBuilder[Composition<X>.Lookup<T>.Type.Length + 1];
                    _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Composition<X>.Lookup<T>.Type[Composition<X>.Lookup<T>.Type.Length - 1].Key, Runtime<Func<T>>.Type }).GetILGenerator();
                    _array = _type.DefineMethod("Array", MethodAttributes.Public, CallingConventions.HasThis, Runtime<T[]>.Type, Type.EmptyTypes).GetILGenerator();
                    _array.Emit(OpCodes.Ldc_I4, Composition<X>.Lookup<T>.Type.Length + 1);
                    _array.Emit(OpCodes.Newarr, Runtime<T>.Type);
                    for (var _index = 0; _index < Composition<X>.Lookup<T>.Type.Length; _index++)
                    {
                        _constructor.Emit(OpCodes.Ldarg_0);
                        _constructor.Emit(OpCodes.Ldarg_1);
                        _constructor.Emit(OpCodes.Ldfld, Composition<X>.Lookup<T>.Module.GetType((Composition<X>.Lookup<T>.Type.Length - 1).ToString()).GetField(_index.ToString()));
                        _constructor.Emit(OpCodes.Stfld, _field = _fields[_index] = _type.DefineField(_index.ToString(), Runtime<Func<T>>.Type, FieldAttributes.Public));
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
                        _array.Emit(OpCodes.Call, Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
                        _array.Emit(OpCodes.Stelem_Ref);
                    }
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Ldarg_2);
                    _constructor.Emit(OpCodes.Stfld, _field = _fields[Composition<X>.Lookup<T>.Type.Length] = _type.DefineField(Composition<X>.Lookup<T>.Type.Length.ToString(), Runtime<Func<T>>.Type, FieldAttributes.Public));
                    _constructor.Emit(OpCodes.Ret);
                    _array.Emit(OpCodes.Dup);
                    _array.Emit(OpCodes.Ldc_I4, Composition<X>.Lookup<T>.Type.Length);
                    _array.Emit(OpCodes.Ldarg_0);
                    _array.Emit(OpCodes.Ldfld, _field);
                    _array.Emit(OpCodes.Call, Runtime<Func<T>>.Method(_Function => _Function.Invoke()));
                    _array.Emit(OpCodes.Stelem_Ref);
                    _array.Emit(OpCodes.Ret);
                    _factory = _type.CreateType();
                    var _buffer = new KeyValuePair<Type, FieldBuilder[]>[Composition<X>.Lookup<T>.Type.Length + 1];
                    for (var _index = 0; _index < Composition<X>.Lookup<T>.Type.Length; _index++) { _buffer[_index] = Composition<X>.Lookup<T>.Type[_index]; }
                    _buffer[Composition<X>.Lookup<T>.Type.Length] = new KeyValuePair<Type, FieldBuilder[]>(_type, _fields);
                    Composition<X>.Lookup<T>.Type = _buffer;
                    Composition<X>.Lookup<T>.m_Array = Delegate.CreateDelegate(Runtime<Func<T[]>>.Type, _factory.GetConstructors()[0].Invoke(new object[] { Composition<X>.Lookup<T>.Array.Target, instance }), _factory.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0]) as Func<T[]>;
                }
            }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Runtime<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, Runtime<IDeserializationCallback>.Type), Runtime<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Runtime<object>.Value)), Expression.Constant(null, Runtime<object>.Type)), Parameter<T>.Expression)).Compile()); }
                    else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile()); }
                }
                else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.New(_constructor), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile()); }
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
                Composition.Scope<T> _scope = null;
                (lifetime as Lifetime.ICycle).Establish<Composition.Scope<T>>(() => _scope = new Composition.Scope<T>(instance));
                Composition<X>.Add<T>(new Func<T>(() => _scope.Instance));
            }
            else { throw new NotSupportedException(); }
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
                    if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
                    else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
                    else { throw new NotSupportedException(); }
                }
                var _constructor = method.ReflectedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
                if (_constructor == null)
                {
                    if (Runtime<IDeserializationCallback>.Type.IsAssignableFrom(method.ReflectedType)) { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, Runtime<IDeserializationCallback>.Type), Runtime<IDeserializationCallback>.Method(_IDeserializationCallback => _IDeserializationCallback.OnDeserialization(Runtime<object>.Value)), Expression.Constant(null, Runtime<object>.Type)), Parameter<T>.Expression)).Compile(), lifetime); }
                    else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.Call(Runtime.Method(() => FormatterServices.GetUninitializedObject(Runtime<Type>.Value)), Expression.Field(null, Runtime.Field(() => Runtime<T>.Type))), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile(), lifetime); }
                }
                else { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.Block(new ParameterExpression[] { Parameter<T>.Expression }, Expression.Assign(Parameter<T>.Expression, Expression.TypeAs(Expression.New(_constructor), Runtime<T>.Type)), Expression.Call(Expression.TypeAs(Parameter<T>.Expression, method.DeclaringType), method, _arguments), Parameter<T>.Expression)).Compile(), lifetime); }
            }
        }

        //static public void Add<T>(MethodInfo constructor, Lifetime lifetime)
        //    where T : class
        //{
        //    var _signature = constructor.GetParameters();
        //    if (_signature.Length == 0) { Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile(), lifetime); }
        //    else
        //    {
        //        var _arguments = new Expression[_signature.Length];
        //        for (var _index = 0; _index < _signature.Length; _index++)
        //        {
        //            var _parameter = _signature[_index];
        //            var _type = _parameter.ParameterType;
        //            if (_parameter.IsOptional) { _arguments[_index] = _parameter.HasDefaultValue ? Expression.Constant(_parameter.DefaultValue, _type) as Expression : Expression.Default(_type); }
        //            else if (_parameter.ParameterType.IsClass && !_parameter.ParameterType.IsSealed) { _arguments[_index] = Runtime.Method(() => Composition<X>.Linq.Instance<object>()).GetGenericMethodDefinition().MakeGenericMethod(_type).Invoke(null, new object[0]) as Expression; }
        //            else { throw new NotSupportedException(); }
        //        }
        //        Composition<X>.Add<T>(Expression.Lambda<Func<T>>(Expression.New(constructor, _arguments)).Compile(), lifetime);
        //    }
        //}

        //static public void Add<T>(string directory, SearchOption option)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory, SearchOption option, Func<Type, bool> predicate)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, predicate);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory, Func<Type, bool> predicate)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, predicate);
        //        }
        //    }
        //}

        //static public void Add<T>(IEnumerable<Assembly> assemblies)
        //    where T : class
        //{
        //    if (assemblies == null) { return; }
        //    foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly); }
        //}

        //static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        //    where T : class
        //{
        //    if (assemblies == null) { return; }
        //    foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, predicate); }
        //}

        //static public void Add<T>(Assembly assembly)
        //    where T : class
        //{
        //    var _types = assembly.GetTypes();
        //    for (var _index = 0; _index < _types.Length; _index++)
        //    {
        //        var _type = _types[_index];
        //        if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type)) { Composition<X>.Add<T>(_type); }
        //    }
        //}

        //static public void Add<T>(Assembly assembly, Func<Type, bool> predicate)
        //    where T : class
        //{
        //    var _types = assembly.GetTypes();
        //    for (var _index = 0; _index < _types.Length; _index++)
        //    {
        //        var _type = _types[_index];
        //        if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type) && ( predicate == null || predicate(_type))) { Composition<X>.Add<T>(_type); }
        //    }
        //}

        //static public void Add<T>(IEnumerable<Type> types)
        //    where T : class
        //{
        //    if (types == null) { return; }
        //    foreach (var _type in types) { Composition<X>.Add<T>(_type); }
        //}

        //static public void Add<T>(string directory, SearchOption option, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, lifetime);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory, SearchOption option, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", option))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, predicate, lifetime);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, lifetime);
        //        }
        //    }
        //}

        //static public void Add<T>(string directory, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    foreach (var _filename in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
        //    {
        //        var _extension = Path.GetExtension(_filename);
        //        if (string.Equals(_extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(_extension, ".exe", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Assembly _assembly = null;
        //            try { _assembly = Assembly.LoadFrom(_filename); }
        //            catch { }
        //            if (_assembly == null) { continue; }
        //            Composition<X>.Add<T>(_assembly, predicate, lifetime);
        //        }
        //    }
        //}

        //static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    if (assemblies == null) { return; }
        //    foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, lifetime); }
        //}

        //static public void Add<T>(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    if (assemblies == null) { return; }
        //    foreach (var _assembly in assemblies) { Composition<X>.Add<T>(_assembly, predicate, lifetime); }
        //}

        //static public void Add<T>(Assembly assembly, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    var _types = assembly.GetTypes();
        //    for (var _index = 0; _index < _types.Length; _index++)
        //    {
        //        var _type = _types[_index];
        //        if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type)) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
        //    }
        //}

        //static public void Add<T>(Assembly assembly, Func<Type, bool> predicate, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    var _types = assembly.GetTypes();
        //    for (var _index = 0; _index < _types.Length; _index++)
        //    {
        //        var _type = _types[_index];
        //        if (_type.IsClass && !_type.IsAbstract && Runtime<T>.Type.IsAssignableFrom(_type) && (predicate == null || predicate(_type))) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
        //    }
        //}

        //static public void Add<T>(IEnumerable<Type> types, Func<Type, Lifetime> lifetime)
        //    where T : class
        //{
        //    if (types == null) { return; }
        //    foreach (var _type in types) { Composition<X>.Add<T>(_type, lifetime == null ? Lifetime.Volatile : lifetime(_type)); }
        //}

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
            return Composition<X>.Lookup<T>.m_Array();
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T[] Array<T>()
            where T : class
        {
            return Composition<X>.Lookup<T>.m_Array();
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
    }
}
