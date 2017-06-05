using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Puresharp.Composition
{
    /// <summary>
    /// Lifetime
    /// </summary>
    public partial class Lifetime
    {
        /// <summary>
        /// Volatile
        /// </summary>
        static public readonly Lifetime Volatile = new Lifetime();

        /// <summary>
        /// Singleton
        /// </summary>
        static public readonly Lifetime Singleton = new Lifetime();

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <summary>
        /// Lifetime
        /// </summary>
        protected Lifetime()
        {
        }
    }
}
