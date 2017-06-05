using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Puresharp.Composition
{
    internal partial class Linkup<T> : IEnumerable<T>
    {
        static public implicit operator T[] (Linkup<T> linkup)
        {
            if (linkup == null) { return null; }
            var _linkup = linkup;
            var _array = new T[_linkup.m_Count];
            for (var _index = 0; _index < _array.Length; _index++)
            {
                _array[_index] = _linkup.m_Value;
                _linkup = _linkup.m_Linkup;
            }
            return _array;
        }

        static public Linkup<T> Update(ref Linkup<T> linkup, T value)
        {
            var _linkup = new Linkup<T>(value);
            while (true)
            {
                var _junction = linkup;
                if (_junction == null)
                {
                    _linkup.m_Count = 1;
                    _linkup.m_Linkup = null;
                }
                else
                {
                    _linkup.m_Count = _junction.m_Count + 1;
                    _linkup.m_Linkup = _junction;
                }
                if (object.ReferenceEquals(Interlocked.CompareExchange(ref linkup, _linkup, _junction), _junction)) { return _linkup; }
            }
        }

        private int m_Count;
        private T m_Value;
        private Linkup<T> m_Linkup;

        private Linkup(T value)
        {
            this.m_Count = 1;
            this.m_Value = value;
        }

        public int Count
        {
            get { return this.m_Count; }
        }

        public void Update(T predicate, T value)
        {
            if (object.Equals(predicate, this.m_Value)) { this.m_Value = value; }
            var _linkup = this.m_Linkup;
            while (_linkup != null)
            {
                if (object.Equals(predicate, _linkup.m_Value)) { _linkup.m_Value = value; }
                _linkup = _linkup.m_Linkup;
            }
        }

        public void Update(Func<T, bool> predicate, T value)
        {
            if (predicate(this.m_Value)) { this.m_Value = value; }
            var _linkup = this.m_Linkup;
            while (_linkup != null)
            {
                if (predicate(_linkup.m_Value)) { _linkup.m_Value = value; }
                _linkup = _linkup.m_Linkup;
            }
        }

        public IEnumerator<T> Enumerator()
        {
            yield return this.m_Value;
            var _linkup = this.m_Linkup;
            while (_linkup != null)
            {
                yield return _linkup.m_Value;
                _linkup = _linkup.m_Linkup;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Enumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Enumerator();
        }
    }
}