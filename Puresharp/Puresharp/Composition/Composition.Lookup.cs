using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Puresharp
{
    public partial class Composition<X>
    {
        [DebuggerNonUserCode]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public class Lookup<T>
            where T : class
        {
            static internal object Handle = new object();
            static internal Dictionary<Func<T>, Func<T>> Map = new Dictionary<Func<T>, Func<T>>();
            static internal Dictionary<Func<T>, Func<T>> Dictionary = new Dictionary<Func<T>, Func<T>>();
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<IEnumerable<T>> Empty = new Func<Func<IEnumerable<T>>>(() => { var _emprty = new T[0]; return new Func<IEnumerable<T>>(() => _emprty); })();
            static internal Data.Linkup<Func<T>> Linkup = null;
            static internal Func<T> m_Instance = Composition<X>.Lookup<T>.None;
            static internal Func<IEnumerable<T>> m_Enumerable = Composition<X>.Lookup<T>.Empty;

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<T> Instance
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return Composition<X>.Lookup<T>.m_Instance; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<IEnumerable<T>> Enumerable
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return Composition<X>.Lookup<T>.m_Enumerable; }
            }

            /// <summary>
            /// Equals.
            /// </summary>
            /// <param name="left">Left</param>
            /// <param name="right">Right</param>
            /// <returns>Equals</returns>
            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            new static public bool ReferenceEquals(object left, object right)
            {
                return object.ReferenceEquals(left, right);
            }
        }
    }
}
