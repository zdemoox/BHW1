using FinancialAccounting.Models;

namespace FinancialAccounting.Visitors
{
    public interface IExportVisitor
    {
        string Visit(BankAccount account);
        string Visit(Category category);
        string Visit(Operation operation);
        string Export(IEnumerable<object> items);
    }
}
