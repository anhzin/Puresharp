using System;
using System.Reflection;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            public partial class Sequence : Advice.IBoundary
            {
                private Advice.IBoundary[] m_Sequence;

                public Sequence(params Advice.IBoundary[] sequence)
                {
                    this.m_Sequence = sequence;
                }

                public void Method(MethodBase method, ParameterInfo[] signature)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Method(method, signature); }
                }

                public void Instance<T>(T instance)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Instance(instance); }
                }

                public void Argument<T>(ParameterInfo parameter, ref T value)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Argument(parameter, ref value); }
                }

                public void Begin()
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Begin(); }
                }

                public void Continue()
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Continue(); }
                }

                public void Await()
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Await(); }
                }

                public void Return()
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Return(); }
                }

                public void Throw(ref Exception exception)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Throw(ref exception); }
                }

                public void Return<T>(ref T value)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Return(ref value); }
                }

                public void Throw<T>(ref Exception exception, ref T value)
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Throw(ref exception, ref value); }
                }

                public void Dispose()
                {
                    var _sequence = this.m_Sequence;
                    for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Dispose(); }
                }
            }
        }
    }
}