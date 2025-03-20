using HSE_financial_accounting.Models.Interfaces;
using System.Text;
using System.Globalization;
namespace HSE_financial_accounting.DataExport
{
    public class CsvExportVisitor : IExportVisitor
    {
        private readonly List<IBankAccount> _accounts = [];
        private readonly List<ICategory> _categories = [];
        private readonly List<IOperation> _operations = [];
        
        public void VisitBankAccount(IBankAccount account)
        {
            _accounts.Add(account);
        }
        
        public void VisitCategory(ICategory category)
        {
            _categories.Add(category);
        }
        
        public void VisitOperation(IOperation operation)
        {
            _operations.Add(operation);
        }
        
        public void SaveToFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".csv")
            {
                throw new ArgumentException($"Invalid file extension. Expected .csv but got {extension}");
            }
            
            StringBuilder sb = new();
            
            // Секция счетов
            sb.AppendLine("[Accounts]");
            // Добавляем заголовки столбцов
            sb.AppendLine("Id,Name,Balance");
            foreach (IBankAccount account in _accounts)
            {
                sb.AppendLine($"{account.Id},{account.Name},{account.Balance.ToString(CultureInfo.InvariantCulture)}");
            }
            
            // Секция категорий
            sb.AppendLine();
            sb.AppendLine("[Categories]");
            // Добавляем заголовки столбцов
            sb.AppendLine("Id,Name,Type");
            foreach (ICategory category in _categories)
            {
                sb.AppendLine($"{category.Id},{category.Name},{category.Type}");
            }
            
            // Секция операций
            sb.AppendLine();
            sb.AppendLine("[Operations]");
            // Добавляем заголовки столбцов
            sb.AppendLine("Id,Type,BankAccountId,Amount,Date,Description,CategoryId");
            foreach (IOperation operation in _operations)
            {
                sb.AppendLine($"{operation.Id},{operation.Type},{operation.BankAccountId}," +
                             $"{operation.Amount.ToString(CultureInfo.InvariantCulture)}," +
                             $"{operation.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                             $"{operation.Description},{operation.CategoryId}");
            }
            
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}