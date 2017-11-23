using ClassLibrary1;
using Puresharp.Confluence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Puresharp.Discovery;

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

    public class Boundary2 : Advice.Boundary
    {
        public override void Begin()
        {
        }
    }

    public class Aspect1 : Aspect
    {
        public override IEnumerable<Advice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Before(() => Console.WriteLine(method.Name));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Metadata.Methods.Discovered += new Metadata.Listener<MethodInfo>(_Method =>
            {
                Aspect.Weave<Aspect<Boundary1>>(_Method);
            });

            //Aspect.Weave<Aspect<Boundary1>>(typeof(Calculator).GetMethod("Add"));
            //Aspect.Weave<Aspect<Boundary2>>(typeof(Calculator).GetMethod("Add"));
            //Aspect.Weave<Aspect1>(typeof(Calculator).GetMethod("Add"));
            var c = new Calculator();
            var gg = c.Add(3, 2);
        }
    }
}
