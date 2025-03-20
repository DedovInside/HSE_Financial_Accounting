namespace HSE_financial_accounting.DataTransferObjects
{
    public class FinancialData
    {
        public List<BankAccountDto> Accounts { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<OperationDto> Operations { get; set; } = new();
    }
}