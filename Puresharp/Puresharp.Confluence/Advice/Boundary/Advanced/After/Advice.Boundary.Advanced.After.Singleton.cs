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
                    public partial class Singleton : Advice.Boundary.IFactory
                    {
                        private Action<object, object[]> m_Action;

                        public Singleton(Action<object, object[]> action)
                        {
                            this.m_Action = action;
                        }
    
                        public Advice.IBoundary Create()
                        {
                            return new Advice.Boundary.Advanced.After(this.m_Action);
                        }
                    }
                }
            }
        }
    }
}