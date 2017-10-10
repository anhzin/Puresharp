using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp.Sample.Library
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public IEnumerable<string> Test(IEnumerable<string> p)
        {
            return p.Select(u => u);
        }

        public async Task Hello()
        {
        }

        public async Task World()
        {
        }

        public async Task<int> TestAsync(int a, int b)
        {
            await this.Hello();
            await this.World();
            return a + b;
        }
    }
}
