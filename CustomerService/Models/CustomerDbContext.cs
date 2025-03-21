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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(c => c.Id)
                .HasDefaultValueSql("hex(randomblob(16))");  // Auto-generate GUIDs
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
