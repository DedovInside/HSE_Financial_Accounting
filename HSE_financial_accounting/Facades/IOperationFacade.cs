using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.Models;

namespace HSE_financial_accounting.Facades
{
    public interface IOperationFacade
    {
        void CreateOperation(OperationType type, Guid bankAccountId, decimal amount, 
            DateTime date, string description, Guid categoryId);

        void CreateOperationWithId(Guid id, OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId);
        void UpdateOperationDescription(Guid operationId, string newDescription);
        void DeleteOperation(Guid operationId);
        IOperation? GetOperation(Guid operationId);
        IEnumerable<IOperation> GetAllOperations();
        IEnumerable<IOperation> GetOperationsByAccount(Guid accountId);
        IEnumerable<IOperation> GetOperationsByCategory(Guid categoryId);
    }
}