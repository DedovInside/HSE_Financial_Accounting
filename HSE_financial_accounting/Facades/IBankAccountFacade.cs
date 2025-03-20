using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Facades
{
    public interface IBankAccountFacade
    {
        void CreateBankAccount(string name, decimal initialBalance);
        void CreateBankAccountWithId(Guid accountId, string name, decimal initialBalance);
        void UpdateAccountName(Guid accountId, string newName);
        void DeleteBankAccount(Guid accountId);
        IBankAccount? GetBankAccount(Guid accountId);
        IEnumerable<IBankAccount> GetAllBankAccounts();
    }
}