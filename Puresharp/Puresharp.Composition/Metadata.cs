using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp.Composition
{
    static internal partial class Metadata
    {
        static private Type m_Void = typeof(void);

        static public Type Void
        {
            get { return Metadata.m_Void; }
        }

        static public ConstructorInfo Constructor<T>(System.Linq.Expressions.Expression<Func<T>> expression)
        {
            return (expression.Body as NewExpression).Constructor;
        }

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

        static public PropertyInfo Property<TValue>(System.Linq.Expressions.Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        static public MethodInfo Method(System.Linq.Expressions.Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        static public MethodInfo Method<TReturn>(System.Linq.Expressions.Expression<Func<T, TReturn>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }
}
