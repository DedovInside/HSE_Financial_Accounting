using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.DataTransferObjects;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace HSE_financial_accounting.DataExport
{

    public class YamlExportVisitor : IExportVisitor
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
            if (extension != ".yaml" && extension != ".yml")
            {
                throw new ArgumentException($"Invalid file extension. Expected .yaml or .yml but got {extension}");
            }
            
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                
            string yaml = serializer.Serialize(_data);
            File.WriteAllText(filePath, yaml);
        }
    }
}