using Microsoft.EntityFrameworkCore;

namespace LoanApplicationService.Models
{
    public class LoanApplicationDbContext : DbContext
    {
        public LoanApplicationDbContext()
        {
            
        }
        public LoanApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoanApplication>()
                .Property(c => c.Id)
                .HasDefaultValueSql("hex(randomblob(16))");  // Auto-generate GUIDs
        }
        public DbSet<LoanApplication> LoanApplications { get; set; }
    }
}
