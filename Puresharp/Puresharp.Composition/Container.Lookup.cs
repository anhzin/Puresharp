using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Puresharp.Composition
{
    public sealed partial class Container
    {
        /// <summary>
        /// Lokup
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        [DebuggerNonUserCode]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public class Lookup<T>
            where T : class
        {
            static internal Container.Container1<T>[] Buffer = Container.Lookup<T>.Initialize();

            static private Container.Container1<T>[] Initialize()
            {
                lock (Container.m_Handle)
                {
                    var _buffer = new Container.Container1<T>[Container.m_Sequence];
                    for (var _index = 0; _index < _buffer.Length; _index++) { _buffer[_index] = new Container.Container1<T>(); }
                    Container.m_Instantiation.Add(() =>
                    {
                        Container.Lookup<T>.Buffer = new Container.Container1<T>[Container.m_Sequence];
                        for (var _index = 0; _index < _buffer.Length; _index++) { Container.Lookup<T>.Buffer[_index] = _buffer[_index]; }
                        Container.Lookup<T>.Buffer[_buffer.Length] = new Container.Container1<T>();
                    });
                    return _buffer;
                }
            }

            /// <summary>
            /// Obtain a single instance.
            /// </summary>
            /// <param name="index">Index</param>
            /// <returns>Instance</returns>
            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            static public T Instance(int index)
            {
                return Container.Lookup<T>.Buffer[index].Instance();
            }

            /// <summary>
            /// Obtain enumerable of all instances registered.
            /// </summary>
            /// <param name="index">Index</param>
            /// <returns>Enumerable</returns>
            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            static public IEnumerable<T> Enumerable(int index)
            {
                return Container.Lookup<T>.Buffer[index].Array();
            }

            /// <summary>
            /// Obtain array of all instances registered.
            /// </summary>
            /// <param name="index">Index</param>
            /// <returns>Array</returns>
            [DebuggerNonUserCode]
            [DebuggerStepThrough]
            [DebuggerHidden]
            static public T[] Array(int index)
            {
                return Container.Lookup<T>.Buffer[index].Array();
            }

            /// <summary>
            /// Determines whether the specified object instances are considered equal.
            /// </summary>
            /// <param name="left">left</param>
            /// <param name="right">right</param>
            /// <returns>Boolean</returns>
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            new static public bool Equals(object left, object right)
            {
                return object.Equals(left, right);
            }

            /// <summary>
            /// Determines whether the specified object instances are the same instance.
            /// </summary>
            /// <param name="left">left</param>
            /// <param name="right">right</param>
            /// <returns>Boolean</returns>
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            new static public bool ReferenceEquals(object left, object right)
            {
                return object.ReferenceEquals(left, right);
            }
        }
    }

    static public partial class Container<T>
    {
        /// <summary>
        /// Lookup
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        [DebuggerNonUserCode]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public class Lookup<T>
            where T : class
        {
            static internal ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(string.Concat(Metadata<Assembly>.Type.Name, Guid.NewGuid().ToString("N"))), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, Guid.NewGuid().ToString("N")), false);
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<T[]> Empty = new Func<Func<T[]>>(() => { var _emprty = new T[0]; return new Func<T[]>(() => _emprty); })();
            static internal Linkup<Func<T>> Linkup = null;
            static internal Func<T> m_Instance = Lookup<T>.None;
            static internal Func<T[]> m_Array = Lookup<T>.Empty;
            static internal int Index = -1;
            static internal object Handle = new object();
            static internal KeyValuePair<Type, FieldBuilder[]>[] Type = new KeyValuePair<Type, FieldBuilder[]>[0];

            /// <summary>
            /// Instance.
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<T> Instance
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                get { return Lookup<T>.m_Instance; }
            }

            /// <summary>
            /// Enumerable.
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<IEnumerable<T>> Enumerable
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                get { return Lookup<T>.m_Array; }
            }

            /// <summary>
            /// Array.
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            static public Func<T[]> Array
            {
                [DebuggerNonUserCode]
                [DebuggerStepThrough]
                [DebuggerHidden]
                get { return Lookup<T>.m_Array; }
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
            new static public bool ReferenceEquals(object left, object right)
            {
                return object.ReferenceEquals(left, right);
            }
        }
    }
}
