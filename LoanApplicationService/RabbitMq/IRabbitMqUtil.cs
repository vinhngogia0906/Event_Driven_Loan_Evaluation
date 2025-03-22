using RabbitMQ.Client;

namespace LoanApplicationService.RabbitMq
{
    public interface IRabbitMqUtil
    {
        Task PublishMessageQueue(string routingKey, string eventData);

        Task ListenMessageQueue(IModel channel, string routingKey, CancellationToken cancellationToken);
    }
}
