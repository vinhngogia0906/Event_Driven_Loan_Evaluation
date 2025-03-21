namespace LoanApplicationService.Models
{
    public class LoanApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LoanLimit {  get; set; }
        public string Purpose { get; set; }
    }
}
