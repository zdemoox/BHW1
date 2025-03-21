using FinancialAccounting.Models;
using System.Collections.Generic;
using System.Linq;

namespace FinancialAccounting.Services
{
    public class CategoryService
    {
        private readonly List<Category> _categories = new();

        // Существующие методы
        public Category CreateCategory(OperationType type, string name)
        {
            var category = new Category { Type = type, Name = name };
            _categories.Add(category);
            return category;
        }

        public Category GetCategory(Guid categoryId) =>
            _categories.FirstOrDefault(c => c.Id == categoryId);

        // Добавляем новые методы
        public void CreateCategoryWithId(Guid id, OperationType type, string name)
        {
            _categories.Add(new Category
            {
                Id = id,
                Type = type,
                Name = name
            });
        }
        public void Clear() => _categories.Clear();

        public IEnumerable<Category> GetAllCategories() =>
            _categories.ToList();
    }
}