using Microsoft.Extensions.DependencyInjection;
using HSE_financial_accounting.Factories;
using HSE_financial_accounting.Repositories;
using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.Facades;
using HSE_financial_accounting.DataExport;
using HSE_financial_accounting.DataImport;
using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Menus;

namespace HSE_financial_accounting
{
    internal static class Program
    {
        private static ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();
            // Регистрация фабрики
            services.AddSingleton<FinanceEntityCreator, StandardFinanceCreator>(
                _ => new StandardFinanceCreator("Standard"));
            
            // Регистрация репозиториев
            services.AddSingleton<IRepository<IBankAccount>, InMemoryRepository<IBankAccount>>();
            services.AddSingleton<IRepository<ICategory>, InMemoryRepository<ICategory>>();
            services.AddSingleton<IRepository<IOperation>, InMemoryRepository<IOperation>>();
            
            // Регистрация фасадов
            services.AddSingleton<IBankAccountFacade, BankAccountFacade>();
            services.AddSingleton<ICategoryFacade, CategoryFacade>();
            services.AddSingleton<IOperationFacade, OperationFacade>();
            services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();
            
            // Регистрация посетителей для экспорта
            services.AddTransient<JsonExportVisitor>();
            services.AddTransient<CsvExportVisitor>();
            services.AddTransient<YamlExportVisitor>();
            
            // Регистрация импортеров
            services.AddTransient<JsonDataImporter>();
            services.AddTransient<CsvDataImporter>();
            services.AddTransient<YamlDataImporter>();
            
            // Регистрация экспортера
            services.AddTransient<DataExporter>();
            
            // Регистрация логгера
            services.AddSingleton<ILogger, ConsoleLogger>();
            
            // Регистрация инвокера команд
            services.AddSingleton<CommandInvoker>();
            
            return services.BuildServiceProvider();
        }
        
        private static void Main()
        {
            ServiceProvider serviceProvider = ConfigureServices();
            
            // Получаем сервисы
            ILogger logger = serviceProvider.GetRequiredService<ILogger>();
            IBankAccountFacade accountFacade = serviceProvider.GetRequiredService<IBankAccountFacade>();
            ICategoryFacade categoryFacade = serviceProvider.GetRequiredService<ICategoryFacade>();
            IOperationFacade operationFacade = serviceProvider.GetRequiredService<IOperationFacade>();
            IAnalyticsFacade analyticsFacade = serviceProvider.GetRequiredService<IAnalyticsFacade>();
            CommandInvoker commandInvoker = serviceProvider.GetRequiredService<CommandInvoker>();
            DataExporter dataExporter = serviceProvider.GetRequiredService<DataExporter>();
            JsonExportVisitor jsonVisitor = serviceProvider.GetRequiredService<JsonExportVisitor>();
            CsvExportVisitor csvVisitor = serviceProvider.GetRequiredService<CsvExportVisitor>();
            YamlExportVisitor yamlVisitor = serviceProvider.GetRequiredService<YamlExportVisitor>();
            JsonDataImporter jsonImporter = serviceProvider.GetRequiredService<JsonDataImporter>();
            CsvDataImporter csvImporter = serviceProvider.GetRequiredService<CsvDataImporter>();
            YamlDataImporter yamlImporter = serviceProvider.GetRequiredService<YamlDataImporter>();
            
            // Создание дерева меню. Главное меню, как корневой компонент
            MainMenuComposite mainMenu = new(logger);
            // Создание компонентов меню
            
            // Управление аккаунтами
            AccountsMenuLeaf bankAccountsMenu = new(accountFacade, operationFacade, logger, commandInvoker);
            mainMenu.AddComponent(bankAccountsMenu);
            
            // Управление категориями
            CategoriesMenuLeaf categoriesMenu = new(categoryFacade, operationFacade, logger, commandInvoker);
            mainMenu.AddComponent(categoriesMenu);
            
            // Управление операциями
            OperationsMenuLeaf operationsMenu = new(operationFacade, accountFacade, categoryFacade, logger, commandInvoker);
            mainMenu.AddComponent(operationsMenu);
            
            // Аналитика
            AnalyticsMenuLeaf analyticsMenu = new(analyticsFacade, accountFacade, categoryFacade, logger, commandInvoker);
            mainMenu.AddComponent(analyticsMenu);
            
            // Экспорт данных
            ExportMenuLeaf exportMenu = new(dataExporter, jsonVisitor, csvVisitor, yamlVisitor, commandInvoker, logger);
            mainMenu.AddComponent(exportMenu);
            
            // Импорт данных
            ImportMenuLeaf importMenu = new(jsonImporter, csvImporter, yamlImporter, logger, commandInvoker);
            mainMenu.AddComponent(importMenu);
            
            // Запуск приложения
            Console.WriteLine("Здравствуйте! Добро пожаловать в систему управления финансами.");
            Console.WriteLine("Нажмите любую клавишу, чтобы начать работу:");
            Console.ReadKey();
            Console.Clear();
            mainMenu.Display();
        }
    }
}