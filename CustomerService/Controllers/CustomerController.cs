using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("customerService")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _customerDbContext;
        public CustomerController(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        [HttpGet]
        [Route("customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _customerDbContext.Customers.ToListAsync();
        }


        [HttpPost]
        [Route("register")]
        public async Task Register(Customer customer)
        {
            _customerDbContext.Customers.Add(customer);
            await _customerDbContext.SaveChangesAsync();
        }

    }
}
