using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    static public partial class Composition
    {
        static public void Add(IContainer container)
        {
        }

        static public void Remove(IContainer container)
        {
        }

        static public T Instance<T>()
            where T : class
        {
            throw new NotImplementedException();
        }

        static public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}
