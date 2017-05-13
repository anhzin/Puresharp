using System;

namespace Puresharp
{
    static public partial class Data
    {
        public interface IMemory : IDisposable
        {
            bool Defined<T>();
            void Define<T>(T value);
            T Value<T>();
        }
    }
}
