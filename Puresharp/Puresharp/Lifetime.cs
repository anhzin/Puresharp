using System;
using System.Reflection;

namespace Puresharp
{
    public class Lifetime
    {
        static public readonly Lifetime Volatile = new Lifetime();
        static public readonly Lifetime Singleton = new Lifetime();

        internal Lifetime()
        {
        }
    }
}
