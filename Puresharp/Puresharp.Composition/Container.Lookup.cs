using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp.Composition
{
    public sealed partial class Container
    {
        static private class Lookup<T>
            where T : class
        {
            static internal Container.Store<T>[] Buffer = Container.Lookup<T>.Initialize();

            static private Container.Store<T>[] Initialize()
            {
                lock (Container.m_Handle)
                {
                    var _buffer = new Container.Store<T>[Container.m_Sequence];
                    for (var _index = 0; _index < _buffer.Length; _index++) { _buffer[_index] = new Container.Store<T>(); }
                    Container.m_Instantiation.Add(() =>
                    {
                        Container.Lookup<T>.Buffer = new Container.Store<T>[Container.m_Sequence];
                        for (var _index = 0; _index < _buffer.Length; _index++) { Container.Lookup<T>.Buffer[_index] = _buffer[_index]; }
                        Container.Lookup<T>.Buffer[_buffer.Length] = new Container.Store<T>();
                    });
                    return _buffer;
                }
            }

            static public T Instance(int index)
            {
                return Container.Lookup<T>.Buffer[index].Instance();
            }

            static public IEnumerable<T> Enumerable(int index)
            {
                return Container.Lookup<T>.Buffer[index].Enumerable();
            }
        }
    }
}
