using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.Facades
{
    public interface ICategoryFacade
    {
        ICategory CreateCategory(string categoryName, CategoryType categoryType);
        ICategory CreateCategoryWithId(Guid categoryId, string categoryName, CategoryType categoryType);
        void UpdateCategoryName(Guid categoryId, string newCategoryName);
        void DeleteCategory(Guid categoryId);
        ICategory? GetCategory(Guid categoryId);
        IEnumerable<ICategory> GetAllCategories();
    }
}