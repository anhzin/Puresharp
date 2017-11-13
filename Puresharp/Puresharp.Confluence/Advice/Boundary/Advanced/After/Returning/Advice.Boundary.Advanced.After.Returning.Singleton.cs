using System;
using System.Reflection;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            static internal partial class Advanced
            {
                public partial class After
                {
                    public partial class Returning
                    {
                        public partial class Singleton : Advice.Boundary.IFactory
                        {
                            private MethodBase m_Method;
                            private ParameterInfo[] m_Signature;
                            private Action<object, object[], object> m_Action;

                            public Singleton(MethodBase method, Action<object, object[], object> action)
                            {
                                this.m_Method = method;
                                this.m_Signature = method.GetParameters();
                                this.m_Action = action;
                            }

                            public Advice.IBoundary Create()
                            {
                                return new Advice.Boundary.Advanced.After.Returning(this.m_Method, this.m_Signature, this.m_Action);
                            }
                        }
                    }
                }
            }
        }
    }
}