
using RabbitMQ.Client;

namespace CustomerService.RabbitMq
{
    public class RabbitMqService : BackgroundService
    {
        private readonly IRabbitMqUtil _rabbitMqUtil;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqService(IRabbitMqUtil rabbitMqUtil)
        {
            _rabbitMqUtil = rabbitMqUtil;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "my-rabbit",
                UserName = "admin",
                Password = "WDb3#Je9h4q6l",
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
