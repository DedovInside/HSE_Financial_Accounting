using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class GroupOperationsByCategoryCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private readonly Guid _accountId;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private Dictionary<ICategory, decimal> _result;

        public GroupOperationsByCategoryCommand(
            IAnalyticsFacade analyticsFacade,
            Guid accountId,
            DateTime startDate,
            DateTime endDate)
        {
            _analyticsFacade = analyticsFacade;
            _accountId = accountId;
            _startDate = startDate;
            _endDate = endDate;
            _result = new Dictionary<ICategory, decimal>();
        }

        public void Execute()
        {
            _result = _analyticsFacade.GroupOperationsByCategoryForAccount(_accountId, _startDate, _endDate);
        }

        // Получение результата после выполнения команды
        public Dictionary<ICategory, decimal> GetResult()
        {
            return _result;
        }
    }
} 