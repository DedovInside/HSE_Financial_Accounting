using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.DataExport
{ 
    public class DataExporter
    {
        private readonly IBankAccountFacade _accountFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly IOperationFacade _operationFacade;
        
        public DataExporter(
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            IOperationFacade operationFacade)
        {
            _accountFacade = accountFacade;
            _categoryFacade = categoryFacade;
            _operationFacade = operationFacade;
        }
        
        public void ExportData(IExportVisitor visitor, string filePath)
        {
            // Validate that the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
            
            // Посещаем все счета
            foreach (IBankAccount account in _accountFacade.GetAllBankAccounts())
            {
                visitor.VisitBankAccount(account);
            }
            
            // Посещаем все категории
            foreach (ICategory category in _categoryFacade.GetAllCategories())
            {
                visitor.VisitCategory(category);
            }
            
            // Посещаем все операции
            foreach (IOperation operation in _operationFacade.GetAllOperations())
            {
                visitor.VisitOperation(operation);
            }
            
            // Сохраняем результат в файл
            visitor.SaveToFile(filePath);
        }
    }
}