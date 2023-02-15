using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "letterbox",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
    );

var consumer = new EventingBasicConsumer(channel);

var random = new Random();

consumer.Received += (model, ea) =>
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received: {message} will take {processingTime} to process");
    Task.Delay(TimeSpan.FromSeconds(processingTime));
};

channel.BasicConsume(queue: "letterbox", 
                    autoAck: false, // means we need to manually acknowledge the queue that message received/processed
                    consumer: consumer);

Console.ReadKey();