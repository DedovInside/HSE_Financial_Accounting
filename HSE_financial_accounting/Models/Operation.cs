using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Models
{
    public enum OperationType
    {
        Income,
        Expense
    }

    public class Operation : IOperation
    {
        public Guid Id { get; }
        public OperationType Type { get; }
        public Guid BankAccountId { get; }
        public decimal Amount { get; }
        public DateTime Date { get; }
        public string Description { get; private set; }
        public Guid CategoryId { get; }
        
        public Operation(OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId)
        {
            Id = Guid.NewGuid();
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            Description = description;
            CategoryId = categoryId;
        }

        // Конструктор для импорта
        public Operation(Guid id, OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId)
        {
            Id = id;
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            Description = description;
            CategoryId = categoryId;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }
}