using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    /// <summary>
    /// CIL Instruction.
    /// </summary>
    [DebuggerDisplay("{this.ToString(), nq}")]
    internal class Instruction
    {
        /// <summary>
        /// Code.
        /// </summary>
        public readonly OpCode Code;

        /// <summary>
        /// Type.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Value.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Create an instruction without value.
        /// </summary>
        /// <param name="code"></param>
        public Instruction(OpCode code)
        {
            this.Code = code;
            this.Type = null;
            this.Value = null;
        }

        internal Instruction(OpCode code, Type type, object value)
        {
            this.Code = code;
            this.Type = type;
            this.Value = value;
        }

        virtual internal void Push(ILGenerator body)
        {
            body.Emit(this.Code);
        }

        /// <summary>
        /// Instruction illustration.
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            if (this.Type == null) {return this.Code.ToString(); }
            if (this.Value == null) { return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [NULL]"); }
            if (this.Type == Runtime<Label>.Type) { return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [", ((Label)this.Value).Value().ToString(), "]"); }
            return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [", this.Value.ToString(), "]");
        }
    }

    /// <summary>
    /// Instruction
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    internal sealed class Instruction<T> : Instruction
    {
        static private class Initialization
        {
            static public Action<ILGenerator, Instruction> Push()
            {
                if (Runtime<T>.Type == Runtime<byte>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (byte)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<sbyte>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (sbyte)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<short>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (short)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<int>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (int)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<long>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (long)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<float>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (float)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<double>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (double)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<string>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as string)); }
                if (Runtime<T>.Type == Runtime<LocalBuilder>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as LocalBuilder)); }
                if (Runtime<T>.Type == Runtime<Label>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (Label)_Instruction.Value)); }
                if (Runtime<T>.Type == Runtime<Label[]>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as Label[])); }
                if (Runtime<T>.Type == Runtime<Type>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as Type)); }
                if (Runtime<T>.Type == Runtime<FieldInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as FieldInfo)); }
                if (Runtime<T>.Type == Runtime<MethodInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as MethodInfo)); }
                if (Runtime<T>.Type == Runtime<ConstructorInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as ConstructorInfo)); }
                if (Runtime<T>.Type == Runtime<SignatureHelper>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as SignatureHelper)); }
                throw new NotSupportedException();
            }
        }

        static private readonly Action<ILGenerator, Instruction> m_Push = Initialization.Push();

        /// <summary>
        /// Value.
        /// </summary>
        new public readonly T Value;

        /// <summary>
        /// Create an instruction.
        /// </summary>
        /// <param name="code">Code</param>
        /// <param name="value">Value</param>
        public Instruction(OpCode code, T value)
            : base(code, Runtime<T>.Type, value)
        {
            this.Value = value;
        }

        override internal void Push(ILGenerator body)
        {
            Instruction<T>.m_Push(body, this);
        }
    }
}
