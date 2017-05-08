using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public partial class Proxy<TAspect>
        where TAspect : Aspect, new()
    {
        static public TInterface Create<TInterface>(TInterface instance)
            where TInterface : class
        {
            return Proxy<TAspect, TInterface>.Create(instance);
        }
    }

    static internal class Proxy<TAspect, TInterface>
        where TAspect : Aspect, new()
        where TInterface : class
    {
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
        static public readonly Func<TInterface, TInterface> Create = Expression.Lambda<Func<TInterface, TInterface>>(Expression.New(Proxy<TAspect, TInterface>.Type(Proxy<TAspect, TInterface>.Intermediate()).Constructors().Single(), Parameter<TInterface>.Expression), Parameter<TInterface>.Expression).Compile();

        static private Dictionary<MethodInfo, MethodInfo> Intermediate()
        {
            var _type = Proxy<TAspect, TInterface>.m_Module.DefineType($"{ Runtime<TInterface>.Type.Name }{ Guid.NewGuid().ToString("N") }", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Serializable);
            foreach (var _method in Runtime<TInterface>.Methods(true)) { Proxy<TAspect, TInterface>.Intermediate(_type, _method); }
            var _intermediate = _type.CreateType();
            var _dicionary = new Dictionary<MethodInfo, MethodInfo>();
            foreach (var _method in Runtime<TInterface>.Methods(true)) { _dicionary.Add(_method, _intermediate.Methods(true).Single(_Method => _Method.Name == _method.Name && _Method.Signature() == _method.Signature())); }
            return _dicionary;
        }

        static private void Intermediate(TypeBuilder type, MethodInfo method)
        {
            var _signature = method.Signature();
            var _method = type.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, method.ReturnType, _signature);
            var _body = _method.GetILGenerator();
            _body.Emit(_signature, false);
            _body.Emit(OpCodes.Callvirt, method);
            _body.Emit(OpCodes.Ret);
        }

        static private Type Type(Dictionary<MethodInfo, MethodInfo> intermediate)
        {
            var _type = Proxy<TAspect, TInterface>.m_Module.DefineType($"{ Runtime<TInterface>.Type.Name }{ Guid.NewGuid().ToString("N") }", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, Runtime<object>.Type, new Type[] { Runtime<TInterface>.Type });
            var _field = _type.DefineField("Instance", Runtime<TInterface>.Type, FieldAttributes.Public);
            var _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { Runtime<TInterface>.Type });
            var _body = _constructor.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Call, Runtime.Constructor(() => new object()));
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldarg_1);
            _body.Emit(OpCodes.Stfld, _field);
            _body.Emit(OpCodes.Ret);
            var _aspectization = new List<MethodInfo>();
            foreach (var _method in Runtime<TInterface>.Methods(true)) { _aspectization.AddRange(Proxy<TAspect, TInterface>.Method(_type, _field, _method, intermediate[_method])); }
            _type.DefineField("Handle", Runtime<MethodInfo[]>.Type, FieldAttributes.Static | FieldAttributes.Private);
            var _handle = _type.CreateType().Fields().Single(_Field => _Field.IsStatic);
            _handle.SetValue(null, _aspectization.ToArray());
            return _handle.DeclaringType;
        }

        static private IEnumerable<MethodInfo> Method(TypeBuilder type, FieldBuilder field, MethodInfo method, MethodInfo intermediate)
        {
            var _parameters = method.GetParameters();
            var _signature = method.Signature();
            var _method = type.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, method.ReturnType, _parameters.Select(_Parameter => _Parameter.ParameterType).ToArray());
            var _body = _method.GetILGenerator();
            var _pointer = intermediate.GetFunctionPointer();
            foreach (var _advice in Singleton<TAspect>.Value.Advise(method))
            {
                var _advised = _advice.Decorate(method, _pointer);
                if (_advised == null || _advised == method) { continue; }
                yield return _advised;
                _pointer = _advised.GetFunctionPointer();
            }
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, field);
            for (var _index = 0; _index < _parameters.Length; _index++)
            {
                switch (_index)
                {
                    case 0: _body.Emit(OpCodes.Ldarg_1); break;
                    case 1: _body.Emit(OpCodes.Ldarg_2); break;
                    case 2: _body.Emit(OpCodes.Ldarg_3); break;
                    default: _body.Emit(OpCodes.Ldarg_S, _index + 1); break;
                }
            }
            _body.Emit(_pointer, method.ReturnType, method.Signature());
            _body.Emit(OpCodes.Ret);
            type.DefineMethodOverride(_method, method);
            yield break;
        }
    }
}
