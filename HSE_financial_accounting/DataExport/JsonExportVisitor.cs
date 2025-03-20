using HSE_financial_accounting.DataTransferObjects;
using HSE_financial_accounting.Models.Interfaces;
using System.Text.Json;
namespace HSE_financial_accounting.DataExport
{
    public class JsonExportVisitor : IExportVisitor
    {
        private readonly FinancialData _data = new();
        
        public void VisitBankAccount(IBankAccount account)
        {
            _data.Accounts.Add(new BankAccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Balance = account.Balance
            });
        }
        
        public void VisitCategory(ICategory category)
        {
            _data.Categories.Add(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type
            });
        }
        
        public void VisitOperation(IOperation operation)
        {
            _data.Operations.Add(new OperationDto
            {
                Id = operation.Id,
                Type = operation.Type,
                BankAccountId = operation.BankAccountId,
                Amount = operation.Amount,
                Date = operation.Date,
                Description = operation.Description,
                CategoryId = operation.CategoryId
            });
        }
        
        public void SaveToFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".json")
            {
                throw new ArgumentException($"Invalid file extension. Expected .json but got {extension}");
            }
            
            string jsonString = JsonSerializer.Serialize(_data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(filePath, jsonString);
        }
    }
}