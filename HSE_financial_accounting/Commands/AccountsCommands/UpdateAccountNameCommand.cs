using HSE_financial_accounting.Facades;

namespace HSE_financial_accounting.Commands.AccountsCommands
{
    public class UpdateAccountNameCommand : ICommand
    {
        private readonly IBankAccountFacade _facade;
        private readonly Guid _accountId;
        private readonly string _newName;
        
        public UpdateAccountNameCommand(IBankAccountFacade facade, Guid accountId, string newName)
        {
            _facade = facade;
            _accountId = accountId;
            _newName = newName;
        }
        
        public void Execute()
        {
            _facade.UpdateAccountName(_accountId, _newName);
        }
    }
} 