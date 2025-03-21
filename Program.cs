using FinancialAccounting.Models;
using FinancialAccounting.Services;
using FinancialAccounting.Commands.Decorators;
using FinancialAccounting.Patterns.Factories;
using FinancialAccounting.Patterns.TemplateMethod;
using FinancialAccounting.Visitors;
using System.Text;

namespace FinancialAccounting
{
    class Program
    {
        static void Main()
        {
            // Инициализация сервисов
            var accountService = new BankAccountService();
            var categoryService = new CategoryService();
            var operationService = new OperationService();

            // Создание тестовых данных
            var mainAccount = accountService.CreateAccount("Основной счет");
            var subAccount = accountService.CreateAccount("Побочный счет");

            var accounts = new List<BankAccount> { mainAccount, subAccount };

            var salaryCategory = categoryService.CreateCategory(OperationType.Income, "Зарплата");
            var foodCategory = categoryService.CreateCategory(OperationType.Expense, "Продукты");
            var theatreCategory = categoryService.CreateCategory(OperationType.Expense, "Театр");
            var transportCategory = categoryService.CreateCategory(OperationType.Expense, "Транспорт");
            var cashbackCategory = categoryService.CreateCategory(OperationType.Income, "Кэшбэк");
            var flowersCategory = categoryService.CreateCategory(OperationType.Expense, "Цветы");

            var categories = new List<Category> { salaryCategory,foodCategory,theatreCategory,
                transportCategory,cashbackCategory,flowersCategory};

            // Создание операций через фабрику
            var operations = new List<Operation>
            {
                OperationFactory.Create(OperationType.Income, 50000, mainAccount.Id, salaryCategory.Id, "Зарплата за март"),
                OperationFactory.Create(OperationType.Expense, 15000, mainAccount.Id, foodCategory.Id, "Пятёрочка"),
                OperationFactory.Create(OperationType.Expense, 5000, mainAccount.Id, foodCategory.Id, "Магнит"),
                OperationFactory.Create(OperationType.Income, 14000, subAccount.Id, salaryCategory.Id, "Подработка за март"),
                OperationFactory.Create(OperationType.Income, 1500, subAccount.Id, cashbackCategory.Id, "Кэшбэк за февраль"),
                OperationFactory.Create(OperationType.Expense, 1449, subAccount.Id, foodCategory.Id, "Социальная карта метро"),
                OperationFactory.Create(OperationType.Expense, 2000, subAccount.Id, theatreCategory.Id, "Театр"),
                OperationFactory.Create(OperationType.Expense, 3000, subAccount.Id, foodCategory.Id, "Цветы")
            };

            // Добавление операций через команды с замером времени
            Console.WriteLine("Добавление операций...");
            foreach (var operation in operations)
            {
                var command = new AddOperationCommand(operationService, operation, accountService);
                var timedCommand = new TimingCommandDecorator(command);

                timedCommand.Execute();
                Console.WriteLine($"Операция \"{operation.Description}\"  |  {operation.Type}  |  " +
                    $"размером {operation.Amount}   |   " +
                    $"добавлена за {timedCommand.ExecutionTime.TotalMilliseconds} мс");
            }

            Console.WriteLine($"\nТекущий баланс '{mainAccount.Name}': {mainAccount.Balance}");
            Console.WriteLine($"\nТекущий баланс '{subAccount.Name}': {subAccount.Balance}");

            Console.WriteLine($"\nПочему так?");
            // Вывод всех категорий
            Console.WriteLine("\nСписок категорий:");
            foreach (var category in categoryService.GetAllCategories())
            {
                Console.WriteLine($"{category.Name} ({category.Type})");
            }

            // Вывод всех операций
            Console.WriteLine("\nСписок операций:");
            foreach (var op in operationService.GetAllOperations())
            {
                var category = categoryService.GetCategory(op.CategoryId);
                Console.WriteLine($"{((op.BankAccountId == mainAccount.Id) ? mainAccount.Name : subAccount.Name) } |" +
                    $" {op.Date:dd.MM.yyyy} | {category?.Name} | {op.Amount} | {op.Type}");
            }

            // Аналитика
            Console.WriteLine($"\nПроведём аналитику.");
            var periodStart = new DateTime(2025, 03, 01);
            var periodEnd = new DateTime(2025, 03, 31);

            // После добавления операций проверяем данные
            Console.WriteLine("\nПроверка суммарных данных:");
            Console.WriteLine($"Счетов: {accountService.GetAllAccounts().Count()}");
            Console.WriteLine($"Категорий: {categoryService.GetAllCategories().Count()}");
            Console.WriteLine($"Операций: {operationService.GetAllOperations().Count()}");
            // Разница доходов/расходов для главного счёта
            var analyticsMain = new AnalyticsService(
    operationService.GetOperationsOfAccount(mainAccount).ToList(),
    categoryService);

            Console.WriteLine("\nАналитика за март:");
            Console.WriteLine($"Разница доходов/расходов основного счёта: " +
                $"{analyticsMain.GetBalanceDifference(periodStart, periodEnd)}");

            // Разница доходов/расходов для побочного счёта
            var analyticsSub = new AnalyticsService(
    operationService.GetOperationsOfAccount(subAccount).ToList(),
    categoryService);

            Console.WriteLine($"Разница доходов/расходов побочного счёта: " +
                $"{analyticsSub.GetBalanceDifference(periodStart, periodEnd)}");

            // Расходы по категориям для главного счёта
            Console.WriteLine("\nРасходы по категориям основного счёта:");
            var expensesMain = analyticsMain.GroupByCategories(OperationType.Expense);
            foreach (var (category, amount) in expensesMain)
            {
                Console.WriteLine($"{category}: {amount}");
            }

            // Расходы по категориям для побочного счёта
            Console.WriteLine("\nРасходы по категориям побочного счёта:");
            var expensesSub = analyticsSub.GroupByCategories(OperationType.Expense);
            foreach (var (category, amount) in expensesSub)
            {
                Console.WriteLine($"{category}: {amount}");
            }
            // Доходы по категориям для главного счёта
            Console.WriteLine("\nДоходы по категориям основного счёта:");
            var incomesMain = analyticsMain.GroupByCategories(OperationType.Income);
            foreach (var (category, amount) in incomesMain)
            {
                Console.WriteLine($"{category}: {amount}");
            }
            // Доходы по категориям для побочного счёта
            Console.WriteLine("\nДоходы по категориям побочного счёта:");
            var incomesSub = analyticsSub.GroupByCategories(OperationType.Income);
            foreach (var (category, amount) in incomesSub)
            {
                Console.WriteLine($"{category}: {amount}");
            }

            // Экспорт данных в JSON
            var exporter = new JsonExportVisitor();
            var exportData = new List<object> {};
            exportData.AddRange(operations);
            exportData.AddRange(categories);
            exportData.AddRange(accounts);


            try
            {
                var json = exporter.Export(exportData);
                File.WriteAllText("export.json", json);
                Console.WriteLine("\nДанные успешно экспортированы в export.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка экспорта: {ex.Message}");
            }

            // А теперь примем из JSON обратно

            try
            {
                var importer = new JsonImporter(
                    accountService,
                    categoryService,
                    operationService
                );

                importer.ImportData("export.json");
                Console.WriteLine("\nДанные успешно импортированы из export.json");

                // Вывод всех импортированных данных
                Console.WriteLine("\nИмпортированные счета:");
                foreach (var account in accountService.GetAllAccounts())
                {
                    Console.WriteLine($"- {account.Name}: {account.Balance}");
                }

                Console.WriteLine("\nИмпортированные категории:");
                foreach (var category in categoryService.GetAllCategories())
                {
                    Console.WriteLine($"- {category.Name} ({category.Type})");
                }

                Console.WriteLine("\nИмпортированные операции:");
                foreach (var op in operationService.GetAllOperations())
                {
                    var category = categoryService.GetCategory(op.CategoryId);
                    var account = accountService.GetAccount(op.BankAccountId);
                    Console.WriteLine($"[{op.Date:dd.MM.yyyy}] {op.Type}: " +
                        $"{category?.Name ?? "Без категории"} -> " +
                        $"{account?.Name ?? "Счет не найден"}: {op.Amount}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("\nОшибка: файл export.json не найден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка импорта: {ex.Message}");
            }

            // Общая статистика для JSON (проверяем, что кол-во осталось то же)
            Console.WriteLine("\nИтоги импорта:");
            Console.WriteLine($"Счетов: {accountService.GetAllAccounts().Count()}");
            Console.WriteLine($"Категорий: {categoryService.GetAllCategories().Count()}");
            Console.WriteLine($"Операций: {operationService.GetAllOperations().Count()}");

            // А теперь выведем всё в CSV и примем обратно.

            Console.WriteLine("\nА теперь выведем в csv формате.");

            var csvExporter = new CsvExportVisitor();
            var csvData = csvExporter.Export(exportData);
            File.WriteAllText("export.csv", csvData, Encoding.UTF8);
            Console.WriteLine("\nДанные успешно экспортированы в export.csv");

            try
            {
                var csvImporter = new CsvImporter(accountService, categoryService, operationService);
                csvImporter.ImportData("export.csv");
                Console.WriteLine("\nДанные успешно импортированы из export.csv");

                // Вывод всех импортированных данных
                Console.WriteLine("\nИмпортированные счета:");
                foreach (var account in accountService.GetAllAccounts())
                {
                    Console.WriteLine($"- {account.Name}: {account.Balance}");
                }

                Console.WriteLine("\nИмпортированные категории:");
                foreach (var category in categoryService.GetAllCategories())
                {
                    Console.WriteLine($"- {category.Name} ({category.Type})");
                }

                Console.WriteLine("\nИмпортированные операции:");
                foreach (var op in operationService.GetAllOperations())
                {
                    var category = categoryService.GetCategory(op.CategoryId);
                    var account = accountService.GetAccount(op.BankAccountId);
                    Console.WriteLine($"[{op.Date:dd.MM.yyyy}] {op.Type}: " +
                        $"{category?.Name ?? "Без категории"} -> " +
                        $"{account?.Name ?? "Счет не найден"}: {op.Amount}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("\nОшибка: файл export.json не найден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка импорта: {ex.Message}");
            }

            // Общая статистика для CSV (проверяем, что кол-во осталось то же)
            Console.WriteLine("\nИтоги импорта:");
            Console.WriteLine($"Счетов: {accountService.GetAllAccounts().Count()}");
            Console.WriteLine($"Категорий: {categoryService.GetAllCategories().Count()}");
            Console.WriteLine($"Операций: {operationService.GetAllOperations().Count()}");
            // Пример обработки ошибок валидации
            try
            {
                Console.WriteLine("А теперь попытаемся создать объект с отрицательным значением операции. " +
                    "P.S. в коде считается, что ");
                var invalidOperation = OperationFactory.Create(
                    OperationType.Expense,
                    -100,
                    mainAccount.Id,
                    salaryCategory.Id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nОшибка валидации: {ex.Message}");
            }
        }
    }
}