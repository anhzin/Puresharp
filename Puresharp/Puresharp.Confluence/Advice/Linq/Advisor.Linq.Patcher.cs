using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        static public partial class Style
        {
            public partial class Linq
            {
                internal sealed class Patcher : ExpressionVisitor
                {
                    static private MethodInfo Method(MethodBase method, IntPtr pointer)
                    {
                        var _type = method.ReturnType();
                        var _signature = method.Signature();
                        var _method = new DynamicMethod(string.Empty, _type, _signature, method.Module, true);
                        var _body = _method.GetILGenerator();
                        _body.Emit(_signature, false);
                        _body.Emit(pointer, _type, _signature);
                        _body.Emit(OpCodes.Ret);
                        _method.Prepare();
                        return _method;
                    }

                    static public Expression Patch(Expression expression, MethodBase method, IntPtr pointer)
                    {
                        return new Advice.Style.Linq.Patcher(method, pointer).Visit(expression);
                    }

                    private readonly MethodBase m_Method;
                    private readonly IntPtr m_Pointer;

                    private Patcher(MethodBase method, IntPtr pointer)
                    {
                        this.m_Method = method;
                        this.m_Pointer = pointer;
                    }

                    override protected Expression VisitMethodCall(MethodCallExpression node)
                    {
                        if (node.Method == this.m_Method)
                        {
                            if (this.m_Method.IsStatic) { return Expression.Call(Advice.Style.Linq.Patcher.Method(this.m_Method, this.m_Pointer), node.Arguments); }
                            return Expression.Call(Advice.Style.Linq.Patcher.Method(this.m_Method, this.m_Pointer), new Expression[] { node.Object }.Concat(node.Arguments));
                        }
                        return base.VisitMethodCall(node);
                    }
                }
            }
        }
    }
}