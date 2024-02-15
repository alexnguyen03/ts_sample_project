using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

using ServerProject.Models;
using ServerProject.Services;

using System.Text;

using WHProject.Service;
namespace WHProject
{
    class Program
    {
        //public readonly IElasticClient _elasticClient;
        //private readonly IOrderService orderService;
        private readonly MsdemoContext dbContext = null;
        public Program(
            //IElasticClient elasticClient,
            //MsdemoContext dbContext,
            //IOrderService orderService
            //RabbitMQManager rabbitMQManager
            )
        {
            //_elasticClient = _elasticClient;
            //this.dbContext = dbContext;
            //this.orderService = orderService;
        }
        public void AddProductToES(IConnection connection, ElasticsearchManager elasticsearchManager)
        {
            using var modelConnection = connection.CreateModel();
            modelConnection.QueueDeclare(Channels.PRODUCT.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
            modelConnection.ExchangeDeclare("messages", ExchangeType.Fanout);
            //modelConnection.QueueBind(Channel.PRODUCT.ToString(), "messages", "");
            var consumer = new EventingBasicConsumer(modelConnection);
            consumer.Received += (modelConnection, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding
                .UTF8.GetString(body);
                Product product = JsonConvert.DeserializeObject<Product>(message)!;
                ProductElastic productElastic = new ProductElastic();
                productElastic.ProductId = product.ProductId;
                productElastic.ProductName = product.ProductName;
                elasticsearchManager.AddDocument("products", productElastic);
                Console.WriteLine(product);
            };
            modelConnection.BasicConsume(queue: Channels.PRODUCT.ToString(), autoAck: true, consumer: consumer);
        }
        public static void Consumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            //Create the RabbitMQ connection using connection factory details as i mentioned above
            var connection = factory.CreateConnection();
            //Here we create channel with session and model
            using
            var channel = connection.CreateModel();
            bool isQueueExists = false;
            try
            {
                channel.QueueDeclarePassive(Channels.ORDER.ToString()); // Tên hàng đợi cần kiểm tra
                isQueueExists = true;
            }
            catch (OperationInterruptedException ex)
            {
                // Xử lý ngoại lệ khi hàng đợi không tồn tại
                Console.WriteLine("Hàng đợi không tồn tại: " + ex.Message);
            }
            //declare the queue after mentioning name and a few property related to that
            //channel.QueueDeclare("ORDER", exclusive: false);
            //Set Event object which listen message from chanel which is sent by producer
            if (isQueueExists)
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Product message received: {message}");
                };
                //read the message
                channel.BasicConsume(queue: Channels.ORDER.ToString(), autoAck: true, consumer: consumer);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Queue was not declared!!!");
            }
        }
        static void Main(string[] args)
        {
            var rabbitMQManager = new RabbitMQManager();
            rabbitMQManager.ConsumeOrderQueue();
            rabbitMQManager.ConsumeProductQueue();
            //rabbitMQManager.ConsumeQueue("ORDER");
            Console.ReadKey();
        }
    }
}
