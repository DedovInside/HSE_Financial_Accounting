using HSE_financial_accounting.Facades;

namespace HSE_financial_accounting.Commands.AnalyticsCommands
{
    public class CalculateIncomeExpenseDifferenceCommand : ICommand
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private readonly Guid _accountId;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private decimal _result;

        public CalculateIncomeExpenseDifferenceCommand(
            IAnalyticsFacade analyticsFacade,
            Guid accountId,
            DateTime startDate,
            DateTime endDate)
        {
            _analyticsFacade = analyticsFacade;
            _accountId = accountId;
            _startDate = startDate;
            _endDate = endDate;
        }

        public void Execute()
        {
            _result = _analyticsFacade.CalculateIncomeExpenseDifferenceForAccount(_accountId, _startDate, _endDate);
        }

        // Получение результата после выполнения команды
        public decimal GetResult()
        {
            return _result;
        }
    }
} 