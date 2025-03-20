using HSE_financial_accounting.Models.Interfaces;
namespace HSE_financial_accounting.DataExport
{
    public interface IExportVisitor
    {
        void VisitBankAccount(IBankAccount account);
        void VisitCategory(ICategory category);
        void VisitOperation(IOperation operation);
        void SaveToFile(string filePath);
    }
}