using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp.Composition
{
    public sealed partial class Container
    {
        private class Store<T>
            where T : class
        {
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<IEnumerable<T>> Empty = System.Linq.Enumerable.Empty<T>;
            internal Linkup<Func<T>> Linkup = null;
            internal Func<T> Instance = Container.Store<T>.None;
            internal Func<IEnumerable<T>> Enumerable = Container.Store<T>.Empty;
        }
    }
}
