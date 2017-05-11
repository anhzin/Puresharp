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

    static public class CCC<T>
    {
        static public Func<T> p;

        static public Func<T> pp
        {
            get { return p; }
        }
    }

    static public class CCCC
    {
        static public T Instance<T>()
            where T : class
        {
            return CCC<T>.p();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var container = new SimpleInjector.Container();

            // Register your types, for instance:
            container.Register<ICalculator, Calculator>(SimpleInjector.Lifestyle.Transient);
         
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
            CCC<ICalculator>.p = lambda;
            Composition.Add<ICalculator>(lambda, Lifetime.Volatile);

            var sw2 = new Stopwatch();
            sw2.Restart();
            for (var i = 0; i < max; i++)
            {
                c = CCC<ICalculator>.pp();
            }
            sw2.Stop();
            Console.WriteLine("lambda : " + sw2.Elapsed);

            //Calculator cp = new Calculator();
            var cc = new Container1();
            //cc.AddSingleton<ICalculator>(() => new Calculator());
            cc.Add<ICalculator>(lambda);

            


            var sw3 = new Stopwatch();
            sw3.Restart();
            for (var i = 0; i < max; i++)
            {
                c = cc.Instance<ICalculator>();
            }
            sw3.Stop();
            Console.WriteLine("Container1 : " + sw3.Elapsed);

            c = Composition.Instance<ICalculator>();

            var sw4 = new Stopwatch();
            sw4.Restart();
            for (var i = 0; i < max; i++)
            {
                c = Composition.Lookup<ICalculator>.Instance();//.Instance<ICalculator>();
            }
            sw4.Stop();
            Console.WriteLine("Puresharp : " + sw4.Elapsed);
        }
    }
}
