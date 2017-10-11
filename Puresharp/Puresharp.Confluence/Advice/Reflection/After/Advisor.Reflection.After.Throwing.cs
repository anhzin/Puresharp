using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    static public partial class __Advice
    {
        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Throwing(this Advice.Style.Reflection.IAfter reflection, Action<ILGenerator> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                var _type = _Method.ReturnType();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (_Boundary != null) //TODO dynamic implementation of boundary will avoid redirection overhead.
                {
                    var _action = new DynamicMethod(string.Empty, Runtime.Void, new Type[] { Metadata<object>.Type, Metadata<object>.Type, Metadata<object[]>.Type, Metadata<Exception>.Type }, true);
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
                    if (_type != Runtime.Void)
                    {
                        var _exception = Advice.Module.DefineThreadField("<Exception>", Metadata<Exception>.Type);
                        _redirection.Emit(OpCodes.Ldarg_3);
                        _redirection.Emit(OpCodes.Stsfld, _exception);
                        _body.Emit(OpCodes.Ldsfld, _exception);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Stsfld, _exception);
                    }
                    _body.Emit(advice);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    _redirection.Emit(OpCodes.Call, _method);
                    _redirection.Emit(OpCodes.Ret);
                    _action.Prepare();
                    return _Boundary.Combine(new Advice.Boundary.Advanced.After.Throwing.Singleton(_action.CreateDelegate(Metadata<Action<object, object[], Exception>>.Type, null) as Action<object, object[], Exception>));
                }
                else
                {
                    if (_type == Runtime.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(_Pointer, _type, _signature);
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(advice);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(_Pointer, _type, _signature);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(advice);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return _method;
                }
            });
        }
    }
}