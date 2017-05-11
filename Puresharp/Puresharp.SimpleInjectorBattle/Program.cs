using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp.SimpleInjectorBattle
{
    public interface ICalculator
    {
    }

    public class Calculator : ICalculator
    {
    }

    public class Container1
    {
        public class C<T>
        {
            static public bool activated;
            static internal object handle = new object();
            static internal T Singleton;
            static internal Func<T> p;

            public T Sing()
            {
                return Singleton;
            }
        }

        virtual public void Add<T>(Func<T> p)
        {
            C<T>.p = p;
        }

        virtual public void AddSingleton<T>(Func<T> p)
        {
            C<T>.p = new Func<T>(() =>
            {
                lock (C<T>.handle)
                {
                    if (C<T>.activated) { return C<T>.p(); }
                    var a  = C<T>.Singleton = p();
                    C<T>.p = new Func<T>(() => a);
                    C<T>.activated = true;
                    return a;
                }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        virtual public T Instance<T>()
        {
            return C<T>.p();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var container = new SimpleInjector.Container();

            // Register your types, for instance:
            container.Register<ICalculator, Calculator>(SimpleInjector.Lifestyle.Singleton);
            //container.Register<ITestInjectedClass, TestInjectedClass>(Lifestyle.Singleton);
            //container.Register<IUserRepository, TestInjectedClass>(Lifestyle.Singleton);
            //container.Register<IUserContext, WinFormsUserContext>();
            var c = container.GetInstance<ICalculator>();
            var max = 10000000;
            var sw = new Stopwatch();
            sw.Restart();
            for (var i = 0; i < max; i++)
            {
                c = container.GetInstance<ICalculator>();
            }
            sw.Stop();
            Console.WriteLine("simple injector : " + sw.Elapsed);

            var sw1 = new Stopwatch();
            sw1.Restart();
            for (var i = 0; i < max; i++)
            {
                c = new Calculator();
            }
            sw1.Stop();
            Console.WriteLine("new : " + sw1.Elapsed);
            
            var lambda = new Func<ICalculator>(() => new Calculator());

            var sw2 = new Stopwatch();
            sw2.Restart();
            for (var i = 0; i < max; i++)
            {
                c = lambda();
            }
            sw2.Stop();
            Console.WriteLine("lambda : " + sw2.Elapsed);

            Calculator cp = new Calculator();
            var cc = new Container1();
            cc.AddSingleton<ICalculator>(() => new Calculator());
          //  cc.Add<ICalculator>(() => cp);

            c = cc.Instance<ICalculator>();

            var sw3 = new Stopwatch();
            sw3.Restart();
            for (var i = 0; i < max; i++)
            {
                c = cc.Instance<ICalculator>();
            }
            sw3.Stop();
            Console.WriteLine("lookup : " + sw3.Elapsed);
        }
    }
}
