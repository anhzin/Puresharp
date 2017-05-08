using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public partial class Runtime
    {
        static public partial class Inventory
        {
            static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
            static private Data.Map<Type, FieldInfo> m_Types = new Data.Map<Type, FieldInfo>(_Type => typeof(Runtime<>).GetField(Runtime.Field(() => Runtime<object>.Type).Name), Concurrency.Interlocked);
            static private Data.Map<FieldInfo, FieldInfo> m_Fields = new Data.Map<FieldInfo, FieldInfo>(_Field => Runtime.Inventory.Define(_Field), Concurrency.Interlocked);
            static private Data.Map<PropertyInfo, FieldInfo> m_Properties = new Data.Map<PropertyInfo, FieldInfo>(_Property => Runtime.Inventory.Define(_Property), Concurrency.Interlocked);
            static private Data.Map<MethodInfo, FieldInfo> m_Methods = new Data.Map<MethodInfo, FieldInfo>(_Method => Runtime.Inventory.Define(_Method), Concurrency.Interlocked);
            static private Data.Map<ConstructorInfo, FieldInfo> m_Constructors = new Data.Map<ConstructorInfo, FieldInfo>(_Constructor => Runtime.Inventory.Define(_Constructor), Concurrency.Interlocked);
            static private Data.Map<ParameterInfo, FieldInfo> m_Parameters = new Data.Map<ParameterInfo, FieldInfo>(_Parameter => Runtime.Inventory.Define(_Parameter), Concurrency.Interlocked);

            static private FieldInfo Define<T>(T value)
            {
                var _type = Runtime.Inventory.m_Module.DefineType($"<{ Guid.NewGuid().ToString("N") }>", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Serializable);
                _type.DefineField($"<{ Runtime<T>.Type.Name }>", Runtime<T>.Type, FieldAttributes.Public | FieldAttributes.Static);
                return _type.CreateType().GetFields().Single();
            }

            static public FieldInfo Lookup(Type type)
            {
                return Runtime.Inventory.m_Types[type];
            }

            static public FieldInfo Lookup(FieldInfo field)
            {
                return Runtime.Inventory.m_Fields[field];
            }

            static public FieldInfo Lookup(PropertyInfo property)
            {
                return Runtime.Inventory.m_Properties[property];
            }

            static public FieldInfo Lookup(MethodInfo method)
            {
                return Runtime.Inventory.m_Methods[method];
            }

            static public FieldInfo Lookup(ConstructorInfo constructor)
            {
                return Runtime.Inventory.m_Constructors[constructor];
            }

            static public FieldInfo Lookup(ParameterInfo parameter)
            {
                return Runtime.Inventory.m_Parameters[parameter];
            }
        }
    }
}
