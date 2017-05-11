using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    static public partial class Composition
    {
        static private class Container<T>
            where T : class
        {
            static public object Handle = new object();
            static public Dictionary<Func<T>, Func<T>> Dictionary = new Dictionary<Func<T>, Func<T>>();
            static public Func<T> None = new Func<T>(() => null);
            static public Func<T> Multiple = new Func<T>(() => { throw new NotSupportedException(); });
            static public Func<IEnumerable<T>> Empty = new Func<Func<IEnumerable<T>>>(() => { var _emprty = new T[0]; return new Func<IEnumerable<T>>(() => _emprty); })();
            static public Data.Linkup<Func<T>> Linkup = null;
            static public Func<T> Instance = Composition.Container<T>.None;
            static public Func<IEnumerable<T>> Enumerable = Composition.Container<T>.Empty;
        }
    }
}
