using System;

namespace Puresharp.Composition
{
    public sealed partial class Container
    {
        private class Store<T>
            where T : class
        {
            static internal Func<T> None = new Func<T>(() => null);
            static internal Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static internal Func<T[]> Empty = new Func<Func<T[]>>(() => { var _emprty = new T[0]; return new Func<T[]>(() => _emprty); })();
            internal Linkup<Func<T>> Linkup = null;
            internal Func<T> Instance = Container.Store<T>.None;
            internal Func<T[]> Array = Container.Store<T>.Empty;
        }
    }
}
