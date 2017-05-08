using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    static public partial class Data
    {
        public class Dictionary<T> : IList<T>
        {
            static public implicit operator Data.Collection<T>(Dictionary<T> dictionary)
            {
                return dictionary == null ? null : dictionary.m_Collection;
            }

            private IList<T> m_List;
            private Data.Collection<T> m_Collection;

            public Dictionary()
                : this(new List<T>())
            {
            }

            public Dictionary(IList<T> list)
            {
                if (list.IsReadOnly) { throw new NotSupportedException(); }
                this.m_List = list;
                this.m_Collection = new Data.Collection<T>(list);
            }

            public int Count
            {
                get { return this.m_List.Count; }
            }

            bool ICollection<T>.IsReadOnly
            {
                get { return false; }
            }

            public T this[int index]
            {
                get { return this.m_List[index]; }
                set { this.m_List[index] = value; }
            }

            public int Index(T item)
            {
                return this.m_List.IndexOf(item);
            }

            int IList<T>.IndexOf(T item)
            {
                return this.m_List.IndexOf(item);
            }

            public void Add(T item)
            {
                this.m_List.Add(item);
            }

            public void Add(int index, T item)
            {
                this.m_List.Insert(index, item);
            }

            void IList<T>.Insert(int index, T item)
            {
                this.m_List.Insert(index, item);
            }

            public void Remove(int index)
            {
                this.m_List.RemoveAt(index);
            }

            void IList<T>.RemoveAt(int index)
            {
                this.m_List.RemoveAt(index);
            }

            public void Clear()
            {
                this.m_List.Clear();
            }

            public bool Contains(T item)
            {
                return this.m_List.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                this.m_List.CopyTo(array, arrayIndex);
            }

            public int Remove(T item, int limit)
            {
                var _index = 0;
                while (_index < limit && this.m_List.Remove(item)) { _index++; }
                return _index;
            }

            bool ICollection<T>.Remove(T item)
            {
                return this.m_List.Remove(item);
            }

            public IEnumerator<T> Enumerator()
            {
                return this.m_List.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.m_List.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_List.GetEnumerator();
            }
        }

        //public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>
        //{
        //}
    }
}
