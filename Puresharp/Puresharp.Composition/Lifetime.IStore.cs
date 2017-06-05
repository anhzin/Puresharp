using System;

namespace Puresharp.Composition
{
    public partial class Lifetime
    {
        /// <summary>
        /// Lifetime through store
        /// </summary>
        public interface IStore
        {
            /// <summary>
            /// Save value into store.
            /// </summary>
            /// <typeparam name="T">Type</typeparam>
            /// <param name="value">Value</param>
            void Save<T>(T value)
                where T : class;

            /// <summary>
            /// Query value from store
            /// </summary>
            /// <typeparam name="T">Type</typeparam>
            /// <returns>Value</returns>
            T Query<T>()
                where T : class;
        }
    }
}
