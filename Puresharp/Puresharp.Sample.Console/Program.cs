using Puresharp.Confluence;
using Puresharp.Sample.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Puresharp.Sample.Console
{
    public class Aspect1 : Aspect
    {
        public override IEnumerable<Advice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Before(() => System.Console.WriteLine("Hello World"));
        }
    }

    public class Boundary1 : Advice.IBoundary
    {
        public void Argument<T>(ParameterInfo parameter, ref T value)
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

        public void Method(MethodBase method, ParameterInfo[] signature)
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

        public void Yield()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Aspect.Weave<Aspect1>(typeof(Calculator).GetMethod("Add"));
            //Aspect.Weave<Aspect1>(typeof(Calculator).GetMethod("Test"));
            //Aspect.Weave<Aspect<Boundary1>>(typeof(Calculator).GetMethod("Add"));
            Aspect.Weave<Aspect<Boundary1>>(typeof(Calculator).GetMethod("TestAsync"));
            //Aspect.Weave<Aspect<Boundary1>>(typeof(Calculator).GetConstructors().Single());
            var calculator = new Calculator();
            //System.Console.WriteLine(calculator.Add(2, 5));
            //var g = calculator.Test(new string[] { "blabla" });
            //var y = g.ToArray();
            var i = calculator.TestAsync(3, 5).Result;
        }
    }
}
