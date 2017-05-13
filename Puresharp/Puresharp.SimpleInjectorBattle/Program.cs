using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
using Autofac;
using Ninject;

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

    public class Container
    {
        public T Instance<T>()
            where T : class
        {
            return Composition.Lookup<T>.Instance();
        }
    }

    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            var _calculator = null as ICalculator;
            Program.Run("None", () => { _calculator = new Calculator(); });

            var _simpleInjector = new SimpleInjector.Container();
            _simpleInjector.Register<ICalculator, Calculator>(SimpleInjector.Lifestyle.Transient);
            Program.Run("SimpleInjector", () => { _calculator = _simpleInjector.GetInstance<ICalculator>(); });

            Composition.Add<ICalculator>(() => new Calculator());
            Program.Run("Puresharp", () => { _calculator = Composition.Lookup<ICalculator>.Instance(); });
            
            DryIoc.Container dryioc = new DryIoc.Container();
            dryioc.Register<ICalculator, Calculator>();
            Program.Run("DryIoc", () => { _calculator = dryioc.Resolve<ICalculator>(); });
            
            Autofac.ContainerBuilder autofacbuilder = new Autofac.ContainerBuilder();
            autofacbuilder.RegisterType<Calculator>().As<ICalculator>();
            var autofac = autofacbuilder.Build(Autofac.Builder.ContainerBuildOptions.None);
            Program.Run("Autofac", () => { _calculator = autofac.Resolve<ICalculator>(); });
            
            var ninject = new Ninject.StandardKernel();
            ninject.Bind<ICalculator>().To<Calculator>();
            Program.Run("Ninject", () => { _calculator = ninject.Get<ICalculator>(); });
            
            Abioc.Registration.RegistrationSetup abiocsetup = new Abioc.Registration.RegistrationSetup();
            abiocsetup.Register<ICalculator, Calculator>();
            var abioc = Abioc.ContainerConstruction.Construct(abiocsetup, typeof(ICalculator).Assembly);
            Program.Run("Abioc", () => { _calculator = abioc.GetService<ICalculator>(); });

            Grace.DependencyInjection.DependencyInjectionContainer gracecontainer = new Grace.DependencyInjection.DependencyInjectionContainer();
            gracecontainer.Configure(c => c.Export<Calculator>().As<ICalculator>());
            Program.Run("Grace", () => { _calculator = gracecontainer.Locate<ICalculator>(); });
        }

        static public void Run(string name, Action action)
        {
            for (var _index = 0; _index < 100; _index++) { action(); } 
            var _measure = new Stopwatch();
            _measure.Start();
            for (var i = 0; i < 1000000; i++) { action(); }
            _measure.Stop();
            Console.WriteLine($"{ name } : { Convert.ToInt32(_measure.Elapsed.TotalMilliseconds) }");
        }
    }
}
