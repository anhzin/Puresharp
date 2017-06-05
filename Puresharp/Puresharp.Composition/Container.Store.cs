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
        internal sealed class Container1<T>
            where T : class
        {
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<T[]> Empty = new Func<Func<T[]>>(() => { var _emprty = new T[0]; return new Func<T[]>(() => _emprty); })();
            internal Linkup<Func<T>> Linkup = null;
            internal Func<T> Instance = Container.Container1<T>.None;
            internal Func<T[]> Array = Container.Container1<T>.Empty;
        }
    }
}
