using HSE_financial_accounting.Facades;
using HSE_financial_accounting.DataTransferObjects;
using System.Globalization;
using HSE_financial_accounting.Models;
namespace HSE_financial_accounting.DataImport
{
    public class CsvDataImporter : DataImporter
    {
        public CsvDataImporter(IBankAccountFacade accountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
            : base(accountFacade, categoryFacade, operationFacade) {}
        
        protected override FinancialData ParseFile(string filePath)
        {
            FinancialData result = new();
            string[] lines = File.ReadAllLines(filePath);
            
            // Определяем секции в CSV файле
            int accountsStartLine = Array.IndexOf(lines, "[Accounts]") + 1;
            int categoriesStartLine = Array.IndexOf(lines, "[Categories]") + 1;
            int operationsStartLine = Array.IndexOf(lines, "[Operations]") + 1;
            
            // Парсим счета (пропускаем строку с заголовками)
            for (int i = accountsStartLine + 1; i < lines.Length && !lines[i].StartsWith("[") && !string.IsNullOrWhiteSpace(lines[i]); i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 3)
                {
                    result.Accounts.Add(new BankAccountDto
                    {
                        Id = Guid.Parse(parts[0]),
                        Name = parts[1],
                        Balance = decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    });
                }
            }
            
            // Парсим категории (пропускаем строку с заголовками)
            for (int i = categoriesStartLine + 1; i < lines.Length && !lines[i].StartsWith("[") && !string.IsNullOrWhiteSpace(lines[i]); i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 3)
                {
                    result.Categories.Add(new CategoryDto
                    {
                        Id = Guid.Parse(parts[0]),
                        Name = parts[1],
                        Type = (CategoryType)Enum.Parse(typeof(CategoryType), parts[2])
                    });
                }
            }
            
            // Парсим операции (пропускаем строку с заголовками)
            for (int i = operationsStartLine + 1; i < lines.Length && !lines[i].StartsWith("[") && !string.IsNullOrWhiteSpace(lines[i]); i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 7)
                {
                    result.Operations.Add(new OperationDto
                    {
                        Id = Guid.Parse(parts[0]),
                        Type = (OperationType)Enum.Parse(typeof(OperationType), parts[1]),
                        BankAccountId = Guid.Parse(parts[2]),
                        Amount = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        Date = DateTime.Parse(parts[4], CultureInfo.InvariantCulture),
                        Description = parts[5],
                        CategoryId = Guid.Parse(parts[6])
                    });
                }
            }
            
            return result;
        }
    }
}