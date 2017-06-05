using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp.Composition
{
    /// <summary>
    /// Composition container.
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Add instance as singleton.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="instance">Instance</param>
        void Add<T>(T instance)
            where T : class;

        /// <summary>
        /// Add enumerable of instances as multiton.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="enumerable">Enumerable</param>
        void Add<T>(IEnumerable<T> enumerable)
            where T : class;

        /// <summary>
        /// Add factory to specify how to create instance as volatile.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="factory">Factory</param>
        void Add<T>(Func<T> factory)
            where T : class;

        /// <summary>
        /// Add type to specifiy which type to create instance as volatile
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="type">Type</param>
        void Add<T>(Type type)
            where T : class;

        /// <summary>
        /// Add constructor to specify how to create instance as volatile.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="constructor">Constructor</param>
        void Add<T>(ConstructorInfo constructor)
            where T : class;

        /// <summary>
        /// Add method to specify declaring type will be created using parameterless constructor before using method to initialize instance as volatile.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="method">Method</param>
        void Add<T>(MethodInfo method)
            where T : class;

        /// <summary>
        /// Add factory to specify how to create instance and specify lifetime.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="factory">Factory</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(Func<T> factory, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add type to specifiy which type to create instance and specify lifetime.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="type">Type</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(Type type, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add constructor to specify how to create instance and specify lifetime.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="constructor">Constructor</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add method to specify declaring type will be created using parameterless constructor before using method to initialize instance and specify lifetime.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="method">Method</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Obtain a single instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <returns>Instance</returns>
        T Instance<T>()
            where T : class;

        /// <summary>
        /// Obtain enumerable of all instances registered.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <returns>Enumerable</returns>
        IEnumerable<T> Enumerable<T>()
            where T : class;

        /// <summary>
        /// Obtain array of all instances registered.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <returns>Array</returns>
        T[] Array<T>()
            where T : class;
    }
}
