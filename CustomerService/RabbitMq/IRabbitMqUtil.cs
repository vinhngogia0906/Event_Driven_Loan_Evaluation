namespace CustomerService.RabbitMq
{
    public interface IRabbitMqUtil
    {
        Task PublishMessageQueue(string routingKey, string eventData);
    }
}
