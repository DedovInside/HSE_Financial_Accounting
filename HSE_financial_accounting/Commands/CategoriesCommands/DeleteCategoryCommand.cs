using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Commands.CategoriesCommands
{
    public class DeleteCategoryCommand : ICommand
    {
        private readonly ICategoryFacade _facade;
        private readonly IOperationFacade _operationFacade;
        private readonly Guid _categoryId;
        
        public DeleteCategoryCommand(ICategoryFacade facade, IOperationFacade operationFacade, Guid categoryId)
        {
            _facade = facade;
            _categoryId = categoryId;
            _operationFacade = operationFacade;
        }
        
        public void Execute()
        {
            IEnumerable<IOperation> operations = _operationFacade.GetOperationsByCategory(_categoryId);
            foreach (IOperation operation in operations)
            {
                _operationFacade.DeleteOperation(operation.Id);
            }
            _facade.DeleteCategory(_categoryId);
        }
    }
}