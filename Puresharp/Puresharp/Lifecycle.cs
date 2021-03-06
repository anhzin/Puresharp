﻿//using System;
//using Mono;
//using Mono.Cecil;

//namespace Puresharp
//{
//    internal class Boundary
//    {
//        private TypeReference m_Type;
//        private MethodReference m_Instance;
//        private MethodReference m_Argument;
//        private MethodReference m_Begin;
//        private MethodReference m_Continue;
//        private MethodReference m_Await;
//        private Feedback m_Void;
//        private Feedback m_Value;

//        public Boundary(TypeReference type, MethodReference instance, MethodReference argument, MethodReference begin, MethodReference await, MethodReference @continue, Feedback @void, Feedback value)
//        {
//            this.m_Type = type;
//            this.m_Instance = instance;
//            this.m_Argument = argument;
//            this.m_Begin = begin;
//            this.m_Await = await;
//            this.m_Continue = @continue;
//            this.m_Void = @void;
//            this.m_Value = value;
//        }

//        public TypeReference Type { get { return this.m_Type; } }
//        public MethodReference Instance { get { return this.m_Instance; } }
//        public MethodReference Argument { get { return this.m_Argument; } }
//        public MethodReference Begin { get { return this.m_Begin; } }
//        public MethodReference Await { get { return this.m_Await; } }
//        public MethodReference Continue { get { return this.m_Continue; } }
//        public Feedback Void { get { return this.m_Void; } }
//        public Feedback Value { get { return this.m_Value; } }

//        public class Feedback
//        {
//            private MethodReference m_Return;
//            private MethodReference m_Throw;

//            public Feedback(MethodReference @return, MethodReference @throw)
//            {
//                this.m_Return = @return;
//                this.m_Throw = @throw;
//            }

//            public MethodReference Return { get { return this.m_Return; } }
//            public MethodReference Throw { get { return this.m_Throw; } }
//        }
//    }
//}
