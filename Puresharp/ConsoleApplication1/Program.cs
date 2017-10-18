using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class C
    {
        virtual public void A()
        {
        }

        public void B()
        {
        }
    }

    class Program
    {
        private C c = new C();

        virtual public void Test()
        {
            //this.c.A();
        }

        virtual public void Test1()
        {
            this.c.B();
        }

        

        static void Main(string[] args)
        {
            var method = typeof(Program).GetMethod("Test");
            var p = new Program();
            var bench = new Benchmaker.Benchmark(() => new Action(() => p.Test()));
            bench.Add("Optimistic", () => new Action(() => p.Test1()));
            bench.Run();
        }
    }
}
