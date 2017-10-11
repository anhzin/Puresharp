using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    static public partial class __Advice
    {
        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression of code to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Before(this Advice.Style.ILinq linq, Expression advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (advice == null) { return null; }
                if (_Boundary != null) { return _Boundary.Combine(new Advice.Boundary.Basic.Before.Singleton(Expression.Lambda<Action>(advice).Compile())); }
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
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
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked before the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable(Expression) = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Before(this Advice.Style.ILinq linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (advice == null) { return null; }
                var _signature = _Method.Signature();
                if (_Boundary != null)
                {
                    //TODO dynamic implementation will be faster by avoiding boxing/casting
                    var _instance = Expression.Parameter(Metadata<object>.Type);
                    var _arguments = Expression.Parameter(Metadata<object[]>.Type);
                    var _advice = advice(_signature.Instance == null ? null : _signature.Instance.IsValueType ? Expression.Convert(_instance, _signature.Instance) : Expression.TypeAs(_instance, _signature.Instance), new Collection<Expression>(_signature.Parameters.Select((_Parameter, _Index) => _Parameter.IsValueType ? Expression.Convert(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter) : Expression.TypeAs(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter)).ToArray()));
                    if (_advice == null) { return null; }
                    if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                    return _Boundary.Combine(new Advice.Boundary.Advanced.Before.Singleton(Expression.Lambda<Action<object, object[]>>(_advice, _instance, _arguments).Compile()));
                }
                else
                {
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                    if (_advice == null) { return null; }
                    if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                    var _type = _Method.ReturnType();
                    var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
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