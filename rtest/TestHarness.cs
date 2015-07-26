using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace rtest
{
    class TestHarness
    {
        public void Interactive()
        {
            int counter = 0;
            Console.WriteLine("Press:");
            Console.WriteLine("1. To publish");
            Console.WriteLine("2. To consume");
            Console.WriteLine("0. To exit");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                string val = key.KeyChar.ToString();

                switch (val)
                {
                    case "0": return;
                    case "1": NewTask.Publish(counter++); break;
                    case "2": Worker.ConsumeOne(() =>
                    {
                        Console.WriteLine(" [x] Press any key to ack");
                        Console.ReadKey(true);
                    }); break;
                    default: break;
                };
            }
        }

        public void NoBlock()
        {

        }

        public void Block(ManualResetEvent x)
        {
            x.WaitOne();
        }

        public void Are_competing_workers_blocking_until_ack_Answer_NO()
        {
            // if a background hangs on uniquely to a queue, then the main thread can't consume  other messages.
            // the messages should return out of order from publish (0 being last)
            using (ManualResetEvent pauseBeforeAck = new ManualResetEvent(false))
            using (ManualResetEvent firstDequeueIsDoneButUnacked = new ManualResetEvent(false))
            {
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < 3; i++) NewTask.Publish(i);
                var t1 = new Task(() => { Worker.ConsumeOne(() => { firstDequeueIsDoneButUnacked.Set(); pauseBeforeAck.WaitOne(); }); });
                t1.Start();

                firstDequeueIsDoneButUnacked.WaitOne();

                Worker.ConsumeOne(NoBlock);
                Worker.ConsumeOne(NoBlock);

                pauseBeforeAck.Set();
                t1.Wait();
            }
        }
    }
}
