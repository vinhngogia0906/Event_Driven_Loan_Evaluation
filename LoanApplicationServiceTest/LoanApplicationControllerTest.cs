using LoanApplicationService.Controllers;
using LoanApplicationService.Models;
using LoanApplicationService.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanApplicationServiceTest
{
    public class LoanApplicationControllerTests
    {
        private readonly LoanApplicationDbContext _dbContext;
        private readonly Mock<IRabbitMqUtil> _mockRabbitMqUtil;
        private readonly LoanApplicationController _controller;

        public LoanApplicationControllerTests()
        {
            var options = new DbContextOptionsBuilder<LoanApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "LoanApplicationServiceDatabase")
                .Options;
            _dbContext = new LoanApplicationDbContext(options);
            _mockRabbitMqUtil = new Mock<IRabbitMqUtil>();
            _controller = new LoanApplicationController(_dbContext, _mockRabbitMqUtil.Object);

            _dbContext.LoanApplications.RemoveRange(_dbContext.LoanApplications);
            _dbContext.SaveChanges();
            // Seed the in-memory database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var loanApplications = new List<LoanApplication>
            {
                new LoanApplication { Id = Guid.NewGuid(), Name = "Loan 1", LoanLimit = 1000, Purpose = "Purpose 1", CustomerId = Guid.NewGuid(), Approved = false, Cancelled = false },
                new LoanApplication { Id = Guid.NewGuid(), Name = "Loan 2", LoanLimit = 2000, Purpose = "Purpose 2", CustomerId = Guid.NewGuid(), Approved = false, Cancelled = false }
            };
            _dbContext.LoanApplications.AddRange(loanApplications);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetLoanApplications_ReturnsListOfLoanApplications()
        {
            // Query all loan applications
            var result = await _controller.GetLoanApplications();

            // Assert, the total count should be 2
            var actionResult = Assert.IsType<ActionResult<IEnumerable<LoanApplication>>>(result);
            var returnValue = Assert.IsType<List<LoanApplication>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task UpdateLoanApplication_UpdatesApprovalStatusAndPublishesMessage()
        {
            // Create a new loan application and push the event to rabbitmq
            var applicationId = Guid.NewGuid();
            var application = new LoanApplication { Id = applicationId, Name = "Loan 1", LoanLimit = 1000, Purpose = "Purpose 1", CustomerId = Guid.NewGuid(), Approved = false, Cancelled = false };
            _dbContext.LoanApplications.Add(application);
            _dbContext.SaveChanges();
            _mockRabbitMqUtil.Setup(r => r.PublishMessageQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Update the approval status of the loan application
            var result = await _controller.UpdateLoanApplicationApproval(applicationId, true);

            // Vewrify the approval status and the message pushed to rabbitmq
            var actionResult = Assert.IsType<ActionResult<LoanApplication>>(result);
            var returnValue = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var updatedApplication = Assert.IsType<LoanApplication>(returnValue.Value);
            Assert.True(updatedApplication.Approved);
            _mockRabbitMqUtil.Verify(r => r.PublishMessageQueue("loanEvaluation.loanApplication", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SubmitLoanApplication_CreatesNewLoanApplicationAndPublishesMessage()
        {
            // Create a new loan application and push the event to rabbitmq
            var name = "Loan 1";
            var limit = 1000;
            var purpose = "Purpose 1";
            var customerId = Guid.NewGuid();
            _mockRabbitMqUtil.Setup(r => r.PublishMessageQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Submit the loan application
            var result = await _controller.SubmitLoanApplication(name, limit, purpose, customerId);

            // Verify the message to see if it is the same as the loan application
            var actionResult = Assert.IsType<ActionResult<LoanApplication>>(result);
            var returnValue = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdApplication = Assert.IsType<LoanApplication>(returnValue.Value);
            Assert.Equal(name, createdApplication.Name);
            Assert.Equal(limit, createdApplication.LoanLimit);
            Assert.Equal(purpose, createdApplication.Purpose);
            Assert.Equal(customerId, createdApplication.CustomerId);
            _mockRabbitMqUtil.Verify(r => r.PublishMessageQueue("loanEvaluation.loanApplication", It.IsAny<string>()), Times.Once);
        }
    }
}
