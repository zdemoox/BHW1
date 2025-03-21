using System.Text.Json;
using FinancialAccounting.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinancialAccounting.Visitors
{
    public class JsonExportVisitor : IExportVisitor
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public string Export(IEnumerable<object> items)
        {
            var wrappedItems = items.Select(item =>
            {
                string type = item switch
                {
                    BankAccount => "BankAccount",
                    Category => "Category",
                    Operation => "Operation",
                    _ => throw new NotSupportedException()
                };

                return new
                {
                    type, 
                    Data = item
                };
            });

            return JsonSerializer.Serialize(wrappedItems, _options);
        }

        public string Visit(BankAccount account) =>
            JsonSerializer.Serialize(account, _options);

        public string Visit(Category category) =>
            JsonSerializer.Serialize(category, _options);

        public string Visit(Operation operation) =>
            JsonSerializer.Serialize(operation, _options);
    }
}