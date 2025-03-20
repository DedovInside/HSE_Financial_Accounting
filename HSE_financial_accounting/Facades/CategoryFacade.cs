using HSE_financial_accounting.Factories;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Repositories;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Facades
{
    public class CategoryFacade : ICategoryFacade
    {
        private readonly FinanceEntityCreator _creator;
        private readonly IRepository<ICategory> _categoryRepository; 
        
        public CategoryFacade(FinanceEntityCreator creator, IRepository<ICategory> categoryRepository)
        {
            _creator = creator;
            _categoryRepository = categoryRepository;
        }
        
        public ICategory CreateCategory(string name, CategoryType categoryType)
        {
            ICategory category = _creator.CreateCategory(name, categoryType);
            _categoryRepository.Add(category);
            return category;
        }

        // Метод для импорта категории с заданным ID
        public ICategory CreateCategoryWithId(Guid id, string name, CategoryType categoryType)
        {
            ICategory category = _creator.CreateCategoryWithId(id, name, categoryType);
            _categoryRepository.Add(category);
            return category;
        }
        
        public void UpdateCategoryName(Guid categoryId, string newCategoryName)
        {
            ICategory? category = _categoryRepository.GetById(categoryId);
            category.UpdateName(newCategoryName);
            _categoryRepository.Update(category);
        }
        
        public void DeleteCategory(Guid categoryId)
        {
            _categoryRepository.Delete(categoryId);
        }
        
        public ICategory? GetCategory(Guid categoryId)
        {
            return _categoryRepository.GetById(categoryId);
        }
        
        public IEnumerable<ICategory> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }
        
        public IEnumerable<ICategory> GetCategoriesByType(CategoryType operationType)
        {
            return _categoryRepository.GetAll().Where(c => c.Type == operationType);
        }
    }
}