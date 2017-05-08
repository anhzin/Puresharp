using System;
using System.Reflection;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public interface IBoundary : IDisposable
        {
            void Method(MethodBase method, ParameterInfo[] signature);
            void Instance<T>(T instance);
            void Argument<T>(int index, ParameterInfo parameter, ref T value);
            void Begin();
            void Continue();
            void Yield();
            void Return();
            void Throw(ref Exception exception);
            void Return<T>(ref T value);
            void Throw<T>(ref Exception exception, ref T value);
        }
    }
}