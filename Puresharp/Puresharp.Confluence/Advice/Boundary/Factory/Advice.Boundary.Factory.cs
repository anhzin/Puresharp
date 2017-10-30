using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            public partial class Factory : Advice.Boundary.IFactory
            {
                static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
                static private Advice.Boundary m_Boundary = new Advice.Boundary();

                public IBoundary Create()
                {
                    return Advice.Boundary.Factory.m_Boundary;
                }
            }

            public class Factory<T> : Advice.Boundary.IFactory
                where T : Advice.IBoundary, new()
            {
                public IBoundary Create()
                {
                    return new T();
                }
            }
        }
    }
}