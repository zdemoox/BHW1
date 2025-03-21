using FinancialAccounting.Models;
using FinancialAccounting.Services;

public class OperationService : IOperationService
{
    private readonly List<Operation> _operations = new();

    public void AddOperation(Operation operation, BankAccountService accountService)
    {
        _operations.Add(operation);
        if (operation.Type == OperationType.Income)
        {
            accountService.UpdateBalance(operation.BankAccountId, operation.Amount);
        } 
        else
        {
            accountService.UpdateBalance(operation.BankAccountId, -operation.Amount);
        }
    }

    public void AddOperationWithId(Operation operation, BankAccountService accountService, Guid id)
    {
        operation.Id = id;
        _operations.Add(operation);
        if (operation.Type == OperationType.Income)
        {
            accountService.UpdateBalance(operation.BankAccountId, operation.Amount);
        }
        else
        {
            accountService.UpdateBalance(operation.BankAccountId, -operation.Amount);
        }
    }

    public IEnumerable<Operation> GetAllOperations() => _operations;
    public IEnumerable<Operation> GetOperationsOfAccount(BankAccount acc)
    {
        return _operations
            .Where(_operations => _operations.BankAccountId == acc.Id)
            .ToList();
    }

    public decimal GetBalanceDifference(DateTime startDate, DateTime endDate)
    {
        var periodOperations = _operations
            .Where(op => op.Date >= startDate && op.Date <= endDate)
            .ToList();

        decimal totalIncome = periodOperations
            .Where(op => op.Type == OperationType.Income)
            .Sum(op => op.Amount);

        decimal totalExpense = periodOperations
            .Where(op => op.Type == OperationType.Expense)
            .Sum(op => op.Amount);

        return totalIncome - totalExpense;
    }


    public void Clear() => _operations.Clear();
}