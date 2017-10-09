using System;

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
                    public partial class Throwing
                    {
                        public partial class Singleton : Advice.Boundary.IFactory
                        {
                            private Action<object, object[], Exception> m_Action;

                            public Singleton(Action<object, object[], Exception> action)
                            {
                                this.m_Action = action;
                            }

                            public Advice.IBoundary Create()
                            {
                                return new Advice.Boundary.Advanced.After.Throwing(this.m_Action);
                            }
                        }
                    }
                }
            }
        }
    }
}