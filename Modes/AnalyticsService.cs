using FinancialAccounting.Models;
using FinancialAccounting.Services;

public class AnalyticsService
{
    private readonly List<Operation> _operations;
    private readonly CategoryService _categoryService;

    public AnalyticsService(List<Operation> operations, CategoryService categoryService)
    {
        _operations = operations ?? new List<Operation>();
        _categoryService = categoryService;
    }

    public decimal GetBalanceDifference(DateTime startDate, DateTime endDate)
    {
        var filtered = _operations
            .Where(op => op.Date >= startDate && op.Date <= endDate)
            .ToList(); 

        decimal totalIncome = filtered
            .Where(op => op.Type == OperationType.Income)
            .Sum(op => op.Amount);

        decimal totalExpense = filtered
            .Where(op => op.Type == OperationType.Expense)
            .Sum(op => op.Amount);

        Console.WriteLine($"Доходы: {totalIncome}, Расходы: {totalExpense}");

        return totalIncome - totalExpense;
    }

    public Dictionary<string, decimal> GroupByCategories(OperationType type)
{
    var grouped = _operations
        .Where(op => op.Type == type)
        .GroupBy(op =>
        {
            var category = _categoryService.GetCategory(op.CategoryId);
            Console.WriteLine($"Категория ID: {op.CategoryId} -> {category?.Name ?? "Неизвестно"} " +
                $"-> {op?.Amount ?? 0}");
            return category?.Name ?? "Без категории";
        })
        .ToDictionary(
            g => g.Key,
            g => g.Sum(op => op.Amount)
        );
    return grouped;
}
}
