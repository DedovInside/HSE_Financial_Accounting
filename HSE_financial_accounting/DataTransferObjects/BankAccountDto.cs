namespace HSE_financial_accounting.DataTransferObjects
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}