namespace LoanApplicationService.RabbitMq
{
    public interface IRabbitMqUtil
    {
        Task PublishMessageQueue(string routingKey, string eventData);
    }
}
