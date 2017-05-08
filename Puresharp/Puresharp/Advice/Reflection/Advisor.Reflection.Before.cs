using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
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
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                _body.Emit(advice);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}