using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Puresharp
{
    public sealed partial class Composition
    {
        internal sealed class Container<T>
            where T : class
        {
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<T[]> Empty = new Func<Func<T[]>>(() => { var _emprty = new T[0]; return new Func<T[]>(() => _emprty); })();
            internal Data.Linkup<Func<T>> Linkup = null;
            internal Func<T> Instance = Composition.Container<T>.None;
            internal Func<T[]> Array = Composition.Container<T>.Empty;
        }

        [DebuggerNonUserCode]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public class Lookup<T>
            where T : class
        {
            static internal Composition.Container<T>[] Buffer = Composition.Lookup<T>.Initialize();

            static private Composition.Container<T>[] Initialize()
            {
                lock (Composition.m_Handle)
                {
                    var _buffer = new Composition.Container<T>[Composition.m_Sequence];
                    for (var _index = 0; _index < _buffer.Length; _index++) { _buffer[_index] = new Composition.Container<T>(); }
                    Composition.m_Instantiation.Add(() =>
                    {
                        Composition.Lookup<T>.Buffer = new Composition.Container<T>[Composition.m_Sequence];
                        for (var _index = 0; _index < _buffer.Length; _index++) { Composition.Lookup<T>.Buffer[_index] = _buffer[_index]; }
                        Composition.Lookup<T>.Buffer[_buffer.Length] = new Composition.Container<T>();
                    });
                    return _buffer;
                }
            }

            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public T Instance(int index)
            {
                return Composition.Lookup<T>.Buffer[index].Instance();
            }

            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public IEnumerable<T> Enumerable(int index)
            {
                return Composition.Lookup<T>.Buffer[index].Array();
            }

            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public T[] Array(int index)
            {
                return Composition.Lookup<T>.Buffer[index].Array();
            }
        }
    }

    static public partial class Composition<X>
    {
        [DebuggerNonUserCode]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public class Lookup<T>
            where T : class
        {
            static internal ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<T[]> Empty = new Func<Func<T[]>>(() => { var _emprty = new T[0]; return new Func<T[]>(() => _emprty); })();
            static internal Data.Linkup<Func<T>> Linkup = null;
            static internal Func<T> m_Instance = Composition<X>.Lookup<T>.None;
            static internal Func<T[]> m_Array = Composition<X>.Lookup<T>.Empty;
            static internal int Index = -1;
            static internal object Handle = new object();
            static internal Type[] Type = new Type[0];
            static internal Func<object, Func<T>, Func<T[]>>[] Factory = new Func<object, Func<T>, Func<T[]>>[0];

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
                get { return Composition<X>.Lookup<T>.m_Array; }
            }

            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<T[]> Array
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return Composition<X>.Lookup<T>.m_Array; }
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
