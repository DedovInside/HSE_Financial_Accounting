using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Factories
{
    
    public abstract class FinanceEntityCreator
    {
        public string CreatorName { get; }
        
        protected FinanceEntityCreator(string name)
        {
            CreatorName = name;
        }
        
        public abstract IBankAccount CreateBankAccount(string name, decimal balance);
        public abstract ICategory CreateCategory(string name, CategoryType type);
        public abstract IOperation CreateOperation(OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId);


        // Методы для импорта
        public abstract IBankAccount CreateBankAccountWithId(Guid id, string name, decimal initialBalance);
        public abstract ICategory CreateCategoryWithId(Guid id, string name, CategoryType type);
        public abstract IOperation CreateOperationWithId(Guid id, OperationType type, Guid bankAccountId, 
            decimal amount, DateTime date, string description, Guid categoryId);
    }
}