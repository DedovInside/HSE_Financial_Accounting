using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class GetAllAccountsCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private IEnumerable<IBankAccount> _result;

        public GetAllAccountsCommand(IAnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
            _result = new List<IBankAccount>();
        }

        public void Execute()
        {
            _result = _analyticsFacade.GetAllAccounts();
        }

        public IEnumerable<IBankAccount> GetResult()
        {
            return _result;
        }
    }
} 