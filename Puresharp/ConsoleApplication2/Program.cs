using Puresharp.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public interface IA
    {
        void Hello();
    }

    public interface IH
    {
        void Go();
        void Go1();
    }

    public class A : IA
    {
        public string Test = "kkk";

        virtual public void Hello()
        {
            //Console.WriteLine("A : " + Test);
        }

        public void Go()
        {
        }

        static public void TestA(object p)
        {
            (p as A).Hello();
        }
    }

    public class H : IH
    {
        private A a = new A();
        private IA aa = new A();

        public virtual void Go()
        {
            this.a.Go();
        }

        public virtual void Go1()
        {
            this.aa.Hello();
        }
    }

    public class B : A
    {
        public override void Hello()
        {
            Console.WriteLine("B");
        }
    }

    public delegate void hello(A o);

    class Program
    {
        static void Main(string[] args)
        {
            var method = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(object) }, typeof(object), true);
            var body = method.GetILGenerator();

            //body.Emit(OpCodes.Ldarg_0);
            //body.Emit(OpCodes.Ldc_I8, typeof(A).GetMethod("Hello").MethodHandle.GetFunctionPointer().ToInt64());
            
            //body.EmitCalli(OpCodes.Calli, CallingConventions.Standard, typeof(void), new Type[] { typeof(object) }, null);
            //body.Emit(OpCodes.Ret);

            body.Emit(OpCodes.Ldarg_1);
            body.Emit(OpCodes.Call, typeof(A).GetMethod("Go"));
            body.Emit(OpCodes.Ret);


            var method1 = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(object), typeof(object) }, typeof(object), true);
            var body1 = method.GetILGenerator();

            body1.Emit(OpCodes.Ldarg_1);
            
            body1.Emit(OpCodes.Call, typeof(A).GetMethod("Go"));
            body1.Emit(OpCodes.Ret);

            var a = new A() { Test = "yes!" };
            var hhh = method.CreateDelegate(typeof(Action<object>), null) as Action<object>;

            var nnn = method.CreateDelegate(typeof(Action<object>), null) as Action<object>;

            hhh(a);
            nnn(a);
            IH h = new H();

            var bench = new Benchmaker.Benchmark(() => new Action(() => { a.Hello(); }));
            bench.Add("delegate", () => new Action(() => { hhh(a); }));
            bench.Add("virtual => non virtual", () => new Action(() => { h.Go(); }));
            bench.Add("virtual => virtual", () => new Action(() => { h.Go1(); }));
            bench.Run();
        }
    }
}
