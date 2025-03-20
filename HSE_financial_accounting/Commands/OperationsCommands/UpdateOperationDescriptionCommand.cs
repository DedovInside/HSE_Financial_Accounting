using HSE_financial_accounting.Facades;
namespace HSE_financial_accounting.Commands.OperationsCommands
{
    public class UpdateOperationDescriptionCommand : ICommand
    {
        private readonly IOperationFacade _facade;
        private readonly Guid _operationId;
        private readonly string _newDescription;
        
        public UpdateOperationDescriptionCommand(IOperationFacade facade, Guid operationId, string newDescription)
        {
            _facade = facade;
            _operationId = operationId;
            _newDescription = newDescription;
        }

        public void Execute()
        {
            _facade.UpdateOperationDescription(_operationId, _newDescription);
        }
    }
}