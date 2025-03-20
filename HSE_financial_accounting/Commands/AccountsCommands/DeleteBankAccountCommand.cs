using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Commands.AccountsCommands
{
    public class DeleteBankAccountCommand : ICommand
    {
        private readonly IBankAccountFacade _facade;
        private readonly IOperationFacade _operationFacade;
        private readonly Guid _accountId;
        
        public DeleteBankAccountCommand(IBankAccountFacade facade, IOperationFacade operationFacade, Guid accountId)
        {
            _facade = facade;
            _operationFacade = operationFacade;
            _accountId = accountId;
        }
        
        public void Execute()
        {
            IEnumerable<IOperation> operations = _operationFacade.GetOperationsByAccount(_accountId);
            foreach (IOperation operation in operations)
            {
                _operationFacade.DeleteOperation(operation.Id);
            }
            _facade.DeleteBankAccount(_accountId);
        }
    }
}