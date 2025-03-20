using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Factories
{
    

    public class StandardFinanceCreator : FinanceEntityCreator
    {
        public StandardFinanceCreator(string name) : base(name) { }

        public override IBankAccount CreateBankAccount(string name, decimal balance)
        {
            return new BankAccount(name, balance);
        }

        public override ICategory CreateCategory(string name, CategoryType type)
        {
            return new Category(name, type);
        }

        public override IOperation CreateOperation(OperationType type, Guid bankAccountId, decimal amount,
            DateTime date, string description, Guid categoryId)
        {
            return new Operation(type, bankAccountId, amount, date, description, categoryId);
        }


        // Методы для импорта
        public override IBankAccount CreateBankAccountWithId(Guid id, string name, decimal initialBalance)
        {
            return new BankAccount(id, name, initialBalance);
        }
    
        public override ICategory CreateCategoryWithId(Guid id, string name, CategoryType type)
        {
            return new Category(id, name, type);
        }
    
        public override IOperation CreateOperationWithId(Guid id, OperationType type, Guid bankAccountId, 
            decimal amount, DateTime date, string description, Guid categoryId)
        {
            return new Operation(id, type, bankAccountId, amount, date, description, categoryId);
        }
    }
}