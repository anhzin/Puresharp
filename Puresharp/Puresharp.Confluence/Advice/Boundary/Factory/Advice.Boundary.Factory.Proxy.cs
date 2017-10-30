using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            public partial class Factory
            {
                abstract internal class Proxy<T>
                    where T : class
                {
                    static private Type m_Type;

                    static Proxy()
                    {
                        var _type = Advice.Boundary.Factory.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class, Metadata<object>.Type, new Type[] { Metadata<T>.Type });
                        Advice.Boundary.Factory.Proxy<T>.m_Type = _type.CreateType();
                    }

                    private Advice.Boundary.IFactory m_Factory;
                    private Func<T> m_Creation;

                    protected Proxy(Advice.Boundary.IFactory factory)
                    {
                        this.m_Factory = factory;
                        this.m_Creation = Expression.Lambda<Func<T>>(Expression.New(Advice.Boundary.Factory.Proxy<T>.m_Type.GetConstructors().Single(), null)).Compile();
                    }

                    public Func<T> Creation
                    {
                        get { return this.m_Creation; }
                    }

                    public Advice.Boundary.IFactory Factory
                    {
                        get { return this.m_Factory; }
                    }
                }
            }
        }
    }
}