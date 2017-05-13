using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    static public partial class Data
    {
        internal class Memory : Data.IMemory
        {
            private Dictionary<Type, object> m_Dictionary = new Dictionary<Type, object>();

            public bool Defined<T>()
            {
                return this.m_Dictionary.ContainsKey(Runtime<T>.Type);
            }

            public void Define<T>(T value)
            {
                this.m_Dictionary.Add(Runtime<T>.Type, value);
            }

            public T Value<T>()
            {
                return (T)this.m_Dictionary[Runtime<T>.Type];
            }

            public void Dispose()
            {
            }
        }
    }
}
