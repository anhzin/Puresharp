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
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Before(this Advice.Style.IBasic basic, Action advice)
        {
            //TODO : test if asynchronous! if true, emit Boundary inheritor => if advice.Target == null && method is public => call directly method, other else, stora delegate as field and call it in before method.
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return new Advice.Boundary.Sequence.Factory(_Boundary, new Advice.Boundary.Basic.Before.Singleton(advice)); }
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice.Module.DefineField(advice.Target)); }
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public Advice Before(this Advice.Style.IBasic basic, Action<object, object[]> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return new Advice.Boundary.Sequence.Factory(_Boundary, new Advice.Boundary.Advanced.Before.Singleton(advice)); }
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}