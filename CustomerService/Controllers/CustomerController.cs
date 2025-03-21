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


        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Customer>> Register(string name, string email)
        {
            var customer = new Customer
            {
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

    }
}
