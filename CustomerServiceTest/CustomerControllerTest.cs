using CustomerService.Controllers;
using CustomerService.Models;
using CustomerService.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CustomerServiceTest
{
    public class CustomerControllerTests
    {
        private readonly CustomerDbContext _dbContext;
        private readonly Mock<IRabbitMqUtil> _mockRabbitMqUtil;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: "CustomerServiceDatabase")
                .Options;
            _dbContext = new CustomerDbContext(options);
            _mockRabbitMqUtil = new Mock<IRabbitMqUtil>();
            _controller = new CustomerController(_dbContext, _mockRabbitMqUtil.Object);

            // Empty database
            _dbContext.Customers.RemoveRange(_dbContext.Customers);
            _dbContext.LoanApplications.RemoveRange(_dbContext.LoanApplications);
            _dbContext.SaveChanges();

            // Seed the in-memory database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add 2 dummy customers
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), Name = "John Doe", Email = "john.doe@example.com" },
                new Customer { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane.smith@example.com" }
            };
            _dbContext.Customers.AddRange(customers);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetCustomers_ReturnsListOfCustomers()
        {
            // Try to get all customers
            var result = await _controller.GetCustomers();

            // Assert, the total count should be 2
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
            var returnValue = Assert.IsType<List<Customer>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCustomerLoanApplications_ReturnsListOfLoanApplications()
        {
            // Add 2 loan applications for a customer
            var customerId = Guid.NewGuid();
            var loanApplications = new List<LoanApplication>
            {
                new LoanApplication { Id = Guid.NewGuid(), CustomerId = customerId, Name = "Loan 1", LoanLimit = 1000, Purpose = "Purpose 1" },
                new LoanApplication { Id = Guid.NewGuid(), CustomerId = customerId, Name = "Loan 2", LoanLimit = 2000, Purpose = "Purpose 2" }
            };
            _dbContext.LoanApplications.AddRange(loanApplications);
            _dbContext.SaveChanges();

            // Try to get all loan applications for the customer
            var result = await _controller.GetCustomerLoanApplications(customerId);

            // Assert, the total count should be 2
            var actionResult = Assert.IsType<ActionResult<IEnumerable<LoanApplication>>>(result);
            var returnValue = Assert.IsType<List<LoanApplication>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetLoanApplications_ReturnsListOfLoanApplications()
        {
            // Empty database, no loan applications
            _dbContext.LoanApplications.RemoveRange(_dbContext.LoanApplications);
            await _dbContext.SaveChangesAsync();
            // Add 2 loan applications for 2 random customers
            var loanApplications = new List<LoanApplication>
            {
                new LoanApplication { Id = Guid.NewGuid(), Name = "Loan 1", LoanLimit = 1000, Purpose = "Purpose 1" },
                new LoanApplication { Id = Guid.NewGuid(), Name = "Loan 2", LoanLimit = 2000, Purpose = "Purpose 2" }
            };
            _dbContext.LoanApplications.AddRange(loanApplications);
            _dbContext.SaveChanges();

            // Try to query all loan applications
            var result = await _controller.GetLoanApplications();

            // Assert, the total count should be 2
            var actionResult = Assert.IsType<ActionResult<IEnumerable<LoanApplication>>>(result);
            var returnValue = Assert.IsType<List<LoanApplication>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task Register_CreatesNewCustomerAndPublishesMessage()
        {
            // Create a new customer and push a event to rabbitmq
            var name = "John Doe";
            var email = "john.doe@example.com";
            _mockRabbitMqUtil.Setup(r => r.PublishMessageQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Submit the register
            var result = await _controller.Register(name, email);

            // Assert, verify the push message to see if it is the same as the customer
            var actionResult = Assert.IsType<ActionResult<Customer>>(result);
            var returnValue = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdCustomer = Assert.IsType<Customer>(returnValue.Value);
            Assert.Equal(name, createdCustomer.Name);
            Assert.Equal(email, createdCustomer.Email);
            _mockRabbitMqUtil.Verify(r => r.PublishMessageQueue("loanEvaluation.customer", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CancelLoanApplication_CancelsLoanApplicationAndPublishesMessage()
        {
            // Create a loan application and push a event to rabbitmq
            var applicationId = Guid.NewGuid();
            var application = new LoanApplication { Id = applicationId, Name = "Loan 1", LoanLimit = 1000, Purpose = "Purpose 1", CustomerId = Guid.NewGuid(), Approved = true, Cancelled = false };
            _dbContext.LoanApplications.Add(application);
            _dbContext.SaveChanges();
            _mockRabbitMqUtil.Setup(r => r.PublishMessageQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Cancel the loan application with application Id
            var result = await _controller.CancelLoanApplication(applicationId);

            // Assert, query the application and check the cancelled status
            var actionResult = Assert.IsType<ActionResult<LoanApplication>>(result);
            var returnValue = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var cancelledApplication = Assert.IsType<LoanApplication>(returnValue.Value);
            Assert.True(cancelledApplication.Cancelled);
            _mockRabbitMqUtil.Verify(r => r.PublishMessageQueue("loanEvaluation.customer", It.IsAny<string>()), Times.Once);
        }
    }
}