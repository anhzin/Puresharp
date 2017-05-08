using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp
{
    /// <summary>
    /// Runtime.
    /// </summary>
    static public partial class Runtime
    {
        /// <summary>
        /// Void.
        /// </summary>
        static public readonly Type Void = typeof(void);
        
        static private IEnumerable<Type> Lineage(this Type type)
        {
            var _type = type;
            while (_type != null)
            {
                yield return _type;
                _type = _type.BaseType;
            }
        }

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

        static public Data.Collection<ConstructorInfo> Constructors(this Type type)
        {
            return Runtime.Dictionary.Constructors[type];
        }

        static public Data.Collection<ConstructorInfo> Constructors(this Type type, MethodAttributes attributes)
        {
            return new Data.Collection<ConstructorInfo>(Runtime.Dictionary.Constructors[type].Where(_Constructor => _Constructor.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<ConstructorInfo> Constructors(this Type type, Func<ConstructorInfo, bool> predicate)
        {
            return new Data.Collection<ConstructorInfo>(Runtime.Dictionary.Constructors[type].Where(predicate).ToArray());
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

        static public Data.Collection<FieldInfo> Fields(this Type type)
        {
            return Runtime.Dictionary.Fields[type];
        }

        static public Data.Collection<FieldInfo> Fields(this Type type, FieldAttributes attributes)
        {
            return new Data.Collection<FieldInfo>(Runtime.Dictionary.Fields[type].Where(_Field => _Field.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(this Type type, Func<FieldInfo, bool> predicate)
        {
            return new Data.Collection<FieldInfo>(Runtime.Dictionary.Fields[type].Where(predicate).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(this Type type, bool inherited)
        {
            return inherited ? Runtime.Dictionary.Inherited.Fields[type] : Runtime.Dictionary.Fields[type];
        }

        static public Data.Collection<FieldInfo> Fields(this Type type, bool inherited, FieldAttributes attributes)
        {
            return new Data.Collection<FieldInfo>((inherited ? Runtime.Dictionary.Inherited.Fields[type] : Runtime.Dictionary.Fields[type]).Where(_Field => _Field.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(this Type type, bool inherited, Func<FieldInfo, bool> predicate)
        {
            return new Data.Collection<FieldInfo>((inherited ? Runtime.Dictionary.Inherited.Fields[type] : Runtime.Dictionary.Fields[type]).Where(predicate).ToArray());
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

        static public Data.Collection<PropertyInfo> Properties(this Type type)
        {
            return Runtime.Dictionary.Properties[type];
        }

        static public Data.Collection<PropertyInfo> Properties(this Type type, PropertyAttributes attributes)
        {
            return new Data.Collection<PropertyInfo>(Runtime.Dictionary.Properties[type].Where(_Property => _Property.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(this Type type, Func<PropertyInfo, bool> predicate)
        {
            return new Data.Collection<PropertyInfo>(Runtime.Dictionary.Properties[type].Where(predicate).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(this Type type, bool inherited)
        {
            return inherited ? Runtime.Dictionary.Inherited.Properties[type] : Runtime.Dictionary.Properties[type];
        }

        static public Data.Collection<PropertyInfo> Properties(this Type type, bool inherited, PropertyAttributes attributes)
        {
            return new Data.Collection<PropertyInfo>((inherited ? Runtime.Dictionary.Inherited.Properties[type] : Runtime.Dictionary.Properties[type]).Where(_Property => _Property.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(this Type type, bool inherited, Func<PropertyInfo, bool> predicate)
        {
            return new Data.Collection<PropertyInfo>((inherited ? Runtime.Dictionary.Inherited.Properties[type] : Runtime.Dictionary.Properties[type]).Where(predicate).ToArray());
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

        static public Data.Collection<MethodInfo> Methods(this Type type)
        {
            return Runtime.Dictionary.Methods[type];
        }

        static public Data.Collection<MethodInfo> Methods(this Type type, MethodAttributes attributes)
        {
            return new Data.Collection<MethodInfo>(Runtime.Dictionary.Methods[type].Where(_Method => _Method.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(this Type type, Func<MethodInfo, bool> predicate)
        {
            return new Data.Collection<MethodInfo>(Runtime.Dictionary.Methods[type].Where(predicate).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(this Type type, bool inherited)
        {
            return inherited ? Runtime.Dictionary.Inherited.Methods[type] : Runtime.Dictionary.Methods[type];
        }

        static public Data.Collection<MethodInfo> Methods(this Type type, bool inherited, MethodAttributes attributes)
        {
            return new Data.Collection<MethodInfo>((inherited ? Runtime.Dictionary.Inherited.Methods[type] : Runtime.Dictionary.Methods[type]).Where(_Method => _Method.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(this Type type, bool inherited, Func<MethodInfo, bool> predicate)
        {
            return new Data.Collection<MethodInfo>((inherited ? Runtime.Dictionary.Inherited.Methods[type] : Runtime.Dictionary.Methods[type]).Where(predicate).ToArray());
        }
    }

    /// <summary>
    /// Runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public partial class Runtime<T>
    {
        /// <summary>
        /// Type.
        /// </summary>
        static public readonly Type Type = typeof(T);

        /// <summary>
        /// Value.
        /// </summary>
        static public T Value;

        static public Data.Collection<ConstructorInfo> Constructors()
        {
            return Runtime<T>.Dictionary.Constructors;
        }

        static public Data.Collection<ConstructorInfo> Constructors(MethodAttributes attributes)
        {
            return new Data.Collection<ConstructorInfo>(Runtime<T>.Dictionary.Constructors.Where(_Constructor => _Constructor.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<ConstructorInfo> Constructors(Func<ConstructorInfo, bool> predicate)
        {
            return new Data.Collection<ConstructorInfo>(Runtime<T>.Dictionary.Constructors.Where(predicate).ToArray());
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

        static public Data.Collection<FieldInfo> Fields()
        {
            return Runtime<T>.Dictionary.Fields;
        }

        static public Data.Collection<FieldInfo> Fields(FieldAttributes attributes)
        {
            return new Data.Collection<FieldInfo>(Runtime<T>.Dictionary.Fields.Where(_Field => _Field.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(Func<FieldInfo, bool> predicate)
        {
            return new Data.Collection<FieldInfo>(Runtime<T>.Dictionary.Fields.Where(predicate).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(bool inherited)
        {
            return inherited ? Runtime<T>.Dictionary.Inherited.Fields : Runtime<T>.Dictionary.Fields;
        }

        static public Data.Collection<FieldInfo> Fields(bool inherited, FieldAttributes attributes)
        {
            return new Data.Collection<FieldInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Fields : Runtime<T>.Dictionary.Fields).Where(_Field => _Field.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<FieldInfo> Fields(bool inherited, Func<FieldInfo, bool> predicate)
        {
            return new Data.Collection<FieldInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Fields : Runtime<T>.Dictionary.Fields).Where(predicate).ToArray());
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

        static public Data.Collection<PropertyInfo> Properties()
        {
            return Runtime<T>.Dictionary.Properties;
        }

        static public Data.Collection<PropertyInfo> Properties(PropertyAttributes attributes)
        {
            return new Data.Collection<PropertyInfo>(Runtime<T>.Dictionary.Properties.Where(_Property => _Property.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(Func<PropertyInfo, bool> predicate)
        {
            return new Data.Collection<PropertyInfo>(Runtime<T>.Dictionary.Properties.Where(predicate).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(bool inherited)
        {
            return inherited ? Runtime<T>.Dictionary.Inherited.Properties : Runtime<T>.Dictionary.Properties;
        }

        static public Data.Collection<PropertyInfo> Properties(bool inherited, PropertyAttributes attributes)
        {
            return new Data.Collection<PropertyInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Properties : Runtime<T>.Dictionary.Properties).Where(_Property => _Property.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<PropertyInfo> Properties(bool inherited, Func<PropertyInfo, bool> predicate)
        {
            return new Data.Collection<PropertyInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Properties : Runtime<T>.Dictionary.Properties).Where(predicate).ToArray());
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

        static public Data.Collection<MethodInfo> Methods()
        {
            return Runtime<T>.Dictionary.Methods;
        }

        static public Data.Collection<MethodInfo> Methods(MethodAttributes attributes)
        {
            return new Data.Collection<MethodInfo>(Runtime<T>.Dictionary.Methods.Where(_Method => _Method.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(Func<MethodInfo, bool> predicate)
        {
            return new Data.Collection<MethodInfo>(Runtime<T>.Dictionary.Methods.Where(predicate).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(bool inherited)
        {
            return inherited ? Runtime<T>.Dictionary.Inherited.Methods : Runtime<T>.Dictionary.Methods;
        }

        static public Data.Collection<MethodInfo> Methods(bool inherited, MethodAttributes attributes)
        {
            return new Data.Collection<MethodInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Methods : Runtime<T>.Dictionary.Methods).Where(_Method => _Method.Attributes.HasFlag(attributes)).ToArray());
        }

        static public Data.Collection<MethodInfo> Methods(bool inherited, Func<MethodInfo, bool> predicate)
        {
            return new Data.Collection<MethodInfo>((inherited ? Runtime<T>.Dictionary.Inherited.Methods : Runtime<T>.Dictionary.Methods).Where(predicate).ToArray());
        }
    }
}
