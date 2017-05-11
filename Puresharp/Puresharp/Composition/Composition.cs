using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Puresharp
{
    static public partial class Composition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Add<T>(Func<T> instance)
            where T : class
        {
            while (true)
            {
                var _instance = Composition.Container<T>.Instance;
                if (_instance == Composition.Container<T>.None)
                {
                    if (Interlocked.CompareExchange(ref Composition.Container<T>.Instance, instance, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Join(ref Composition.Container<T>.Linkup, instance);
                        while (true)
                        {
                            var _enumerable = Composition.Container<T>.Enumerable;
                            var _linkup = Composition.Container<T>.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref Composition.Container<T>.Enumerable, Composition.Container<T>.Empty, _enumerable) == _enumerable) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref Composition.Container<T>.Enumerable, new Func<IEnumerable<T>>(() => { var _array = new T[_activation.Length]; for (var _index = 0; _index < _array.Length; _index++) { _array[_index] = _activation[_index](); } return _array; }), _enumerable) == _enumerable) { return; }
                            }
                        }
                    }
                }
                else
                {
                    if (Interlocked.CompareExchange(ref Composition.Container<T>.Instance, Composition.Container<T>.Multiple, _instance) == _instance)
                    {
                        Data.Linkup<Func<T>>.Join(ref Composition.Container<T>.Linkup, instance);
                        while (true)
                        {
                            var _enumerable = Composition.Container<T>.Enumerable;
                            var _linkup = Composition.Container<T>.Linkup;
                            if (_linkup == null) { if (Interlocked.CompareExchange(ref Composition.Container<T>.Enumerable, Composition.Container<T>.Empty, _enumerable) == _enumerable) { return; } }
                            else
                            {
                                var _activation = (Func<T>[])_linkup;
                                if (Interlocked.CompareExchange(ref Composition.Container<T>.Enumerable, new Func<IEnumerable<T>>(() => { var _array = new T[_activation.Length]; for (var _index = 0; _index < _array.Length; _index++) { _array[_index] = _activation[_index](); } return _array; }), _enumerable) == _enumerable) { return; }
                            }
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class
        {
            if (object.ReferenceEquals(lifetime, Lifetime.Volatile)) { Composition.Add<T>(instance); }
            else if (object.ReferenceEquals(lifetime, Lifetime.Singleton))
            {
                var _instance = new Func<T>(() =>
                {
                    lock (Composition.Container<T>.Handle)
                    {
                        Func<T> _item;
                        if (Composition.Container<T>.Dictionary.TryGetValue(instance, out _item)) { return _item(); }
                        var _singleton = instance();
                        _item = new Func<T>(() => _singleton);
                        Composition.Container<T>.Dictionary.Add(instance, _item);
                        Interlocked.CompareExchange(ref Composition.Container<T>.Instance, _item, instance);
                        var _linkup = Composition.Container<T>.Linkup;
                        if (_linkup != null) { _linkup.Update(instance, _item); }
                        return _singleton;
                    }
                });
                Composition.Add<T>(instance);
            }
            else
            {
                //TODO!
                throw new NotSupportedException();
            }
        } 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Clear<T>()
            where T : class
        {
            lock (Composition.Container<T>.Handle)
            {
                Composition.Container<T>.Instance = Composition.Container<T>.None;
                Composition.Container<T>.Enumerable = Composition.Container<T>.Empty;
                Composition.Container<T>.Dictionary.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T Instance<T>()
            where T : class
        {
            return Composition.Container<T>.Instance();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<T> Enumerable<T>()
            where T : class
        {
            return Composition.Container<T>.Enumerable();
        }
    }
}
