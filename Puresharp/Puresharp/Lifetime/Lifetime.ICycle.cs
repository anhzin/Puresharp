using System;

namespace Puresharp
{
    public partial class Lifetime
    {
        public interface ICycle
        {
            void Establish<T>(Func<T> lifecycle)
                where T : IDisposable;
        }
    }
}
