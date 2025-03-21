using FinancialAccounting.Models;
using FinancialAccounting.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FinancialAccounting.Patterns.TemplateMethod
{
    public class CsvImporter : DataImporter
    {
        private readonly BankAccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly OperationService _operationService;

        public CsvImporter(
            BankAccountService accountService,
            CategoryService categoryService,
            OperationService operationService)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _operationService = operationService;
        }

        protected override void ParseData(string rawData)
        {
            var tempAccounts = new List<BankAccount>();
            var tempCategories = new List<Category>();
            var tempOperations = new List<Operation>();

            var lines = rawData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return;

            for (int i = 1; i < lines.Length; i++)
            {
                var fields = ParseCsvLine(lines[i]);
                if (fields.Length < 2) continue;

                try
                {
                    switch (fields[0])
                    {
                        case "BankAccount" when fields.Length >= 4:
                            tempAccounts.Add(new BankAccount
                            {
                                Id = Guid.Parse(fields[1]),
                                Name = UnescapeCsvField(fields[2]),
                                Balance = decimal.Parse(fields[3], CultureInfo.InvariantCulture)
                            });
                            break;

                        case "Category" when fields.Length >= 4:
                            tempCategories.Add(new Category
                            {
                                Id = Guid.Parse(fields[1]),
                                Name = UnescapeCsvField(fields[2]),
                                Type = (OperationType)int.Parse(fields[3])
                            });
                            break;

                        case "Operation" when fields.Length >= 8:
                            tempOperations.Add(new Operation
                            {
                                Id = Guid.Parse(fields[1]),
                                Type = (OperationType)int.Parse(fields[2]),
                                BankAccountId = Guid.Parse(fields[3]),
                                Amount = decimal.Parse(fields[4], CultureInfo.InvariantCulture),
                                Date = DateTime.Parse(fields[5], CultureInfo.InvariantCulture),
                                Description = UnescapeCsvField(fields[6]),
                                CategoryId = Guid.Parse(fields[7])
                            });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка парсинга строки {i}: {ex.Message}");
                }
            }

            _accountService.Clear();
            _categoryService.Clear();
            _operationService.Clear();

            foreach (var account in tempAccounts)
            {
                _accountService.CreateAccountWithId(account.Id, account.Name);
                var acc = _accountService.GetAccount(account.Id);
                if (acc != null) acc.Balance = account.Balance;
            }

            foreach (var category in tempCategories)
            {
                _categoryService.CreateCategoryWithId(category.Id, category.Type, category.Name);
            }

            foreach (var operation in tempOperations)
            {
                _operationService.AddOperationWithId(operation, _accountService, operation.Id);
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            fields.Add(currentField.ToString());
            return fields.ToArray();
        }

        private string UnescapeCsvField(string field)
        {
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field[1..^1];
                field = field.Replace("\"\"", "\"");
            }
            return field.Trim();
        }
    }
}
