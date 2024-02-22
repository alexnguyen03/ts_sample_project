using Nest;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

using ServerProject.Models;

using System.Text;

namespace ServerProject.Services
{
    public class RabbitMQManager
    {
        private readonly ConnectionFactory _connectionFactory = new()
        {
            HostName = "localhost"
        };
        private readonly MsdemoContext dbContext;
        private readonly IConnection? _connection;
        private readonly IModel _channel;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger<RabbitMQManager> _logger;


        public RabbitMQManager()
        {
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _orderService = new OrderService(new MsdemoContext());
            this.dbContext = new MsdemoContext();
            _productService = new ProductService(new MsdemoContext(), new ElasticClient());
        }

        public void CreateChannel(
            string exchangeName,
            string exchangeType,
            string queueName,
            string routingKey
        )
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }

        public void SendMessage<T>(T message, string exchangeName, string routingKey)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
        }

        public void ConsumeQueue(string queueName)
        {
            bool isQueueExists = false;
            try
            {
                _channel.QueueDeclarePassive(queueName);

                isQueueExists = true;
            }
            catch (OperationInterruptedException ex)
            {
                Console.WriteLine("Lỗi kết nối với hàng đợi: " + ex.Message);
            }
            if (isQueueExists)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0}", message);
                };
                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }
            else
            {
                Console.WriteLine("Hàng đợi không tồn tại: ");
            }
        }

        public void ConsumeOrderQueue()
        {
            bool isQueueExists = false;
            try
            {
                _channel.QueueDeclarePassive(Channels.ORDER.ToString());
                isQueueExists = true;
            }
            catch (OperationInterruptedException ex)
            {
                // Xử lý ngoại lệ khi hàng đợi không tồn tại
                Console.WriteLine("Lỗi kết nối với hàng đợi: " + ex.Message);
            }
            if (isQueueExists)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0}", message);
                    //! xu li them order vao db
                    Order order = JsonConvert.DeserializeObject<Order>(message)!;
                    _orderService.Create(order);
                };
                _channel.BasicConsume(
                    queue: Channels.ORDER.ToString(),
                    autoAck: true,
                    consumer: consumer
                );
            }
            else
            {
                Console.WriteLine("Hàng đợi không tồn tại: ");
            }
        }

        public void ConsumeProductQueue()
        {
            bool isQueueExists = false;
            try
            {
                _channel.QueueDeclarePassive(Channels.PRODUCT.ToString());
                isQueueExists = true;
            }
            catch (OperationInterruptedException ex)
            {
                // Xử lý ngoại lệ khi hàng đợi không tồn tại
                Console.WriteLine("Lỗi kết nối với hàng đợi: " + ex.Message);
            }
            if (isQueueExists)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0}", message);
                    Product product = JsonConvert.DeserializeObject<Product>(message)!;
                    ProductElastic productElastic = new ProductElastic();
                    productElastic.ProductId = product.ProductId;
                    productElastic.ProductName = product.ProductName;
                    _productService.UpdateDocumentInES("products", productElastic);
                    Console.WriteLine(product);
                };
                _channel.BasicConsume(
                    queue: Channels.PRODUCT.ToString(),
                    autoAck: true,
                    consumer: consumer
                );
            }
            else
            {
                Console.WriteLine("Hàng đợi không tồn tại: ");
                // Ví dụ: tạo hàng đợi, hiển thị thông báo lỗi
            }
        }
    }
}
