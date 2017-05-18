using System;
using System.Collections.Generic;
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
                    private Advice.Boundary.IFactory[] Sequence;

                    public Factory(params Advice.Boundary.IFactory[] sequence)
                    {
                        var _list = new List<Advice.Boundary.IFactory>();
                        foreach (var _boundary in sequence)
                        {
                            if (_boundary is Advice.Boundary.Sequence.Factory) { _list.AddRange((_boundary as Advice.Boundary.Sequence.Factory).Sequence); }
                            else { _list.Add(_boundary); }
                        }
                        this.Sequence = _list.ToArray();
                    }

                    public Advice.IBoundary Create()
                    {
                        var _sequence = this.Sequence;
                        var _array = new Advice.IBoundary[_sequence.Length];
                        for (var _index = 0; _index < _sequence.Length; _index++) { _array[_index] = _sequence[_index].Create(); }
                        return new Advice.Boundary.Sequence(_array); 
                    }
                }
            }
        }
    }
}