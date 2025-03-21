using FinancialAccounting.Models;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Visitors
{
    public class CsvExportVisitor : IExportVisitor
    {
        public string Visit(BankAccount account)
        {
            return string.Join(",",
                "BankAccount",
                account.Id,
                EscapeCsvField(account.Name),
                account.Balance
            );
        }

        public string Visit(Category category)
        {
            return string.Join(",",
                "Category",
                category.Id,
                EscapeCsvField(category.Name),
                (int)category.Type
            );
        }

        public string Visit(Operation operation)
        {
            return string.Join(",",
                "Operation",
                operation.Id,
                (int)operation.Type,
                operation.BankAccountId,
                operation.Amount,
                operation.Date.ToString("o"),
                EscapeCsvField(operation.Description),
                operation.CategoryId
            );
        }

        public string Export(IEnumerable<object> items)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Type,Id,Name,Balance,OperationType,BankAccountId,Amount,Date,Description,CategoryId");

            foreach (var item in items)
            {
                switch (item)
                {
                    case BankAccount account:
                        csv.AppendLine(Visit(account));
                        break;
                    case Category category:
                        csv.AppendLine(Visit(category));
                        break;
                    case Operation operation:
                        csv.AppendLine(Visit(operation));
                        break;
                }
            }

            return csv.ToString();
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                return $"\"{field.Replace("\"", "\"\"")}\"";
            return field;
        }
    }
}
