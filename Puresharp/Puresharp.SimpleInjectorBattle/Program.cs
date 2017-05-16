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
using Benchmaker;

namespace Puresharp.SimpleInjectorBattle
{
    public interface ICalculator
    {
    }

    public class Calculator : ICalculator
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
                Composition<object>.Add<ICalculator>(() => new Calculator());
                return () => Composition<object>.Lookup<ICalculator>.Instance();
            });
            _benchmark.Add("Puresharp [instance]", () =>
            {
                var _container = new Composition();
                _container.Add<ICalculator>(() => new Calculator());
                return () => Composition.Lookup<ICalculator>.Instance(_container);
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
