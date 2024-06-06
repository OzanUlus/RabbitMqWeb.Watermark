using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private  IConnection _connection;
        private  IModel _chanel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueNAme = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            Connect();
        }
        public IModel Connect() 
        {
          _connection = _connectionFactory.CreateConnection();
            if(_chanel is { IsOpen: true }) 
            {
             return _chanel;
            }
            _chanel = _connection.CreateModel();
            _chanel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _chanel.QueueDeclare(QueueNAme ,true,false,false,null);

            _chanel.QueueBind(exchange: ExchangeName, queue: QueueNAme, routingKey: RoutingWatermark);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");


            return _chanel;

        }

        public void Dispose()
        {
            _chanel?.Close();
            _chanel?.Dispose();
            
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
            
        }
    }
}
