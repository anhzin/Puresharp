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
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression (void) of code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Returning(this Advice.Style.Linq.IAfter linq, Expression advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return new Advice.Boundary.Sequence.Factory(_Boundary, new Advice.Boundary.Basic.After.Returning.Singleton(Expression.Lambda<Action>(advice).Compile())); }
                var _signature = _Method.Signature();
                if (advice == null) { return null; }
                if (advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable(Expression) = [enumerable of expression of argument used to call advised method], Expression = [expression of return value (null if return type is void)]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Returning(this Advice.Style.Linq.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression, Expression> advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (advice == null) { return null; }
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType();
                if (_Boundary != null)
                {
                    //TODO dynamic implementation will be faster by avoiding boxing/casting
                    var _instance = Expression.Parameter(Metadata<object>.Type);
                    var _arguments = Expression.Parameter(Metadata<object[]>.Type);
                    var _return = Expression.Parameter(Metadata<object>.Type);
                    var _advice = advice(_signature.Instance == null ? null : _signature.Instance.IsValueType ? Expression.Convert(_instance, _signature.Instance) : Expression.TypeAs(_instance, _signature.Instance), new Collection<Expression>(_signature.Parameters.Select((_Parameter, _Index) => _Parameter.IsValueType ? Expression.Convert(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter) : Expression.TypeAs(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter)).ToArray()), _type== Runtime.Void ? null : _type.IsValueType ? Expression.Convert(_return, _Method.ReturnType()) : Expression.TypeAs(_return, _type));
                    if (_advice == null) { return null; }
                    if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                    return new Advice.Boundary.Sequence.Factory(_Boundary, new Advice.Boundary.Advanced.After.Returning.Singleton(Expression.Lambda<Action<object, object[], object>>(_advice, _instance, _arguments, _return).Compile()));
                }
                else
                {
                    var _parameters = _signature.Select(_Type => Expression.Parameter(_Type)).ToArray();
                    if (_type == Runtime.Void)
                    {
                        var _advice = _signature.Instance == null ? advice(null, _parameters, null) : advice(_parameters[0], _parameters.Skip(1), null);
                        if (_advice == null) { return null; }
                        if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                        var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                        var _body = _method.GetILGenerator();
                        _body.Emit(_signature, false);
                        _body.Emit(_Pointer, _type, _signature);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                        _body.Emit(OpCodes.Ret);
                        _method.Prepare();
                        return _method;
                    }
                    else
                    {
                        var _return = Expression.Parameter(_type);
                        var _advice = _signature.Instance == null ? advice(null, _parameters, _return) : advice(_parameters[0], _parameters.Skip(1), _return);
                        if (_advice == null) { return null; }
                        if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                        var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                        var _body = _method.GetILGenerator();
                        _body.Emit(_signature, false);
                        _body.Emit(_Pointer, _type, _signature);
                        _body.Emit(OpCodes.Dup);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Call, Expression.Lambda(_advice, new ParameterExpression[] { _return }.Concat(_parameters)).CompileToMethod());
                        _body.Emit(OpCodes.Ret);
                        _method.Prepare();
                        return _method;
                    }
                }
            });
        }
    }
}