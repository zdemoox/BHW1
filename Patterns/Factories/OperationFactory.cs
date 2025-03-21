using FinancialAccounting.Models;

namespace FinancialAccounting.Patterns.Factories
{
    public static class OperationFactory
    {
        public static Operation Create(
            OperationType type,
            decimal amount,
            Guid bankAccountId,
            Guid categoryId,
            string description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            return new Operation
            {
                Type = type,
                Date = DateTime.UtcNow,
                Amount = amount,
                BankAccountId = bankAccountId,
                CategoryId = categoryId,
                Description = description
            };
        }
    }
}
