using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class GetAllOperationsCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private IEnumerable<IOperation> _result;

        public GetAllOperationsCommand(IAnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
            _result = new List<IOperation>();
        }

        public void Execute()
        {
            _result = _analyticsFacade.GetAllOperations();
        }

        public IEnumerable<IOperation> GetResult()
        {
            return _result;
        }
    }
} 