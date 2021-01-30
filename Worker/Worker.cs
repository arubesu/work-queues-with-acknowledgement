using System;
using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;

namespace Worker
{
  class Worker
  {
    static void Main(string[] args)
    {
      var factory = new ConnectionFactory { HostName = "localhost" };

      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "task_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, eventArgs) =>
        {
          var body = eventArgs.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);
          Console.WriteLine($" [x] Received {message}");

          var dots = message.Split('.').Length - 1;
          var ms = dots * 1000;
          Thread.Sleep(ms);

          System.Console.WriteLine($" [x] Task Done after {ms} ms");

          channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: "task_queue",
                            autoAck: false,
                            consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
      }
    }
  }
}
