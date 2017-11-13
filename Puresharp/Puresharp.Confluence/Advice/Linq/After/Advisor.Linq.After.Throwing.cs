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
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression (void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Throwing(this Advice.Style.Linq.IAfter linq, Expression advice)
        {
            return new Advice((_Method, _Pointer, _Boundary) =>
            {
                if (_Boundary != null) { return _Boundary.Combine(new Advice.Boundary.Basic.After.Throwing.Singleton(Expression.Lambda<Action>(advice).Compile())); }
                var _signature = _Method.Signature();
                var _exception = Expression.Parameter(Metadata<Exception>.Type);
                if (advice == null) { return null; }
                if (advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                var _type = _Method.ReturnType();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                var _body = _method.GetILGenerator();
                if (_type == Runtime.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
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
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
        
        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable(Expression) = [enumerable of expression of argument used to call advised method], Expression = [expression of exception]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice Throwing(this Advice.Style.Linq.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression, Expression> advice)
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
                    var _exception = Expression.Parameter(Metadata<Exception>.Type);
                    var _advice = advice(_signature.Instance == null ? null : _signature.Instance.IsValueType ? Expression.Convert(_instance, _signature.Instance) : Expression.TypeAs(_instance, _signature.Instance), new Collection<Expression>(_signature.Parameters.Select((_Parameter, _Index) => _Parameter.IsValueType ? Expression.Convert(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter) : Expression.TypeAs(Expression.ArrayIndex(_arguments, Expression.Constant(_Index, Metadata<int>.Type)), _Parameter)).ToArray()), _exception);
                    if (_advice == null) { return null; }
                    if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                    return _Boundary.Combine(new Advice.Boundary.Advanced.After.Throwing.Singleton(_Method, Expression.Lambda<Action<object, object[], Exception>>(_advice, _instance, _arguments, _exception).Compile()));
                }
                else
                {
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    var _exception = Expression.Parameter(Metadata<Exception>.Type);
                    var _advice = _signature.Instance == null ? advice(null, _parameters, _exception) : advice(_parameters[0], _parameters.Skip(1), _exception);
                    if (_advice == null) { return null; }
                    if (_advice.Type != Runtime.Void) { throw new NotSupportedException(); }
                    var _type = _Method.ReturnType();
                    var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.Module, true);
                    var _body = _method.GetILGenerator();
                    if (_type == Runtime.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(_Pointer, _type, _signature);
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Call, Expression.Lambda(_advice, new ParameterExpression[] { _exception }.Concat(_parameters)).CompileToMethod());
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
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Call, Expression.Lambda(_advice, new ParameterExpression[] { _exception }.Concat(_parameters)).CompileToMethod());
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