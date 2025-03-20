namespace HSE_financial_accounting.DataTransferObjects
{
    using Models;
    public class OperationDto
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}