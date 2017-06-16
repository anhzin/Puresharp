using System;

namespace Puresharp.Composition
{
    /// <summary>
    /// Lifetime
    /// </summary>
    public partial class Lifetime
    {
        static private Lifetime m_Volatile = new Lifetime();
        static private Lifetime m_Singleton = new Lifetime();

        /// <summary>
        /// Volatile
        /// </summary>
        static public Lifetime Volatile
        {
            get { return Lifetime.m_Volatile; }
        }

        /// <summary>
        /// Singleton
        /// </summary>
        static public Lifetime Singleton
        {
            get { return Lifetime.m_Singleton; }
        }

        /// <summary>
        /// Lifetime
        /// </summary>
        protected Lifetime()
        {
        }
    }
}
