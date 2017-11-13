using System;
using System.Reflection;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            static internal partial class Basic
            {
                public partial class After
                {
                    public partial class Throwing
                    {
                        public partial class Volatile : Advice.Boundary.IFactory
                        {
                            private Func<Action> m_Action;

                            public Volatile(Func<Action> action)
                            {
                                this.m_Action = action;
                            }

                            public Advice.IBoundary Create()
                            {
                                return new Advice.Boundary.Basic.After.Throwing(this.m_Action());
                            }
                        }
                    }
                }
            }
        }
    }
}