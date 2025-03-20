using HSE_financial_accounting.Facades;
namespace HSE_financial_accounting.Commands.AccountsCommands
{
    public class CreateBankAccountCommand : ICommand
    {
        private readonly IBankAccountFacade _bankAccountFacade;
        private readonly string _name;
        private readonly decimal _initialBalance;
        
        public CreateBankAccountCommand(IBankAccountFacade bankAccountFacade, string name, decimal initialBalance)
        {
            _bankAccountFacade = bankAccountFacade;
            _name = name;
            _initialBalance = initialBalance;
        }
        
        public void Execute()
        {
            _bankAccountFacade.CreateBankAccount(_name, _initialBalance);
        }
    }
}