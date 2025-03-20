namespace HSE_financial_accounting.DataImport
{
    using Facades;
    using DataTransferObjects;
    using System.Text.Json;
    public class JsonDataImporter : DataImporter
    {
        public JsonDataImporter(
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            IOperationFacade operationFacade)
            : base(accountFacade, categoryFacade, operationFacade) {}
        
        protected override FinancialData ParseFile(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<FinancialData>(jsonString) ?? new FinancialData();
        }
    }
}