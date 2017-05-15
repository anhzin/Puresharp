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
using System.Threading;
using System.Reflection;

namespace Puresharp.SimpleInjectorBattle
{
    public interface ICalculator
    {
    }

    public class Calculator : ICalculator
    {
    }

    static public class T1
    {
        static public int Index = -1;
    }

    static public class TTT<T>
        where T : class
    {
        static public Func<T>[] Buffer;
    }

    public class Container1 : IComposition
    {
        private object[] m_Instance;

        public void Add<T>(params T[] array) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(Type type) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(MethodInfo method) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(ConstructorInfo constructor) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(IEnumerable<T> enumerable) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(T instance) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(Func<T> ii)
            where T : class
        {
            TTT<T>.Buffer = new Func<T>[] { ii };
        }

        public void Add<T>(MethodInfo method, Lifetime lifetime) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(ConstructorInfo constructor, Lifetime lifetime) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(Type type, Lifetime lifetime) where T : class
        {
            throw new NotImplementedException();
        }

        public void Add<T>(Func<T> instance, Lifetime lifetime) where T : class
        {
            throw new NotImplementedException();
        }

        public T[] Array<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Enumerable<T>() where T : class
        {
            throw new NotImplementedException();
        }

        private int p = 0;

        public T Instance<T>()
            where T : class
        {
            return TTT<T>.Buffer[this.p]();
        }
    }

    static public class Program
    {
        [STAThread]
        static public void Main(string[] args)
        {
            //Composition<object>.Add<ICalculator>(() => new Calculator());

            //var t1 = Composition<object>.Enumerable<ICalculator>();

            //Composition<object>.Add<ICalculator>(() => new Calculator());
            ////Composition<object>.Add<ICalculator>(() => new Calculator());
            ////Composition<object>.Add<ICalculator>(() => new Calculator());

            //var t = Composition<object>.Array<ICalculator>();

            var _benchmark = new Benchmark(() => new Action(() => new Calculator()));
            _benchmark.Add("SimpleInjector", () => 
            {
                var _container = new SimpleInjector.Container();
                _container.Register<ICalculator, Calculator>(SimpleInjector.Lifestyle.Transient);
                return () => _container.GetInstance<ICalculator>();
            });
            _benchmark.Add("Puresharp [static]", () => 
            {
                Composition<object>.Add<ICalculator>(() => new Calculator());
                return () => Composition<object>.Lookup<ICalculator>.Instance();
            });
            _benchmark.Add("Puresharp [instance]", () =>
            {
                var _container = new Composition();
                _container.Add<ICalculator>(() => new Calculator());
                return () => Composition.Lookup<ICalculator>.Instance(_container);
            });
            _benchmark.Add("Puresharp [interface]", () =>
            {
                var _container = new Composition() as IComposition;
                _container.Add<ICalculator>(() => new Calculator());
                return () => _container.Instance<ICalculator>();
            });
            _benchmark.Add("Puresharp [X]", () =>
            {
                var _container = new Container1() as IComposition;
                _container.Add<ICalculator>(() => new Calculator());
                return () => _container.Instance<ICalculator>();
            });
            //_benchmark.Add("Puresharp [X2]", () =>
            //{
            //    var _container = Composition.Create();
            //    _container.Add<ICalculator>(() => new Calculator());
            //    return () => _container.Instance<ICalculator>();
            //});
            _benchmark.Add("DryIoc", () => 
            {
                var _container = new DryIoc.Container();
                _container.Register<ICalculator, Calculator>();
                return () => _container.Resolve<ICalculator>();
            });
            _benchmark.Add("DryIoc [interface]", () =>
            {
                var _container = new DryIoc.Container() as DryIoc.IContainer;
                _container.Register<ICalculator, Calculator>();
                return () => _container.Resolve<ICalculator>();
            });
            _benchmark.Add("Autofac", () =>
            {
                var _builder = new Autofac.ContainerBuilder();
                _builder.RegisterType<Calculator>().As<ICalculator>();
                var _container = _builder.Build(Autofac.Builder.ContainerBuildOptions.None);
                return () => _container.Resolve<ICalculator>();
            });
            _benchmark.Add("Ninject", () =>
            {
                var _container = new Ninject.StandardKernel();
                _container.Bind<ICalculator>().To<Calculator>();
                return () => _container.Get<ICalculator>();
            });
            _benchmark.Add("Abioc", () => 
            {
                var _setup = new Abioc.Registration.RegistrationSetup();
                _setup.Register<ICalculator, Calculator>();
                var _container = Abioc.ContainerConstruction.Construct(_setup, typeof(ICalculator).Assembly);
                return () => _container.GetService<ICalculator>();
            });
            _benchmark.Add("Grace", () => 
            {
                var _container = new Grace.DependencyInjection.DependencyInjectionContainer();
                _container.Configure(c => c.Export<Calculator>().As<ICalculator>());
                return () => _container.Locate<ICalculator>();
            });
            _benchmark.Run(Console.WriteLine);
        }
    }
}
