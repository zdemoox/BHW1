namespace FinancialAccounting.Models
{
    public class Operation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}
