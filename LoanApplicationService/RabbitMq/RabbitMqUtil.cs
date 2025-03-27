using LoanApplicationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LoanApplicationService.RabbitMq
{
    public class RabbitMqUtil : IRabbitMqUtil
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        public RabbitMqUtil(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }
        public async Task PublishMessageQueue(string routingKey, string eventData)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQConnection:Hostname"),
                UserName = _configuration.GetValue<string>("RabbitMQConnection:UserName"),
                Password = _configuration.GetValue<string>("RabbitMQConnection:Password"),
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

        public async Task ListenMessageQueue(IModel channel, string routingKey, CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                // Parse the message
                await ParseLoanApplicationMessage(body, ea, cancellationToken);
            };
            channel.BasicConsume(queue: routingKey,
                                 autoAck: true,
                                 consumer: consumer);
            await Task.CompletedTask;
        }

        private async Task ParseLoanApplicationMessage(string message, BasicDeliverEventArgs ea,
            CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var loanApplicationDbContext = scope.ServiceProvider.GetRequiredService<LoanApplicationDbContext>();

            var data = JObject.Parse(message);
            var type = ea.RoutingKey;
            Console.WriteLine($"Got the message: {message}");
            if (type == "loanEvaluation.customer")
            {
                var applicationGuid = Guid.Parse(data["Id"].Value<string>());

                var loanApplication = await loanApplicationDbContext.LoanApplications
                    .FirstOrDefaultAsync(l => l.Id == applicationGuid, cancellationToken);

                if (loanApplication != null)
                {
                    loanApplication.Name = data["Name"].Value<string>();
                    loanApplication.LoanLimit = data["LoanLimit"].Value<int>();
                    loanApplication.Purpose = data["Purpose"].Value<string>();
                    loanApplication.Approved = data["Approved"].Value<bool>();
                    loanApplication.Cancelled = data["Cancelled"].Value<bool>();
                    Console.WriteLine("Cancelled loan application");
                }
                await loanApplicationDbContext.SaveChangesAsync(cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }

        }
    }
}
