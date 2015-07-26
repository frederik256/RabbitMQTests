using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace rtest
{

    class Worker
    {
        public static void Consume()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("task_queue", true, false, false, null);

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("task_queue", false, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine(" [x] Acked " + ea.DeliveryTag);
                }
            }
        }

        public static void ConsumeOne(Action blockAfterDequeue)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("task_queue", true, false, false, null);

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("task_queue", false, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    blockAfterDequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;

                    
                    channel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine(" [x] Acked msg contents " + message);
                }
            }
        }
    }
}
