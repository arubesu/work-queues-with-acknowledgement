﻿using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
  class NewTask
  {
    static void Main(string[] args)
    {
      var factory = new ConnectionFactory { HostName = "localhost" };

      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(exchange: "",
                            routingKey: "task_queue",
                            basicProperties: properties,
                            body);

        Console.WriteLine($" [X] Sent {message}");

      }
    }

    private static string GetMessage(string[] args) =>
       ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
  }
}