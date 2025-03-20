using HSE_financial_accounting.Factories;
using HSE_financial_accounting.Repositories;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Facades
{
    public class BankAccountFacade : IBankAccountFacade
    {
        private readonly FinanceEntityCreator _creator;
        private readonly IRepository<IBankAccount> _bankAccountRepository;

        public BankAccountFacade(FinanceEntityCreator creator, IRepository<IBankAccount> bankAccountRepository)
        {
            _creator = creator;
            _bankAccountRepository = bankAccountRepository;
        }
        
        public void CreateBankAccount(string name, decimal currency)
        {
            IBankAccount bankAccount = _creator.CreateBankAccount(name, currency);
            _bankAccountRepository.Add(bankAccount);
        }

        // Метод для импорта счета с заданным ID
        public void CreateBankAccountWithId(Guid id, string name, decimal balance)
        {
            IBankAccount bankAccount = _creator.CreateBankAccountWithId(id, name, balance);
            _bankAccountRepository.Add(bankAccount);
        }

        public void UpdateAccountName(Guid id, string newName)
        {
            IBankAccount? account = _bankAccountRepository.GetById(id);
            account.UpdateName(newName); // Обновляем имя напрямую
            _bankAccountRepository.Update(account); // Сохраняем изменения в репозитории
            
        }
        
        public void DeleteBankAccount(Guid id)
        {
            _bankAccountRepository.Delete(id);
        }
        
        public IBankAccount? GetBankAccount(Guid id)
        {
            return _bankAccountRepository.GetById(id);
        }
        
        public IEnumerable<IBankAccount> GetAllBankAccounts()
        {
            return _bankAccountRepository.GetAll();
        }
    }
}