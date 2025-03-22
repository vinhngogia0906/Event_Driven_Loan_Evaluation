namespace CustomerService.Models
{
    public class LoanApplication
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LoanLimit { get; set; }
        public string Purpose { get; set; }
        public Guid CustomerId { get; set; }
        public bool Approved { get; set; }
    }
}
