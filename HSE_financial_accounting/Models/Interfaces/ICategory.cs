namespace HSE_financial_accounting.Models.Interfaces
{
    public interface ICategory
    {
        Guid Id { get; }
        string Name { get; }
        CategoryType Type { get; }
        void UpdateName(string name);
    }
}