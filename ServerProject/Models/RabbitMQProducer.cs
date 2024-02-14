using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
namespace ServerProject.Models
{
    public class RabbitMQProducer : IMessageProducer
    {
        public void SendMessage<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: queueName, body: body);
        }
    }
}
