
using RabbitMQ.Client;

namespace CustomerService.RabbitMq
{
    public class RabbitMqService : BackgroundService
    {
        private readonly IRabbitMqUtil _rabbitMqUtil;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqService(IRabbitMqUtil rabbitMqUtil, IConfiguration configuration)
        {
            _rabbitMqUtil = rabbitMqUtil;
            _configuration = configuration;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQConnection:Hostname"),
                UserName = _configuration.GetValue<string>("RabbitMQConnection:UserName"),
                Password = _configuration.GetValue<string>("RabbitMQConnection:Password"),
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMqUtil.ListenMessageQueue(_channel, "loanEvaluation.loanApplication", stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
        }
    }
}
