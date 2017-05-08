using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp.Sandbox
{
    static public class Program
    {
        static void Main(string[] arguments)
        {
            var _proxy = Proxy<Aspect1>.Create<ICalculator>(new Calculator());
            var _return = _proxy.Add(2, 3);
            Console.WriteLine(_return);
        }
    }

    public interface ICalculator
    {
        int Add(int a, int b);
    }

    public class Calculator : ICalculator
    {
        public int Add(int a, int b)
        {
            Console.WriteLine("Add!");
            return a + b;
        }
    }

    public class Aspect1 : Aspect
    {
        public override IEnumerable<Advice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Before(() => Console.WriteLine("Before!"));
        }
    }
}
