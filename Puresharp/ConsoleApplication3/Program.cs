using ClassLibrary1;
using Puresharp.Confluence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleApplication3
{
    public class Boundary1 : Advice.IBoundary
    {
        public void Argument<T>(ParameterInfo parameter, ref T value)
        {
        }

        public void Await(MethodInfo method, ref Task task)
        {
        }

        public void Await<T>(MethodInfo method, ref Task<T> task)
        {
        }

        public void Begin()
        {
        }

        public void Continue()
        {
        }

        public void Dispose()
        {
        }

        public void Instance<T>(T instance)
        {
        }

        public void Return()
        {
        }

        public void Return<T>(ref T value)
        {
        }

        public void Throw(ref Exception exception)
        {
        }

        public void Throw<T>(ref Exception exception, ref T value)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Aspect.Weave<Aspect<Boundary1>>(typeof(Calculator).GetMethod("TestAsync"));
            var c = new Calculator();
            var gg = c.TestAsync(3, 2);
        }
    }
}
