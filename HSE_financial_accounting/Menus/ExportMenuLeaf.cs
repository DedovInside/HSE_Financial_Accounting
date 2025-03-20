using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Commands.ExportCommands;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.DataExport;
namespace HSE_financial_accounting.Menus
{
    public class ExportMenuLeaf : BaseMenuComponent
    {
        private readonly DataExporter _exporter;
        private readonly JsonExportVisitor _jsonVisitor;
        private readonly CsvExportVisitor _csvVisitor;
        private readonly YamlExportVisitor _yamlVisitor;
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        
        public override string Name => "Экспорт данных в файл";

        public ExportMenuLeaf(
            DataExporter exporter,
            JsonExportVisitor jsonVisitor,
            CsvExportVisitor csvVisitor,
            YamlExportVisitor yamlVisitor,
            CommandInvoker commandInvoker,
            ILogger logger)
        {
            _exporter = exporter;
            _jsonVisitor = jsonVisitor;
            _csvVisitor = csvVisitor;
            _yamlVisitor = yamlVisitor;
            _commandInvoker = commandInvoker;
            _logger = logger;
        }

        public override void Display()
        {
            while (true)
            {
                (int index, string text)[] options = GetOptions();
                int selectedOption = RunMenu(options, "Экспорт данных в файл:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Возврат в главное меню");
                    break;
                }
                
                Console.WriteLine("Введите путь для сохранения данных в файл:");
                string? filePath = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    _logger.LogWarning("Экспорт данных отменен: не указан путь для сохранения данных");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    continue;
                }

                IExportVisitor? visitor = null;
                string? formatName = null;
                switch (selectedOption)
                {
                    // Экспорт данных в CSV
                    case 1:
                        visitor = _csvVisitor;
                        formatName = "CSV";
                        break;
                    
                    // Экспорт данных в JSON
                    case 2:
                        visitor = _jsonVisitor;
                        formatName = "JSON";
                        break;
                    
                    // Экспорт данных в YAML
                    case 3:
                        visitor = _yamlVisitor;
                        formatName = "YAML";
                        break;
                }

                try
                {
                    ExportDataCommand exportCommand = new(_exporter, visitor, filePath);
                    _commandInvoker.ExecuteWithTimeMeasurement(exportCommand, $"Экспорт данных в формат {formatName}");
                    _logger.LogInformation($"Данные успешно экспортированы в формате {formatName} в файл {filePath}");
                    Console.WriteLine("Экспорт успешно завершен!");
                }
                
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при экспорте данных в формат {formatName}", ex);
                    Console.WriteLine($"Ошибка при экспорте данных в формат {formatName}: {ex.Message}");
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        public override bool HandleInput(ConsoleKeyInfo key)
        {
            return false;
        }

        private (int index, string text)[] GetOptions()
        {
            return
            [
                (1, "Экспорт данных в CSV"),
                (2, "Экспорт данных в JSON"),
                (3, "Экспорт данных в YAML"),
                (0, "Назад")
            ];
        }
    }
}