using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class GetAllCategoriesCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private IEnumerable<ICategory> _result;

        public GetAllCategoriesCommand(IAnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
            _result = new List<ICategory>();
        }

        public void Execute()
        {
            _result = _analyticsFacade.GetAllCategories();
        }

        public IEnumerable<ICategory> GetResult()
        {
            return _result;
        }
    }
} 