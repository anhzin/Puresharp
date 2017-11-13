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
                    public partial class Volatile : Advice.Boundary.IFactory
                    {
                        private MethodBase m_Method;
                        private ParameterInfo[] m_Signature;
                        private Func<Action<object, object[]>> m_Action;

                        public Volatile(MethodBase method, Func<Action<object, object[]>> action)
                        {
                            this.m_Method = method;
                            this.m_Signature = method.GetParameters();
                            this.m_Action = action;
                        }
    
                        public Advice.IBoundary Create()
                        {
                            return new Advice.Boundary.Advanced.After(this.m_Method, this.m_Signature, this.m_Action());
                        }
                    }
                }
            }
        }
    }
}