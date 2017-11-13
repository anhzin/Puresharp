using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Puresharp.Confluence
{
    public sealed partial class Advice
    {
        public partial class Boundary : Advice.IBoundary
        {
            virtual public void Instance<T>(T instance)
            {
            }

            virtual public void Argument<T>(ParameterInfo parameter, ref T value)
            {
            }

            virtual public void Begin()
            {
            }

            virtual public void Await(MethodInfo method, ref Task task)
            {
            }

            public virtual void Await<T>(MethodInfo method, ref Task<T> task)
            {
            }

            public virtual void Continue()
            {
            }

            virtual public void Return()
            {
            }

            virtual public void Throw(ref Exception exception)
            {
            }

            virtual public void Return<T>(ref T value)
            {
            }

            virtual public void Throw<T>(ref Exception exception, ref T value)
            {
            }

            virtual public void Dispose()
            {
            }
        }
    }
}