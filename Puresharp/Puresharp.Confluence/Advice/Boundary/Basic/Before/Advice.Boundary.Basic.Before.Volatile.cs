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
                public partial class Before
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
                            return new Advice.Boundary.Basic.Before(this.m_Action());
                        }
                    }
                }
            }
        }
    }
}