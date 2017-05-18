using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    /// <summary>
    /// Advice.
    /// </summary>
    public sealed partial class Advice
    {
        static internal readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();

        /// <summary>
        /// Basic.
        /// </summary>
        static public readonly Advice.Style.IBasic Basic = null;

        /// <summary>
        /// Linq.
        /// </summary>
        static public readonly Advice.Style.ILinq Linq = null;

        /// <summary>
        /// Reflection.
        /// </summary>
        static public readonly Advice.Style.IReflection Reflection = null;

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// ReferenceEquals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        private Func<MethodBase, IntPtr, Advice.Boundary.IFactory, object> m_Decorate;
        //private readonly Func<MethodBase, IntPtr, MethodInfo> m_Decorate;
        //private Func<MethodBase, Advice.Boundary.IFactory> m_Boundary;

        /// <summary>
        /// Create an advice, a way to decorate.
        /// </summary>
        /// <param name="decorate">Delegate use to decorate a method : Func(MethodBase = [base method to decorate], IntPtr = [pointer to base method]) return replacing method or boundary</param>
        internal Advice(Func<MethodBase, IntPtr, Advice.Boundary.IFactory, object> decorate)
        {
            this.m_Decorate = decorate;
        }

        ///// <summary>
        ///// Create an advice, a way to decorate.
        ///// </summary>
        ///// <param name="decorate">Delegate use to decorate a method : Func(MethodBase = [base method to decorate]) return replacing method</param>
        //internal Advice(Func<MethodBase, MethodInfo> decorate)
        //{
        //    this.m_Decorate = new Func<MethodBase, IntPtr, MethodInfo>((_Method, _Pointer) => decorate(_Method));
        //}

        ///// <summary>
        ///// Create an advice with a specific replacing method.
        ///// </summary>
        ///// <param name="method">Replacing method</param>
        //internal Advice(MethodInfo method)
        //{
        //    this.m_Decorate = new Func<MethodBase, IntPtr, MethodInfo>((_Method, _Pointer) => method);
        //}

        //internal Advice(Func<MethodBase, Advice.Boundary.IFactory> boundary)
        //{
        //    this.m_Boundary = boundary;
        //}

        /// <summary>
        /// Decorate a method for a specific concern.
        /// </summary>
        /// <param name="method">Base method to decorate</param>
        /// <param name="pointer">Pointer to base method</param>
        /// <returns>Replacing method</returns>
        internal MethodInfo Decorate(MethodBase method, IntPtr pointer)
        {
            return this.m_Decorate(method, pointer, null) as MethodInfo;
        }

        internal Advice.Boundary.IFactory Decorate(MethodBase method, Advice.Boundary.IFactory boundary)
        {
            return this.m_Decorate(method, IntPtr.Zero, boundary) as Advice.Boundary.IFactory;
            //    if (boundary is Advice.Boundary.Sequence.Factory)
            //    {
            //        var _sequence = (boundary as Advice.Boundary.Sequence.Factory).Sequence;
            //        var _array = new Advice.Boundary.IFactory[_sequence.Length + 1];
            //        for (var _index = 0; _index < _array.Length; _index++) { _array[_index] = _sequence[_index]; }
            //        _array[_sequence.Length] = this.m_Decorate(method, IntPtr.Zero, boundary) as Advice.Boundary.IFactory;
            //        return new Advice.Boundary.Sequence.Factory(_array);
            //    }
            //    return new Advice.Boundary.Sequence.Factory(new Advice.Boundary.IFactory[] { boundary, this.m_Decorate(method, IntPtr.Zero, boundary) as Advice.Boundary.IFactory });
        }
    }

    /// <summary>
    /// Extension for advising.
    /// </summary>
    static public partial class __Advice
    {
    }
}