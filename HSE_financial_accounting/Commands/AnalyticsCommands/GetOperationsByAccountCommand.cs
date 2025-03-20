using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class GetOperationsByAccountCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private readonly Guid _accountId;
        private IEnumerable<IOperation> _result;

        public GetOperationsByAccountCommand(IAnalyticsFacade analyticsFacade, Guid accountId)
        {
            _analyticsFacade = analyticsFacade;
            _accountId = accountId;
            _result = new List<IOperation>();
        }

        public void Execute()
        {
            _result = _analyticsFacade.GetOperationsByAccount(_accountId);
        }

        public IEnumerable<IOperation> GetResult()
        {
            return _result;
        }
    }
} 