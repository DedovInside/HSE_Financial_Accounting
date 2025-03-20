using HSE_financial_accounting.Factories;
using HSE_financial_accounting.Repositories;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Facades
{
   
    public class OperationFacade : IOperationFacade
    {
        private readonly FinanceEntityCreator _creator;
        private readonly IRepository<IOperation> _operationRepository;
        private readonly IBankAccountFacade _bankAccountFacade;
        
        public OperationFacade(FinanceEntityCreator creator, IRepository<IOperation> operationRepository, IBankAccountFacade bankAccountFacade)
        {
            _creator = creator;
            _operationRepository = operationRepository;
            _bankAccountFacade = bankAccountFacade;
        }
        
        public void CreateOperation(OperationType type, Guid bankAccountId, decimal amount, 
            DateTime date, string description, Guid categoryId)
        {
            IOperation operation = _creator.CreateOperation(type, bankAccountId, amount, date, description, categoryId);
            _operationRepository.Add(operation);
            IBankAccount? account = _bankAccountFacade.GetBankAccount(bankAccountId);
            if (type == OperationType.Income)
            {
                account.Deposit(amount);
            }
            else
            {
                account.Withdraw(amount);
            }
        }


        // Метод для импорта операции с заданным ID
        public void CreateOperationWithId(Guid id, OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId)
        {
            IBankAccount? account = _bankAccountFacade.GetBankAccount(bankAccountId);
            
            IOperation operation = _creator.CreateOperationWithId(id, type, bankAccountId, amount, date, description, categoryId);
            _operationRepository.Add(operation);
            
            // Обновляем баланс счета
            if (type == OperationType.Income)
            {
                account.Deposit(amount);
            }
            else
            {
                account.Withdraw(amount);
            }
        }
        
        public void UpdateOperationDescription(Guid operationId, string newDescription)
        {
            IOperation? operation = _operationRepository.GetById(operationId);
            operation.UpdateDescription(newDescription);
            _operationRepository.Update(operation);
        }
        
        public void DeleteOperation(Guid operationId)
        {
            IOperation? operation = _operationRepository.GetById(operationId);
            IBankAccount account = _bankAccountFacade.GetBankAccount(operation.BankAccountId);
            if (operation.Type == OperationType.Income)
            {
                account.Withdraw(operation.Amount);
            }
            else
            {
                account.Deposit(operation.Amount);
            }
            _operationRepository.Delete(operationId);
        }

        public IOperation? GetOperation(Guid operationId)
        {
            return _operationRepository.GetById(operationId);
        }
        
        public IEnumerable<IOperation> GetAllOperations()
        {
            return _operationRepository.GetAll();
        }
        
        public IEnumerable<IOperation> GetOperationsByAccount(Guid accountId)
        {
            return _operationRepository.GetAll().Where(o => o.BankAccountId == accountId);
        }
        
        public IEnumerable<IOperation> GetOperationsByCategory(Guid categoryId)
        {
            return _operationRepository.GetAll().Where(o => o.CategoryId == categoryId);
        }
    }
}