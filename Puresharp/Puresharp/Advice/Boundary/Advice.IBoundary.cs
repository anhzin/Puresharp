using System;
using System.Reflection;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public interface IBoundary : IDisposable
        {
            void Instance<T>(T instance);
            void Runtime<T>(int index, ref T value);
            void Invoke();
            void Resume();
            void Yield();
            void Return();
            void Throw(ref Exception exception);
            void Return<T>(ref T value);
            void Throw<T>(ref Exception exception, ref T value);
        }
    }
}