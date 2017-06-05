using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp.Reflection
{
    /// <summary>
    /// Metadata.
    /// </summary>
    static public partial class Metadata
    {
        /// <summary>
        /// Void.
        /// </summary>
        static public readonly Type Void = typeof(void);

        /// <summary>
        /// Obtain constructor from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Constructor</returns>
        static public ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as NewExpression).Constructor;
        }
        
        /// <summary>
        /// Obtain static field from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain static property from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>PropertyInfo</returns>
        static public PropertyInfo Property<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }
        
        /// <summary>
        /// Obtain static method from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain static method from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }

    /// <summary>
    /// Metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public partial class Metadata<T>
    {
        static private Type m_Type = typeof(T);

        /// <summary>
        /// Type.
        /// </summary>
        static public Type Type
        {
            get { return Metadata<T>.m_Type; }
        }

        /// <summary>
        /// Obtain field from linq expression.
        /// </summary>
        /// <typeparam name="TValue">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<TValue>(Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain property from linq expression.
        /// </summary>
        /// <typeparam name="TValue">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Property</returns>
        static public PropertyInfo Property<TValue>(Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }
        
        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <typeparam name="TReturn">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }
}
