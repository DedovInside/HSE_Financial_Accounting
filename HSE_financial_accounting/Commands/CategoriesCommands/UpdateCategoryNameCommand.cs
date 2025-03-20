using HSE_financial_accounting.Facades;     
namespace HSE_financial_accounting.Commands.CategoriesCommands
{
    public class UpdateCategoryNameCommand : ICommand
    {
        private readonly ICategoryFacade _facade;
        private readonly Guid _categoryId;
        private readonly string _newName;
        
        public UpdateCategoryNameCommand(ICategoryFacade facade, Guid categoryId, string newName)
        {
            _facade = facade;
            _categoryId = categoryId;
            _newName = newName;
        }

        public void Execute()
        {
            _facade.UpdateCategoryName(_categoryId, _newName);
        }
    }
}