namespace WebhookServer
{
    public class Program
    {
        private static readonly ProductService _productService;


        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();

            using var productChannel = connection.CreateModel();
            productChannel.QueueDeclare("new-product-create", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(productChannel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Product product = JsonConvert.DeserializeObject<Product>(message)!;

                // Use productService to add product to ES or perform other operations
                _productService.AddProductToES(product);

                // Send APIs

                Console.WriteLine(product);
            };

            productChannel.BasicConsume(queue: "new-product-create", autoAck: true, consumer: consumer);

            Console.ReadKey();
        }
    }
}
