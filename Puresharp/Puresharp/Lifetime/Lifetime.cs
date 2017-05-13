using System;

namespace Puresharp
{
    public partial class Lifetime
    {
        static public readonly Lifetime Volatile = new Lifetime();
        static public readonly Lifetime Singleton = new Lifetime();

        protected Lifetime()
        {
        }
    }
}
