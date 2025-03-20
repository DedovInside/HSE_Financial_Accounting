namespace HSE_financial_accounting.Models.Interfaces
{
    public interface IBankAccount
    {
        Guid Id { get; }
        string Name { get; }
        decimal Balance { get; }
        void Deposit(decimal amount);
        void Withdraw(decimal amount);
        void UpdateName(string newName);
    }
}