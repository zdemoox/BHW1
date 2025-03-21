namespace FinancialAccounting.Models
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OperationType Type { get; set; }
        public string Name { get; set; }
    }
}
