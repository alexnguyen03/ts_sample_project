using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServerProject.Models;
using ServerProject.Services;
using System.Text;
using WHProject.Service;
namespace WHProject
{
    class Program
    {
        public readonly IElasticClient _elasticClient;
        public readonly IOrderService orderService;
        private readonly MsdemoContext dbContext = null;
        public Program(IElasticClient elasticClient, MsdemoContext dbContext, IOrderService orderService)
        {
            _elasticClient = elasticClient;
            this.dbContext = dbContext;
            this.orderService = orderService;
        }
        public Program()
        {
        } 
        public void AddProductToES(IConnection connection, ElasticsearchManager elasticsearchManager)
        {
            using var modelConnection = connection.CreateModel();
            modelConnection.QueueDeclare(Channel.PRODUCT.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
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
            modelConnection.BasicConsume(queue: Channel.PRODUCT.ToString(), autoAck: true, consumer: consumer);
            Console.ReadKey();
        }
        public void AddOrder(IConnection connection)
        {
            using var modelConnection = connection.CreateModel();
            modelConnection.QueueDeclare(Channel.ORDER.ToString(), durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(modelConnection);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding
                .UTF8.GetString(body);
                Order order = JsonConvert.DeserializeObject<Order>(message)!;
                //try
                //{
                //    Customer foundCustomer = dbContext.Customers.Find(order.CustomerId)!;
                //    Employee foundEmployee = dbContext.Employees.Find(order.EmployeeId)!;
                //    if (foundEmployee == null)
                //    {
                //        throw new Exception("Employee not found")!;
                //    }
                //    if (foundCustomer == null)
                //    {
                //        throw new Exception("Customer not found")!;
                //    }
                //    order.Customer = foundCustomer;
                //    order.Employee = foundEmployee;
                //    foundCustomer.Orders.Add(order);
                //    dbContext.Orders.Add(order);
                //    dbContext.SaveChanges();
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception("Error occurred while adding the order.", ex)!;
                //}
                orderService.Create(order);
                Console.WriteLine(order);
            };
            modelConnection.BasicConsume(queue: Channel.ORDER.ToString(), autoAck: true, consumer: consumer);
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            Program program = new Program();
            var elasticsearchManager = new ElasticsearchManager("http://localhost:9200");
            Console.WriteLine("Project is running.....");
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            program.AddProductToES(connection, elasticsearchManager);
            program.AddOrder(connection);
            Console.WriteLine("Project is end.....");
        }
    }
}
