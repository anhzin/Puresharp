using System;

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
                        public partial class Singleton : Advice.Boundary.IFactory
                        {
                            private Action m_Action;

                            public Singleton(Action action)
                            {
                                this.m_Action = action;
                            }

                            public Advice.IBoundary Create()
                            {
                                return new Advice.Boundary.Basic.After.Throwing(this.m_Action);
                            }
                        }
                    }
                }
            }
        }
    }
}