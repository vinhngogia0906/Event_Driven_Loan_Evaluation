using CustomerService.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerService.RabbitMq
{
    public class RabbitMqUtil : IRabbitMqUtil
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RabbitMqUtil(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task PublishMessageQueue(string routingKey, string eventData)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "my-rabbit",
                UserName = "admin",
                Password = "WDb3#Je9h4q6l"
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
            var customerDbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            var data = JObject.Parse(message);
            var type = ea.RoutingKey;
            if (type == "loanEvaluation.loanApplication")
            {
                var customerGuid = Guid.Parse(data["CustomerId"].Value<string>());

                var loanApplication = await customerDbContext.LoanApplications
                    .FirstOrDefaultAsync(l => l.CustomerId == customerGuid, cancellationToken);

                if (loanApplication != null)
                {
                    loanApplication.Name = data["Name"].Value<string>();
                    loanApplication.LoanLimit = data["LoanLimit"].Value<int>();
                    loanApplication.Purpose = data["Purpose"].Value<string>();
                    loanApplication.Approved = data["Approved"].Value<bool>();
                    loanApplication.Cancelled = data["Cancelled"].Value<bool>();
                    Console.WriteLine("Updated loan application");
                }
                else
                {
                    await customerDbContext.LoanApplications.AddAsync(new LoanApplication
                    {
                        Id = Guid.Parse(data["Id"].Value<string>()),
                        Name = data["Name"].Value<string>(),
                        LoanLimit = data["LoanLimit"].Value<int>(),
                        Purpose = data["Purpose"].Value<string>(),
                        CustomerId = customerGuid,
                        Approved = data["Approved"].Value<bool>()
                    });
                    Console.WriteLine("Created new loan application");
                }
                await customerDbContext.SaveChangesAsync(cancellationToken);

                await Task.Delay(1000, cancellationToken);
            }

        }
    }
}
