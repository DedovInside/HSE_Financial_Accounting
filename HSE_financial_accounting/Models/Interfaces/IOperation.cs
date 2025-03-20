namespace HSE_financial_accounting.Models.Interfaces
{
    public interface IOperation
    {
        Guid Id { get; }
        OperationType Type { get; }
        Guid BankAccountId { get; }
        decimal Amount { get; }
        DateTime Date { get; }
        string Description { get; }
        Guid CategoryId { get; }
        void UpdateDescription(string description);
    }
}