using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Puresharp.Composition
{
    static internal class Multiton
    {
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run).DefineDynamicModule(Guid.NewGuid().ToString("N"));

        static public ModuleBuilder Module
        {
            get { return Multiton.m_Module; }
        }
    }

    static internal class Multiton<T>
    {
        static private Func<Func<T>[], Func<IEnumerable<T>>>[] m_Creation = new Func<Func<T>[], Func<IEnumerable<T>>>[0];

        static private Func<Func<T>[], Func<IEnumerable<T>>> Prepare(int length)
        {
            var _type = Multiton.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Serializable | TypeAttributes.SequentialLayout | TypeAttributes.Public, Metadata<object>.Type, new Type[] { Metadata<IEnumerable<T>>.Type });
            var _array = new FieldBuilder[length];
            var _body = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Metadata<Func<T>[]>.Type }).GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Call, Metadata.Constructor(() => new object()));
            for (var _index = 0; _index < length; _index++)
            {
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_1);
                switch (_index)
                {
                    case 0: _body.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: _body.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: _body.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: _body.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: _body.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: _body.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: _body.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: _body.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: _body.Emit(OpCodes.Ldc_I4_8); break;
                    default: _body.Emit(OpCodes.Ldc_I4_S, _index); break;
                }
                _body.Emit(OpCodes.Ldelem_Ref);
                _body.Emit(OpCodes.Call, Metadata<Func<T>>.Method(_Function => _Function.Invoke()));
                _body.Emit(OpCodes.Stfld, _array[_index] = _type.DefineField(_index.ToString(), Metadata<T>.Type, FieldAttributes.Public));
            }
            _body.Emit(OpCodes.Ret);
            var _enumerator = _type.DefineNestedType(Guid.NewGuid().ToString("N"), TypeAttributes.NestedPublic, Metadata<object>.Type, new Type[] { Metadata<IEnumerator<T>>.Type });
            var _field = _enumerator.DefineField(Guid.NewGuid().ToString("N"), _type, FieldAttributes.Public);
            var _position = _enumerator.DefineField(Guid.NewGuid().ToString("N"), Metadata<int>.Type, FieldAttributes.Public);
            var _constructor = _enumerator.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { _type });
            _body = _constructor.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Call, Metadata.Constructor(() => new object()));
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldarg_1);
            _body.Emit(OpCodes.Stfld, _field);
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldc_I4_M1);
            _body.Emit(OpCodes.Stfld, _position);
            _body.Emit(OpCodes.Ret);
            var _method = _enumerator.DefineMethod("Move", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata<bool>.Type, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            var _false = _body.DefineLabel();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, _position);
            switch (length)
            {
                case 0: _body.Emit(OpCodes.Ldc_I4_M1); break;
                case 1: _body.Emit(OpCodes.Ldc_I4_0); break;
                case 2: _body.Emit(OpCodes.Ldc_I4_1); break;
                case 3: _body.Emit(OpCodes.Ldc_I4_2); break;
                case 4: _body.Emit(OpCodes.Ldc_I4_3); break;
                case 5: _body.Emit(OpCodes.Ldc_I4_4); break;
                case 6: _body.Emit(OpCodes.Ldc_I4_5); break;
                case 7: _body.Emit(OpCodes.Ldc_I4_6); break;
                case 8: _body.Emit(OpCodes.Ldc_I4_7); break;
                default: _body.Emit(OpCodes.Ldc_I4_S, length - 1); break;
            }
            _body.Emit(OpCodes.Bge_S, _false);
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, _position);
            _body.Emit(OpCodes.Ldc_I4_1);
            _body.Emit(OpCodes.Add);
            _body.Emit(OpCodes.Stfld, _position);
            _body.Emit(OpCodes.Ldc_I4_1);
            _body.Emit(OpCodes.Ret);
            _body.MarkLabel(_false);
            _body.Emit(OpCodes.Ldc_I4_0);
            _body.Emit(OpCodes.Ret);
            _enumerator.DefineMethodOverride(_method, Metadata<IEnumerator>.Method(_IEnumerator => _IEnumerator.MoveNext()));
            _method = _enumerator.DefineMethod("Reset", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata.Void, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldc_I4_0);
            _body.Emit(OpCodes.Stfld, _position);
            _enumerator.DefineMethodOverride(_method, Metadata<IEnumerator>.Method(_IEnumerator => _IEnumerator.Reset()));
            //var _property = _enumerator.DefineProperty("IEnumerable.Current", PropertyAttributes.None, CallingConventions.HasThis, Metadata<object>.Type, Type.EmptyTypes);
            _method = _enumerator.DefineMethod("IEnumerable.Current", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata<object>.Type, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            var _labelization = new Label[length];
            for (var _index = 0; _index < length; _index++) { _labelization[_index] = _body.DefineLabel(); }
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, _position);
            _body.Emit(OpCodes.Switch, _labelization);
            _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
            _body.Emit(OpCodes.Throw);
            for (var _index = 0; _index < length; _index++)
            {
                _body.MarkLabel(_labelization[_index]);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _array[_index]);
                _body.Emit(OpCodes.Ret);
            }
            _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
            _body.Emit(OpCodes.Throw);
            _enumerator.DefineMethodOverride(_method, Metadata<IEnumerator>.Property(_IEnumerator => _IEnumerator.Current).GetGetMethod());
            //_property = _enumerator.DefineProperty("IEnumerable<T>.Current", PropertyAttributes.None, CallingConventions.HasThis, Metadata<T>.Type, Type.EmptyTypes);
            _method = _enumerator.DefineMethod("IEnumerable<T>.Current", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata<T>.Type, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            _labelization = new Label[length];
            for (var _index = 0; _index < length; _index++) { _labelization[_index] = _body.DefineLabel(); }
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, _position);
            _body.Emit(OpCodes.Switch, _labelization);
            _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
            _body.Emit(OpCodes.Throw);
            for (var _index = 0; _index < length; _index++)
            {
                _body.MarkLabel(_labelization[_index]);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ldfld, _array[_index]);
                _body.Emit(OpCodes.Ret);
            }
            _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
            _body.Emit(OpCodes.Throw);
            _enumerator.DefineMethodOverride(_method, Metadata<IEnumerator<T>>.Property(_IEnumerator => _IEnumerator.Current).GetGetMethod());
            _method = _enumerator.DefineMethod("Dispose", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata.Void, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ret);
            _enumerator.DefineMethodOverride(_method, Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()));
            _method = _type.DefineMethod("IEnumerable.Enumerator", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata<IEnumerator>.Type, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Newobj, _constructor);
            _body.Emit(OpCodes.Ret);
            _type.DefineMethodOverride(_method, Metadata<IEnumerable>.Method(_IEnumerable => _IEnumerable.GetEnumerator()));
            _method = _type.DefineMethod("IEnumerable<T>.Enumerator", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, Metadata<IEnumerator<T>>.Type, Type.EmptyTypes);
            _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Newobj, _constructor);
            _body.Emit(OpCodes.Ret);
            _type.DefineMethodOverride(_method, Metadata<IEnumerable<T>>.Method(_IEnumerable => _IEnumerable.GetEnumerator()));
            _enumerator.CreateType();
            return Expression.Lambda<Func<Func<T>[], Func<IEnumerable<T>>>>(Expression.New(_type.CreateType().GetConstructors()[0], Expression<Func<T>[]>.Parameter), Expression<Func<T>[]>.Parameter).Compile();
        }

        static public Func<IEnumerable<T>> Create(Func<T>[] multiton)
        {
            var _creation = Multiton<T>.m_Creation;
            if (multiton.Length < _creation.Length) { return _creation[multiton.Length - 1](multiton); }
            while (true)
            {
                var _buffer = new Func<Func<T>[], Func<IEnumerable<T>>>[multiton.Length + 1];
                for (var _index = 0; _index < _creation.Length; _index++) { _buffer[_index] = _creation[_index]; }
                for (var _index = _creation.Length; _index <= multiton.Length; _index++) { _buffer[_index] = Multiton<T>.Prepare(_index); }
                if (object.ReferenceEquals(Interlocked.CompareExchange(ref Multiton<T>.m_Creation, _buffer, _creation), _creation)) { return _buffer[multiton.Length](multiton); }
            }
        }
    }
}
