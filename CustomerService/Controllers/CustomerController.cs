using CustomerService.Models;
using CustomerService.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("customerService")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _customerDbContext;
        private readonly IRabbitMqUtil _rabbitMqUtil;
        public CustomerController(CustomerDbContext customerDbContext, IRabbitMqUtil rabbitMqUtil)
        {
            _customerDbContext = customerDbContext;
            _rabbitMqUtil = rabbitMqUtil;
        }

        [HttpGet]
        [Route("customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _customerDbContext.Customers.ToListAsync();
        }

        [HttpGet]
        [Route("customerLoanApplications")]
        public async Task<ActionResult<IEnumerable<LoanApplication>>>GetCustomerLoanApplications(Guid customerId)
        {
            return await _customerDbContext.LoanApplications
                .Where(l => l.CustomerId == customerId)
                .ToListAsync();
        }

        [HttpGet]
        [Route("getLoanApplications")]
        public async Task<ActionResult<IEnumerable<LoanApplication>>> GetLoanApplications()
        {
            return await _customerDbContext.LoanApplications.ToListAsync();
        }


        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Customer>> Register(string name, string email)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email
            };
            _customerDbContext.Customers.Add(customer);
            await _customerDbContext.SaveChangesAsync();
            var newCustomer = JsonSerializer.Serialize(new Customer
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            });
            await _rabbitMqUtil.PublishMessageQueue("loanEvaluation.customer", newCustomer);
            return CreatedAtAction("GetCustomers", new { customer.Id }, customer);
        }

        [HttpPut]
        [Route("cancelLoanApplication")]
        public async Task<ActionResult<LoanApplication>> CancelLoanApplication(Guid applicationId)
        {
            var application = await _customerDbContext.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == applicationId);
            application.Cancelled = true;
            await _customerDbContext.SaveChangesAsync();
            var loanApplication = JsonSerializer.Serialize(new LoanApplication
            {
                Id = application.Id,
                Name = application.Name,
                LoanLimit = application.LoanLimit,
                Purpose = application.Purpose,
                CustomerId = application.CustomerId,
                Approved = application.Approved,
                Cancelled = application.Cancelled
            });
            await _rabbitMqUtil.PublishMessageQueue("loanEvaluation.customer", loanApplication);
            return CreatedAtAction("GetLoanApplications", new { application.Id }, application);
        }

    }
}
