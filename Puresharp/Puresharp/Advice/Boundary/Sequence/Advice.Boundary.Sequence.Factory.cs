using System;
using System.Reflection;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public partial class Boundary
        {
            public partial class Sequence
            {
                public partial class Factory : Advice.Boundary.IFactory
                {
                    private Advice.Boundary.IFactory[] m_Sequence;

                    public Factory(params Advice.Boundary.IFactory[] sequence)
                    {
                        this.m_Sequence = sequence;
                    }

                    public Advice.IBoundary Create()
                    {
                        var _sequence = this.m_Sequence;
                        var _array = new Advice.IBoundary[_sequence.Length];
                        for (var _index = 0; _index < _sequence.Length; _index++) { _array[_index] = _sequence[_index].Create(); }
                        return new Advice.Boundary.Sequence(_array); 
                    }
                }
            }
        }
    }
}