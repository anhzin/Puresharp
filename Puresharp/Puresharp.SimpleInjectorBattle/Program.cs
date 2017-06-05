using System;
using Benchmaker;
using Autofac;
using DryIoc;
using Ninject;
using Castle.Windsor;
using Microsoft.Practices.Unity;

namespace Puresharp.SimpleInjectorBattle
{
    public interface ICalculator
    {
    }

    [System.Composition.Export(typeof(ICalculator))]
    public class Calculator : ICalculator
    {
    }

    public class Primary
    {
    }

    public class Secondary
    {
    }
    
    static public class Program
    {
        [STAThread]
        static public void Main(string[] args)
        {
            var _benchmark = new Benchmark(() => new Action(() => new Calculator()));
            _benchmark.Add("SimpleInjector", () => 
            {
                var _container = new SimpleInjector.Container();
                _container.Register<ICalculator, Calculator>(SimpleInjector.Lifestyle.Transient);
                return () => _container.GetInstance<ICalculator>();
            });
            _benchmark.Add("Puresharp [static]", () =>
            {
                Puresharp.Composition.Container<Secondary>.Add<ICalculator>(() => new Calculator());
                return () => Puresharp.Composition.Container<Secondary>.Instance<ICalculator>();
            });
            _benchmark.Add("Puresharp [static] with optimizer", () => 
            {
                Puresharp.Composition.Container<Primary>.Add<ICalculator>(() => new Calculator());
                return () => Puresharp.Composition.Container<Primary>.Lookup<ICalculator>.Instance();
            });
            _benchmark.Add("Puresharp [instance]", () =>
            {
                var _container = new Puresharp.Composition.Container();
                _container.Add<ICalculator>(() => new Calculator());
                return () => _container.Instance<ICalculator>();
            });
            _benchmark.Add("Puresharp [instance] with optimizer", () =>
            {
                var _container = new Puresharp.Composition.Container();
                _container.Add<ICalculator>(() => new Calculator());
                return () => Puresharp.Composition.Container.Lookup<ICalculator>.Instance(_container);
            });
            _benchmark.Add("MEF", () =>
            {
                var _container = new System.Composition.Hosting.ContainerConfiguration().WithAssembly(typeof(ICalculator).Assembly).CreateContainer();
                return () => _container.GetExport<ICalculator>();
            });
            _benchmark.Add("Castle Windsor", () =>
            {
                var _container = new WindsorContainer();
                _container.Register(Castle.MicroKernel.Registration.Component.For<ICalculator>().ImplementedBy<Calculator>());
                return () => _container.Resolve<ICalculator>();
            });
            _benchmark.Add("Unity", () =>
            {
                var _container = new UnityContainer();
                _container.RegisterType<ICalculator, Calculator>();
                return () => _container.Resolve<ICalculator>();
            });
            _benchmark.Add("StuctureMap", () =>
            {
                var _container = new StructureMap.Container(_Builder => _Builder.For<ICalculator>().Use<Calculator>());
                return () => _container.GetInstance<ICalculator>();
            });
            _benchmark.Add("DryIoc", () => 
            {
                var _container = new DryIoc.Container();
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
