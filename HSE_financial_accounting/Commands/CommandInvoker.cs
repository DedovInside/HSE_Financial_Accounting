using HSE_financial_accounting.Logging;

namespace HSE_financial_accounting.Commands
{
    public class CommandInvoker
    {
        private readonly ILogger _logger;
        
        public CommandInvoker(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void ExecuteWithTimeMeasurement(ICommand command, string commandName)
        {
            TimeMeasurementDecorator decoratedCommand = new(
                command, 
                commandName, 
                (name, elapsed) => _logger.LogInformation($"Команда '{name}' выполнена за {elapsed.TotalMilliseconds} мс"));
                
            decoratedCommand.Execute();
        }
    }
}