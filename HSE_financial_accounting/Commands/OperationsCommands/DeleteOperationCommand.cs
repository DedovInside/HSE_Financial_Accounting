using HSE_financial_accounting.Facades;
namespace HSE_financial_accounting.Commands.OperationsCommands
{
    public class DeleteOperationCommand : ICommand
    {
        private readonly IOperationFacade _facade;
        private readonly Guid _operationId;
        
        public DeleteOperationCommand(IOperationFacade facade, Guid operationId)
        {
            _facade = facade;
            _operationId = operationId;
        }
        
        public void Execute()
        {
            _facade.DeleteOperation(_operationId);
        }
    }
}