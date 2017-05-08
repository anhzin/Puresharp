using System;
using System.Reflection;

namespace Puresharp
{
    public sealed partial class Advice
    {
        public partial class Boundary : Advice.IBoundary
        {
            virtual public void Method(MethodBase method, ParameterInfo[] signature)
            {
            }

            virtual public void Instance<T>(T instance)
            {
            }

            virtual public void Argument<T>(ParameterInfo parameter, ref T value)
            {
            }

            virtual public void Begin()
            {
            }

            virtual public void Continue()
            {
            }

            virtual public void Yield()
            {
            }

            public void Return()
            {
            }

            public void Throw(ref Exception exception)
            {
            }

            public void Return<T>(ref T value)
            {
            }

            public void Throw<T>(ref Exception exception, ref T value)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}