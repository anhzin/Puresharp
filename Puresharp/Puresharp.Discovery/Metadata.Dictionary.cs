using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Puresharp.Discovery
{
    static public partial class Metadata
    {
        public class Dictionary<T> : IEnumerable<T>
            where T : class
        {
            private object m_Handle;
            private LinkedList<T> m_Archive;
            private Listener<T> m_Discovered;

            internal Dictionary()
            {
                this.m_Handle = new object();
                this.m_Archive = new LinkedList<T>();
                this.m_Discovered += new Listener<T>(_Metadada => { });
            }

            public event Listener<T> Discovered
            {
                add
                {
                    lock (this.m_Handle)
                    {
                        this.m_Discovered += value;
                        foreach (var _method in this.m_Archive) { value(_method); }
                    }
                }
                remove
                {
                    lock (this.m_Handle)
                    {
                        this.m_Discovered -= value;
                    }
                }
            }

            internal void Add(T metadata)
            {
                lock (this.m_Handle)
                {
                    this.m_Archive.AddLast(metadata);
                    this.m_Discovered(metadata);
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.m_Archive.ToList().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_Archive.ToList().GetEnumerator();
            }
        }
    }
}
