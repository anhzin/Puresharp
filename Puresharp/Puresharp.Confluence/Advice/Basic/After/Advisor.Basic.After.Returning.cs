﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    static public partial class __Advice
    {
        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Returning(this Advice.Style.Basic.IAfter basic, Action advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return _Boundary.Combine(new Advice.Boundary.Basic.After.Returning.Singleton(advice)); }
                var _type = _Method.ReturnType();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice.Module.DefineField(advice.Target)); }
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method], object = [return value (null if return type is void)])</param>
        /// <returns>Advice</returns>
        static public Advice Returning(this Advice.Style.Basic.IAfter basic, Action<object, object[], object> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return _Boundary.Combine(new Advice.Boundary.Advanced.After.Returning.Singleton(_Method, advice)); }
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<object>.Type);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                if (_type == Runtime.Void) { _body.Emit(OpCodes.Ldnull); }
                else
                {
                    _body.Emit(OpCodes.Dup);
                    if (_type.IsValueType) { _body.Emit(OpCodes.Box, _type); }
                    else if (_type != Metadata<object>.Type) { _body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                }
                _body.Emit(OpCodes.Stloc_0);
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}