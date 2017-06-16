using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp.Composition
{
    static internal partial class Metadata
    {
        static public FieldInfo Field<T>(System.Linq.Expressions.Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        static public MethodInfo Method<T>(System.Linq.Expressions.Expression<Func<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        static public bool HasDefaultValue(this ParameterInfo parameter)
        {
            return false;
        }
    }

    static internal partial class Metadata<T>
    {
        static private Type m_Type = typeof(T);

        static public Type Type
        {
            get { return Metadata<T>.m_Type; }
        }

        static public FieldInfo Field<TValue>(System.Linq.Expressions.Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        static public MethodInfo Method<TReturn>(System.Linq.Expressions.Expression<Func<T, TReturn>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }
}
