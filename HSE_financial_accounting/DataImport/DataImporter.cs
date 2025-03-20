namespace HSE_financial_accounting.DataImport
{
    using Facades;
    using DataTransferObjects;
    public abstract class DataImporter
    {
        protected readonly IBankAccountFacade AccountFacade;
        protected readonly ICategoryFacade CategoryFacade;
        protected readonly IOperationFacade OperationFacade;
        
        protected DataImporter(
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            IOperationFacade operationFacade)
        {
            AccountFacade = accountFacade;
            CategoryFacade = categoryFacade;
            OperationFacade = operationFacade;
        }
        
        protected abstract FinancialData ParseFile(string filePath);
        
        // Общий метод обработки данных
        private void ProcessData(FinancialData data)
        {
            
            // 1. Импортируем счета с исходными ID
            foreach (BankAccountDto account in data.Accounts)
            {
                AccountFacade.CreateBankAccountWithId(account.Id, account.Name, account.Balance);
            }
    
            // 2. Импортируем категории с исходными ID
            foreach (CategoryDto category in data.Categories)
            {
                CategoryFacade.CreateCategoryWithId(category.Id, category.Name, category.Type);
            }
    
            // 3. Импортируем операции с исходными ID
            foreach (OperationDto operation in data.Operations)
            {
                
                OperationFacade.CreateOperationWithId(
                    operation.Id,
                    operation.Type,
                    operation.BankAccountId,
                    operation.Amount,
                    operation.Date,
                    operation.Description,
                    operation.CategoryId);
                    
            }
        }
        
        public void ImportData(string filePath)
        {
            FinancialData data = ParseFile(filePath);
            ProcessData(data);
        }
    }
}