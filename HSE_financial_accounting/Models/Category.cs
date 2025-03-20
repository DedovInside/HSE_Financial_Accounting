using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Models
{
    public enum CategoryType
    {
        Income,
        Expense
    }
    
    public class Category : ICategory
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; private set; }
        public CategoryType Type { get; }

        public Category(string name, CategoryType type)
        {
            Name = name;
            Type = type;
        }

        // Конструктор для импорта
        public Category(Guid id, string name, CategoryType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty");
            }

            Name = name;
        }
    }
}