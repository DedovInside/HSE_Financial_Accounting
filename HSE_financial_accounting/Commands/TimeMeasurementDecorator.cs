using System.Diagnostics;
namespace HSE_financial_accounting.Commands
{
    public class TimeMeasurementDecorator : ICommand
    {
        private readonly ICommand _command;
        private readonly string _commandName;
        private readonly Action<string, TimeSpan> _logAction;
        
        public TimeMeasurementDecorator(
            ICommand command, 
            string commandName, 
            Action<string, TimeSpan> logAction)
        {
            _command = command;
            _commandName = commandName;
            _logAction = logAction;
        }
        
        public void Execute()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            try
            {
                _command.Execute();
            }
            finally
            {
                stopwatch.Stop();
                _logAction(_commandName, stopwatch.Elapsed);
            }
        }
    }
    
}