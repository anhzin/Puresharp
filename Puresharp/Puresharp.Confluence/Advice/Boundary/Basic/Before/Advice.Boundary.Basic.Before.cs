using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            static internal partial class Basic
            {
                public partial class Before : Advice.IBoundary
                {
                    private Action m_Action;

                    public Before(Action action)
                    {
                        this.m_Action = action;
                    }

                    void Advice.IBoundary.Method(MethodBase method, ParameterInfo[] signature)
                    {
                    }

                    void Advice.IBoundary.Instance<T>(T instance)
                    {
                    }

                    void Advice.IBoundary.Argument<T>(ParameterInfo parameter, ref T value)
                    {
                    }

                    void Advice.IBoundary.Begin()
                    {
                        this.m_Action();
                    }

                    void Advice.IBoundary.Continue()
                    {
                    }

                    void Advice.IBoundary.Yield()
                    {
                    }

                    void Advice.IBoundary.Return()
                    {
                    }

                    void Advice.IBoundary.Throw(ref Exception exception)
                    {
                    }

                    void Advice.IBoundary.Return<T>(ref T value)
                    {
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