using Microsoft.EntityFrameworkCore;

namespace CustomerService.Models
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext()
        {
            
        }
        public CustomerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
    }
}
