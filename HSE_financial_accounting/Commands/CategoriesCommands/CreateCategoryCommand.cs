using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models;
namespace HSE_financial_accounting.Commands.CategoriesCommands
{
    public class CreateCategoryCommand : ICommand
    {
        private readonly ICategoryFacade _facade;
        private readonly string _name;
        private readonly CategoryType _type;
        
        public CreateCategoryCommand(ICategoryFacade facade, string name, CategoryType type)
        {
            _facade = facade;
            _name = name;
            _type = type;
        }

        public void Execute()
        {
            _facade.CreateCategory(_name, _type);
        }
    }
}