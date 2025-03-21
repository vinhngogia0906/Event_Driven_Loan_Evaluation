using RabbitMQ.Client;
using System.Text;

namespace CustomerService.RabbitMq
{
    public class RabbitMqUtil : IRabbitMqUtil
    {
        public async Task PublishMessageQueue(string routingKey, string eventData)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "my-rabbit",
                UserName = "guest",
                Password = "guest"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var body = Encoding.UTF8.GetBytes(eventData);

            channel.BasicPublish(exchange: "topic.exchange",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            await Task.CompletedTask;
        }
    }
}
