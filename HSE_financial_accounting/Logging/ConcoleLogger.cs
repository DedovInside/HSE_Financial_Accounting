namespace HSE_financial_accounting.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void LogInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
            Console.ResetColor();
        }
        
        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {DateTime.Now}: {message}");
            Console.ResetColor();
        }
        
        public void LogError(string message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            Console.WriteLine($"Exception: {exception.Message}");
            Console.ResetColor();
        }
    }
}