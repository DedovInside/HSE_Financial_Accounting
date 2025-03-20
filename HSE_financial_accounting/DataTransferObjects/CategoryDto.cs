using HSE_financial_accounting.Models;
namespace HSE_financial_accounting.DataTransferObjects
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
    }
}