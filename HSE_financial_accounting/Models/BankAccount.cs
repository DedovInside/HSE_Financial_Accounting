using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Models
{
    
    public class BankAccount : IBankAccount
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        public BankAccount(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }

        // Конструктор для импорта с указанным ID
        public BankAccount(Guid id, string name, decimal balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            Balance -= amount;
        }
        
        public void UpdateName(string newName)
        {
            Name = newName;
        }
    }
}