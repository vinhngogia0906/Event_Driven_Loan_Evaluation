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

        public DbSet<LoanApplication> LoanApplications { get; set; }
    }
}
