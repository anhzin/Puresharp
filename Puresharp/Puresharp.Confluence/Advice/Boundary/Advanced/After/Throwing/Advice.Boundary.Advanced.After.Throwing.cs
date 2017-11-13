﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
                    public partial class Throwing : Advice.IBoundary
                    {
                        private object m_Instance;
                        private object[] m_Arguments;
                        private Action<object, object[], Exception> m_Action;

                        public Throwing(MethodBase method, ParameterInfo[] signature, Action<object, object[], Exception> action)
                        {
                            this.m_Arguments = new object[signature.Length];
                            this.m_Action = action;
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

                        void Advice.IBoundary.Await(MethodInfo method, ref Task task)
                        {
                        }

                        void Advice.IBoundary.Await<T>(MethodInfo method, ref Task<T> task)
                        {
                        }

                        void Advice.IBoundary.Continue()
                        {
                        }

                        void Advice.IBoundary.Return()
                        {
                        }

                        void Advice.IBoundary.Throw(ref Exception exception)
                        {
                            this.m_Action(this.m_Instance, this.m_Arguments, exception);
                        }

                        void Advice.IBoundary.Return<T>(ref T value)
                        {
                        }

                        void Advice.IBoundary.Throw<T>(ref Exception exception, ref T value)
                        {
                            this.m_Action(this.m_Instance, this.m_Arguments, exception);
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