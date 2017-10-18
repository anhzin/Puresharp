using System;
using Mono;
using Mono.Cecil;

namespace Puresharp
{
    internal class Lifecycle
    {
        private TypeDefinition m_Type;
        private MethodDefinition m_Method;
        private MethodDefinition m_Instance;
        private MethodDefinition m_Argument;
        private MethodDefinition m_Begin;
        private MethodDefinition m_Continue;
        private MethodDefinition m_Await;
        private Feedback m_Void;
        private Feedback m_Value;

        public Lifecycle(TypeDefinition type, MethodDefinition method, MethodDefinition instance, MethodDefinition argument, MethodDefinition begin, MethodDefinition @continue, MethodDefinition await, Feedback @void, Feedback value)
        {
            this.m_Type = type;
            this.m_Method = method;
            this.m_Instance = instance;
            this.m_Argument = argument;
            this.m_Begin = begin;
            this.m_Continue = @continue;
            this.m_Await = await;
            this.m_Void = @void;
            this.m_Value = value;
        }

        public TypeDefinition Type { get { return this.m_Type; } }
        public MethodDefinition Method { get { return this.m_Method; } }
        public MethodDefinition Instance { get { return this.m_Instance; } }
        public MethodDefinition Argument { get { return this.m_Argument; } }
        public MethodDefinition Begin { get { return this.m_Begin; } }
        public MethodDefinition Continue { get { return this.m_Continue; } }
        public MethodDefinition Await { get { return this.m_Await; } }
        public Feedback Void { get { return this.m_Void; } }
        public Feedback Value { get { return this.m_Value; } }

        public class Feedback
        {
            private MethodDefinition m_Return;
            private MethodDefinition m_Throw;

            public Feedback(MethodDefinition @return, MethodDefinition @throw)
            {
                this.m_Return = @return;
                this.m_Throw = @throw;
            }

            public MethodDefinition Return { get { return this.m_Return; } }
            public MethodDefinition Throw { get { return this.m_Throw; } }
        }
    }
}
