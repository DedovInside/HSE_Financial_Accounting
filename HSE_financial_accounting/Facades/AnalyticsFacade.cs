using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.Models;
using Moq;

namespace HSE_financial_accounting.Facades
{
    public class AnalyticsFacade : IAnalyticsFacade
    {
        private readonly IOperationFacade _operationFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly IBankAccountFacade _bankAccountFacade;
         
        public AnalyticsFacade(
            IOperationFacade operationFacade, 
            ICategoryFacade categoryFacade,
            IBankAccountFacade bankAccountFacade)
        {
            _operationFacade = operationFacade;
            _categoryFacade = categoryFacade;
            _bankAccountFacade = bankAccountFacade;
        }
        
        // Получение информации о счетах и категориях
        public IEnumerable<IBankAccount> GetAllAccounts()
        {
            return _bankAccountFacade.GetAllBankAccounts();
        }
        
        public IEnumerable<ICategory> GetAllCategories()
        {
            return _categoryFacade.GetAllCategories();
        }
        
        // Получение информации об операциях
        public IEnumerable<IOperation> GetAllOperations()
        {
            return _operationFacade.GetAllOperations();
        }
        
        public IEnumerable<IOperation> GetOperationsByAccount(Guid accountId)
        {
            return _operationFacade.GetOperationsByAccount(accountId);
        }
        
        // Аналитические функции
        public decimal CalculateIncomeExpenseDifferenceForAccount(Guid accountId, DateTime startDate, DateTime endDate)
        {
            List<IOperation> operations = _operationFacade.GetOperationsByAccount(accountId)
                .Where(o => o.Date >= startDate && o.Date <= endDate)
                .ToList();
            
            decimal totalIncome = operations
                .Where(o => o.Type == OperationType.Income)
                .Sum(o => o.Amount);
                
            decimal totalExpense = operations
                .Where(o => o.Type == OperationType.Expense)
                .Sum(o => o.Amount);
                
            return totalIncome - totalExpense;
        }
        
        public Dictionary<ICategory, decimal> GroupOperationsByCategoryForAccount(Guid accountId, DateTime startDate, DateTime endDate)
        {
            Dictionary<ICategory, decimal> result = new();
            List<IOperation> operations = _operationFacade.GetOperationsByAccount(accountId)
                .Where(o => o.Date >= startDate && o.Date <= endDate)
                .ToList();
                
            IEnumerable<ICategory> categories = _categoryFacade.GetAllCategories();
            
            foreach (ICategory category in categories)
            {
                decimal amount = operations
                    .Where(o => o.CategoryId == category.Id)
                    .Sum(o => o.Amount);
                    
                result[category] = amount;
            }
            return result;
        }
    }
}