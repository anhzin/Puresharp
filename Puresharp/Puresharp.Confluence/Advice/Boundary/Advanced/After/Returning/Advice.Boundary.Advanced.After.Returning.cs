using System;
using System.Linq.Expressions;
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
                    public partial class Returning : Advice.IBoundary
                    {
                        private object m_Instance;
                        private object[] m_Arguments;
                        private Action<object, object[], object> m_Action;

                        public Returning(Action<object, object[], object> action)
                        {
                            this.m_Action = action;
                        }

                        void Advice.IBoundary.Method(MethodBase method, ParameterInfo[] signature)
                        {
                            this.m_Arguments = new object[signature.Length];
                        }

                        void Advice.IBoundary.Instance<T>(T instance)
                        {
                            this.m_Instance = instance;
                        }

                        void Advice.IBoundary.Argument<T>(ParameterInfo parameter, ref T value)
                        {
                            this.m_Arguments[parameter.Position] = value;
                        }

                        void Advice.IBoundary.Begin()
                        {
                        }

                        void Advice.IBoundary.Continue()
                        {
                        }

                        void Advice.IBoundary.Yield()
                        {
                        }

                        void Advice.IBoundary.Return()
                        {
                            this.m_Action(this.m_Instance, this.m_Arguments, null);
                        }

                        void Advice.IBoundary.Throw(ref Exception exception)
                        {
                        }

                        void Advice.IBoundary.Return<T>(ref T value)
                        {
                            this.m_Action(this.m_Instance, this.m_Arguments, value);
                        }

                        void Advice.IBoundary.Throw<T>(ref Exception exception, ref T value)
                        {
                        }

                        void IDisposable.Dispose()
                        {
                        }
                    }
                }
            }
        }
    }
}