using FinancialAccounting.Models;
using FinancialAccounting.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace FinancialAccounting.Patterns.TemplateMethod
{
    public class JsonImporter : DataImporter
    {
        private readonly BankAccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly OperationService _operationService;
        private readonly JsonSerializerOptions _options;

        public JsonImporter(
            BankAccountService accountService,
            CategoryService categoryService,
            OperationService operationService)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _operationService = operationService;
            _options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
        }

        protected override void ParseData(string rawData)
        {
            var tempAccounts = new List<BankAccount>();
            var tempCategories = new List<Category>();
            var tempOperations = new List<Operation>();

            try
            {
                var importedData = JsonSerializer.Deserialize<List<JsonElement>>(rawData, _options);

                foreach (var element in importedData)
                {
                    if (element.TryGetProperty("type", out var typeProperty))
                    {
                        var typeName = typeProperty.GetString();
                        var dataProperty = element.GetProperty("Data");

                        switch (typeName)
                        {
                            case "BankAccount":
                                var account = JsonSerializer.Deserialize<BankAccount>(dataProperty.GetRawText(), _options);
                                tempAccounts.Add(account);
                                break;

                            case "Category":
                                var category = JsonSerializer.Deserialize<Category>(dataProperty.GetRawText(), _options);
                                tempCategories.Add(category);
                                break;

                            case "Operation":
                                var operation = JsonSerializer.Deserialize<Operation>(dataProperty.GetRawText(), _options);
                                tempOperations.Add(operation);
                                break;
                        }
                    }
                }

                // Очищаем сервисы только после успешного парсинга
                _accountService.Clear();
                _categoryService.Clear();
                _operationService.Clear();

                // Восстанавливаем счета
                foreach (var account in tempAccounts)
                {
                    _accountService.CreateAccountWithId(account.Id, account.Name);
                    var acc = _accountService.GetAccount(account.Id);
                    if (acc != null) acc.Balance = account.Balance; // Обновляем баланс
                }

                // Восстанавливаем категории
                foreach (var category in tempCategories)
                {
                    _categoryService.CreateCategoryWithId(category.Id, category.Type, category.Name);
                }

                // Восстанавливаем операции
                foreach (var operation in tempOperations)
                {
                    _operationService.AddOperationWithId(operation, _accountService, operation.Id);
                }
            }
            catch (Exception ex)
            {
                // Откат изменений при ошибке
                _accountService.Clear();
                _categoryService.Clear();
                _operationService.Clear();
                Console.WriteLine($"Ошибка импорта: {ex.Message}");
                throw;
            }
        }
    }
}
