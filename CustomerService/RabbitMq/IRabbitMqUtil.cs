using RabbitMQ.Client;

namespace CustomerService.RabbitMq
{
    public interface IRabbitMqUtil
    {
        Task PublishMessageQueue(string routingKey, string eventData);

        Task ListenMessageQueue(IModel channel, string routingKey, CancellationToken cancellationToken);
    }
}
