using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Commands.ImportCommands;
using HSE_financial_accounting.DataImport;
using HSE_financial_accounting.Logging;
namespace HSE_financial_accounting.Menus
{
    public class ImportMenuLeaf : BaseMenuComponent
    {
        private readonly JsonDataImporter _jsonImporter;
        private readonly CsvDataImporter _csvImporter;
        private readonly YamlDataImporter _yamlImporter;
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        
        public override string Name => "Импорт данных из файла";

        public ImportMenuLeaf(
            JsonDataImporter jsonImporter,
            CsvDataImporter csvImporter,
            YamlDataImporter yamlImporter,
            ILogger logger,
            CommandInvoker commandInvoker)
        {
            _jsonImporter = jsonImporter;
            _csvImporter = csvImporter;
            _yamlImporter = yamlImporter;
            _logger = logger;
            _commandInvoker = commandInvoker;
        }

        public override void Display()
        {
            while (true)
            {
                (int index, string text)[] options = GetOptions();
                int selectedOption = RunMenu(options, "Управление категориями:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Возврат в главное меню");
                    break;
                }
                
                Console.Write("Введите путь к файлу для импорта: ");
                string? filePath = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    _logger.LogWarning("Импорт отменен: путь не указан");
                    Console.WriteLine("Путь к файлу не указан. Попробуйте снова.");
                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                }
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"Файл не найден: {filePath}");
                    Console.WriteLine("Файл не найден. Проверьте путь и попробуйте снова.");
                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    continue;
                }
                
                DataImporter? importer = null;
                string? formatName = null;
                switch (selectedOption)
                {
                    // Экспорт данных в CSV
                    case 1:
                        importer = _csvImporter;
                        formatName = "CSV";
                        break;
                    // Экспорт данных в JSON
                    case 2:
                        importer = _jsonImporter;
                        formatName = "JSON";
                        break;
                    
                    // Экспорт данных в YAML
                    case 3:
                        importer = _yamlImporter;
                        formatName = "YAML";
                        break;
                }

                try
                {
                    ImportDataCommand command = new(importer, filePath);
                    _commandInvoker.ExecuteWithTimeMeasurement(command, $"Импорт из {formatName}");
                    _logger.LogInformation($"Данные успешно импортированы из {formatName}: {filePath}");
                    Console.WriteLine("Импорт успешно завершен!");
                }
                
                catch (Exception e)
                {
                    _logger.LogError($"Ошибка при импорте данных из {formatName}: {filePath}", e);
                    Console.WriteLine($"Произошла ошибка при импорте данных из {formatName}: {filePath}");
                }

                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
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
                (1, "Импорт данных из CSV"),
                (2, "Импорт данных из JSON"),
                (3, "Импорт данных из YAML"),
                (0, "Назад")
            ];
        }
    }
}