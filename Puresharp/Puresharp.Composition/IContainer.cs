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
        /// Add factory to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="function">Factory</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(Func<T> function, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add constructor to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="constructor">Constructor</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(ConstructorInfo constructor, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add a static method to specify how to create instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="method">Method</param>
        /// <param name="lifetime">Lifetime</param>
        void Add<T>(MethodInfo method, Lifetime lifetime)
            where T : class;

        /// <summary>
        /// Add a container to specify where to find instance.
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="container">Container</param>
        void Add<T>(IContainer container)
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
    }
}
