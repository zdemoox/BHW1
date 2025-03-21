using FinancialAccounting.Models;
namespace FinancialAccounting.Services
{
    public interface IOperationService
    {
        void AddOperation(Operation operation, BankAccountService accountService);
        IEnumerable<Operation> GetAllOperations();
    }
}
