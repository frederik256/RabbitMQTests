using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace rtest
{
    class Program
    {
        static void Main(string[] args)
        {
            //new TestHarness().Interactive();
            new TestHarness().Are_competing_workers_blocking_until_ack_Answer_NO();
        }
    }
}
