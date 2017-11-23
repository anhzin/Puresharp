using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Puresharp.Confluence;
using Puresharp.Sample;
using Puresharp.Sample.Library;
using System.Threading.Tasks;

namespace Puresharp.Sample.Console
{
    public class Aspect1 : Aspect
    {
        public override IEnumerable<Advice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Before(() => System.Console.WriteLine("Hello World"));
        }
    }

    public class Aspect2 : Aspect
    {
        public override IEnumerable<Advice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Before(() => System.Console.WriteLine($"Before => { method.Name }"));
            yield return Advice.Basic.After(() => System.Console.WriteLine($"After => { method.Name }"));
        }
    }

    public class Boundary1 : Advice.IBoundary
    {
        public void Argument<T>(ParameterInfo parameter, ref T value)
        {
            throw new NotImplementedException();
        }

        public void Await(MethodInfo method, ref Task task)
        {
            throw new NotImplementedException();
        }

        public void Await<T>(MethodInfo method, ref Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void Begin()
        {
            throw new NotImplementedException();
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Instance<T>(T instance)
        {
            throw new NotImplementedException();
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        public void Return<T>(ref T value)
        {
            throw new NotImplementedException();
        }

        public void Throw(ref Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Throw<T>(ref Exception exception, ref T value)
        {
            throw new NotImplementedException();
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

            Aspect.Weave<Aspect2>(typeof(Calculator).GetMethod("AddEx"));
            Aspect.Weave<Aspect2>(typeof(SuperCalculator).GetMethod("AddEx"));
            Aspect.Weave<Aspect<Boundary1>>(typeof(SuperCalculator).GetMethod("AddEx"));

            var calculator = new Calculator();
            //var superCalculator = new SuperCalculator();

            //var res = superCalculator.AddEx(2, 3);
            //System.Console.WriteLine(calculator.Add(2, 5));
            //var g = calculator.Test(new string[] { "blabla" });
            //var y = g.ToArray();
            var i = calculator.TestAsync(3, 5).Result;
        }
    }
}
