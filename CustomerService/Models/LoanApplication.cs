namespace CustomerService.Models
{
    public class LoanApplication
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int limit {  get; set; }
        public string purpose { get; set; }
    }
}
