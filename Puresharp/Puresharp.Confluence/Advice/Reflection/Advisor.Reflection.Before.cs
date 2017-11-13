using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    static public partial class __Advice
    {
        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Before(this Advice.Style.IReflection reflection, Action<ILGenerator> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (_Boundary != null) //TODO dynamic implementation of boundary will avoid redirection overhead.
                {
                    _body.Emit(advice);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    var _action = new DynamicMethod(string.Empty, Runtime.Void, new Type[] { Metadata<object>.Type, Metadata<object>.Type, Metadata<object[]>.Type }, true);
                    var _redirection = _action.GetILGenerator();
                    if (_signature.Instance != null)
                    {
                        _redirection.Emit(OpCodes.Ldarg_1);
                        if (_signature.Instance.IsValueType) { _redirection.Emit(OpCodes.Unbox_Any, _signature.Instance); }
                        else { _redirection.Emit(OpCodes.Castclass, _signature.Instance); }
                    }
                    for (var _index = 0; _index < _signature.Length; _index++)
                    {
                        var _parameter = _signature[_index];
                        _redirection.Emit(OpCodes.Ldarg_2);
                        _redirection.Emit(OpCodes.Ldc_I4, _index); //TODO shortcut
                        _redirection.Emit(OpCodes.Ldelem_Ref);
                        if (_parameter.IsValueType) { _redirection.Emit(OpCodes.Unbox_Any, _parameter); }
                        else { _redirection.Emit(OpCodes.Castclass, _parameter); }
                    }
                    _redirection.Emit(OpCodes.Call, _method);
                    _redirection.Emit(OpCodes.Ret);
                    _action.Prepare();
                    return _Boundary.Combine(new Advice.Boundary.Advanced.Before.Singleton(_Method, _action.CreateDelegate(Metadata<Action<object, object[]>>.Type, null) as Action<object, object[]>));
                }
                else
                {
                    _body.Emit(advice);
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return _method;
                }
            });
        }
    }
}