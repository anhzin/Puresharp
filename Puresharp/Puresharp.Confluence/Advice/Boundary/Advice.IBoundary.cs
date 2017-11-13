using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public interface IBoundary : IDisposable
        {
            void Instance<T>(T instance);
            void Argument<T>(ParameterInfo parameter, ref T value);
            void Begin();
            void Await(MethodInfo method, ref Task task);
            void Await<T>(MethodInfo method, ref Task<T> task);
            void Continue();
            void Return();
            void Throw(ref Exception exception);
            void Return<T>(ref T value);
            void Throw<T>(ref Exception exception, ref T value);
        }
    }
}