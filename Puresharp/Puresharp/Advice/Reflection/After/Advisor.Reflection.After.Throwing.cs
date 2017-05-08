﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
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
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.ReturnType();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (_type == Runtime.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginCatchBlock(Runtime<Exception>.Type);
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
                    _body.BeginCatchBlock(Runtime<Exception>.Type);
                    _body.Emit(advice);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}