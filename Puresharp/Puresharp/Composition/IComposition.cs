using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Puresharp
{
    public interface IComposition : IDisposable
    {
        void Add<T>(T instance)
            where T : class;

        void Add<T>(params T[] array)
            where T : class;

        void Add<T>(IEnumerable<T> enumerable)
            where T : class;

        void Add<T>(Func<T> instance)
            where T : class;

        void Add<T>(Type type)
            where T : class;

        void Add<T>(ConstructorInfo constructor)
            where T : class;

        void Add<T>(MethodInfo method)
            where T : class;

        void Add<T>(Func<T> instance, Lifetime lifetime)
            where T : class;

        void Add<T>(Type type, Lifetime lifetime)
            where T : class;

        void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class;

        void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class;

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Instance<T>()
            where T : class;

        IEnumerable<T> Enumerable<T>()
            where T : class;

        T[] Array<T>()
            where T : class;
    }
}
