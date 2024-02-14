using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using ServerProject.Models;

using System.Text;
//private static    readonly ProductService _productService = null;
var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
using var PruductChannel = connection.CreateModel();

PruductChannel.QueueDeclare("new-product-create", durable: false, exclusive: false, autoDelete: false, arguments: null);
var consumer = new EventingBasicConsumer(PruductChannel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Product product = JsonConvert.DeserializeObject<Product>(message)!;
    //AddProductToES
    //Send apis
    Console.WriteLine(product);
};
PruductChannel.BasicConsume(queue: "new-product-create", autoAck: true, consumer: consumer);
Console.ReadKey();