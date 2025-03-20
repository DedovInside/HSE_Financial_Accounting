using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Facades
{
    public interface IAnalyticsFacade
    {
        // Получение информации о счетах и категориях
        IEnumerable<IBankAccount> GetAllAccounts();
        IEnumerable<ICategory> GetAllCategories();
        
        // Получение информации об операциях
        IEnumerable<IOperation> GetAllOperations();
        IEnumerable<IOperation> GetOperationsByAccount(Guid accountId);
        
        // Аналитические функции
        decimal CalculateIncomeExpenseDifferenceForAccount(Guid accountId, DateTime startDate, DateTime endDate);
        Dictionary<ICategory, decimal> GroupOperationsByCategoryForAccount(Guid accountId, DateTime startDate, DateTime endDate);
    }
}