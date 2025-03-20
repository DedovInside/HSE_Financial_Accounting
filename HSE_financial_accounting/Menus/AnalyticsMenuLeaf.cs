using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;
using System.Globalization;
using HSE_financial_accounting.Commands.AnalyticsCommands;
namespace HSE_financial_accounting.Menus
{
    public class AnalyticsMenuLeaf : BaseMenuComponent
    {
        private readonly IAnalyticsFacade _analyticsFacade;
        private readonly IBankAccountFacade _accountFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        
        public override string Name => "Аналитика";

        public AnalyticsMenuLeaf(
            IAnalyticsFacade analyticsFacade, 
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            ILogger logger,
            CommandInvoker commandInvoker)
        {
            _analyticsFacade = analyticsFacade;
            _accountFacade = accountFacade;
            _categoryFacade = categoryFacade;
            _logger = logger;
            _commandInvoker = commandInvoker;
        }

        public override void Display()
        {
            while (true)
            {
                (int index, string text)[] options = GetOptions();
                int selectedOption = RunMenu(options, "Аналитика:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Возврат в главное меню");
                    break;
                }

                switch (selectedOption)
                {
                    // Показать существующие счета
                    case 1:
                        {
                            try
                            {
                                GetAllAccountsCommand allAccountsCommand = new(_analyticsFacade);
                                _commandInvoker.ExecuteWithTimeMeasurement(allAccountsCommand, "Получение всех счетов");

                                IEnumerable<IBankAccount> accounts = allAccountsCommand.GetResult();
                                List<IBankAccount> accountsList = accounts.ToList();

                                if (accountsList.Count == 0)
                                {
                                    Console.WriteLine("Нет созданных счетов.");
                                }
                                else
                                {
                                    Console.WriteLine("\nСписок всех счетов:");
                                    Console.WriteLine("------------------------------------------------------");

                                    decimal totalBalance = 0;
                                    foreach (IBankAccount account in accountsList)
                                    {
                                        Console.WriteLine(
                                            $"ID: {account.Id} | Название: {account.Name} | Баланс: {account.Balance:C2}");
                                        totalBalance += account.Balance;
                                    }

                                    Console.WriteLine("------------------------------------------------------");
                                    Console.WriteLine($"Итого на всех счетах: {totalBalance:C2}");
                                }

                                _logger.LogInformation("Просмотр всех счетов");
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при получении списка счетов.");
                                _logger.LogError("Ошибка при получении списка счетов", ex);
                            }

                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Показать существующие категории
                    case 2:
                        {
                            try
                            {
                                GetAllCategoriesCommand existingCategoriesCommand = new(_analyticsFacade);
                                _commandInvoker.ExecuteWithTimeMeasurement(existingCategoriesCommand,
                                    "Получение списка категорий");

                                IEnumerable<ICategory> categories = existingCategoriesCommand.GetResult();
                                List<ICategory> categoriesList = categories.ToList();

                                if (categoriesList.Count == 0)
                                {
                                    Console.WriteLine("Нет созданных категорий.");
                                }
                                else
                                {
                                    Console.WriteLine("\nСписок всех категорий:");
                                    Console.WriteLine("------------------------------------------------------");

                                    // Сначала выводим категории доходов
                                    Console.WriteLine("КАТЕГОРИИ ДОХОДОВ:");
                                    bool hasIncomeCategories = false;
                                    foreach (ICategory category in categoriesList.Where(c =>
                                                 c.Type == CategoryType.Income))
                                    {
                                        hasIncomeCategories = true;
                                        Console.WriteLine($"ID: {category.Id} | Название: {category.Name}");
                                    }

                                    if (!hasIncomeCategories)
                                    {
                                        Console.WriteLine("Нет категорий доходов");
                                    }

                                    // Затем выводим категории расходов
                                    Console.WriteLine("\nКАТЕГОРИИ РАСХОДОВ:");
                                    bool hasExpenseCategories = false;
                                    foreach (ICategory category in
                                             categoriesList.Where(c => c.Type == CategoryType.Expense))
                                    {
                                        hasExpenseCategories = true;
                                        Console.WriteLine($"ID: {category.Id} | Название: {category.Name}");
                                    }

                                    if (!hasExpenseCategories)
                                    {
                                        Console.WriteLine("Нет категорий расходов");
                                    }
                                }
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при получении списка категорий.");
                                _logger.LogError("Ошибка при получении списка категорий", ex);
                            }

                            _logger.LogInformation("Просмотр всех категорий");
                            
                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Показать все осуществлённые операции на всех счетах
                    case 3:
                        {
                            try
                            {
                                GetAllOperationsCommand allOperationsCommand = new(_analyticsFacade);
                                _commandInvoker.ExecuteWithTimeMeasurement(allOperationsCommand,
                                    "Получение всех операций");

                                IEnumerable<IOperation> operations = allOperationsCommand.GetResult();
                                List<IOperation> operationsList = operations.ToList();

                                if (operationsList.Count == 0)
                                {
                                    Console.WriteLine("Нет выполненных операций.");
                                }
                                else
                                {
                                    Console.WriteLine("\nСписок всех операций:");
                                    Console.WriteLine("------------------------------------------------------");

                                    foreach (IOperation operation in operationsList.OrderByDescending(o => o.Date))
                                    {
                                        string accountName =
                                            _accountFacade.GetBankAccount(operation.BankAccountId)?.Name ??
                                            "Неизвестный счет";
                                        string categoryName = _categoryFacade.GetCategory(operation.CategoryId)?.Name ??
                                                              "Без категории";
                                        string operationType =
                                            operation.Type == OperationType.Income ? "Доход" : "Расход";

                                        Console.WriteLine(
                                            $"Дата: {operation.Date:d} | Тип: {operationType} | Сумма: {operation.Amount:C2}");
                                        Console.WriteLine($"Счет: {accountName} | Категория: {categoryName}");
                                        Console.WriteLine($"Описание: {operation.Description}");
                                        Console.WriteLine("------------------------------------------------------");
                                    }
                                }

                                _logger.LogInformation("Просмотр всех операций");
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при получении списка операций.");
                                _logger.LogError("Ошибка при получении списка операций", ex);
                            }

                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            
                            break;
                        }

                    // Показать осуществлённые операции на конкретном счёте
                    case 4:
                        {
                            try
                            {
                                Console.Write("Введите ID счёта для просмотра операций: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    Console.WriteLine("Неверный формат ID счёта.");
                                    _logger.LogWarning("Просмотр операций отменен: введен неверный ID счета");
                                    break;
                                }

                                // Проверяем существование счёта
                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    Console.WriteLine($"Счёт с ID {accountId} не найден.");
                                    _logger.LogWarning($"Просмотр операций отменен: счет с ID {accountId} не найден");
                                    break;
                                }

                                GetOperationsByAccountCommand command = new(_analyticsFacade, accountId);
                                _commandInvoker.ExecuteWithTimeMeasurement(command,
                                    $"Получение операций для счета {account.Name}");

                                IEnumerable<IOperation> operations = command.GetResult();
                                List<IOperation> operationsList = operations.ToList();

                                if (operationsList.Count == 0)
                                {
                                    Console.WriteLine($"Нет выполненных операций для счета '{account.Name}'.");
                                }
                                else
                                {
                                    Console.WriteLine($"\nСписок операций для счета '{account.Name}':");
                                    Console.WriteLine("------------------------------------------------------");

                                    foreach (IOperation operation in operationsList.OrderByDescending(o => o.Date))
                                    {
                                        string categoryName = _categoryFacade.GetCategory(operation.CategoryId)?.Name ??
                                                              "Без категории";
                                        string operationType =
                                            operation.Type == OperationType.Income ? "Доход" : "Расход";

                                        Console.WriteLine(
                                            $"Дата: {operation.Date:d} | Тип: {operationType} | Сумма: {operation.Amount:C2}");
                                        Console.WriteLine($"Категория: {categoryName}");
                                        Console.WriteLine($"Описание: {operation.Description}");
                                        Console.WriteLine("------------------------------------------------------");
                                    }

                                    // Подсчитываем общую сумму доходов и расходов
                                    decimal totalIncome = operationsList.Where(o => o.Type == OperationType.Income)
                                        .Sum(o => o.Amount);
                                    decimal totalExpense = operationsList.Where(o => o.Type == OperationType.Expense)
                                        .Sum(o => o.Amount);

                                    Console.WriteLine($"Итого доходов: {totalIncome:C2}");
                                    Console.WriteLine($"Итого расходов: {totalExpense:C2}");
                                    Console.WriteLine($"Баланс счета: {account.Balance:C2}");
                                }

                                _logger.LogInformation($"Просмотр операций для счета {account.Name} (ID: {accountId})");
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при получении списка операций на счёте.");
                                _logger.LogError("Ошибка при получении списка операций на счёте", ex);
                            }

                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Показать разницу доходов и расходов за период по счёту
                    case 5:
                        {
                            try
                            {
                                Console.Write("Введите ID счёта для анализа: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    Console.WriteLine("Неверный формат ID счёта.");
                                    _logger.LogWarning("Анализ доходов и расходов отменен: введен неверный ID счета");
                                    break;
                                }

                                // Проверяем существование счёта
                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    Console.WriteLine($"Счёт с ID {accountId} не найден.");
                                    _logger.LogWarning($"Просмотр операций отменен: счет с ID {accountId} не найден");
                                    break;
                                }

                                Console.Write("Введите начальную дату периода (dd.MM.yyyy): ");
                                if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime startDate))
                                {
                                    Console.WriteLine("Неверный формат даты.");
                                    _logger.LogWarning(
                                        "Анализ доходов и расходов отменен: введена неверная дата начала периода");
                                    break;
                                }

                                Console.Write("Введите конечную дату периода (dd.MM.yyyy): ");
                                DateTime endDate;

                                while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out endDate) || endDate < startDate)
                                {
                                    Console.WriteLine(endDate < startDate
                                        ? "Конечная дата должна быть позже начальной. Пожалуйста, введите дату заново:"
                                        : "Неверный формат даты. Пожалуйста, введите дату в формате DD.MM.YYYY:");
                                }

                                endDate = endDate.AddDays(1).AddTicks(-1);

                                // Создаём и выполняем команду.
                                CalculateIncomeExpenseDifferenceCommand command =
                                    new CalculateIncomeExpenseDifferenceCommand(
                                        _analyticsFacade,
                                        accountId,
                                        startDate,
                                        endDate);

                                _commandInvoker.ExecuteWithTimeMeasurement(command,
                                    "Расчет разницы доходов и расходов");

                                // Получаем результат
                                decimal difference = command.GetResult();

                                Console.WriteLine($"\nРазница между доходами и расходами для счета '{account.Name}':");
                                Console.WriteLine($"За период с {startDate:d} по {endDate:d}: {difference:C2}");

                                switch (difference)
                                {
                                    case > 0:
                                        Console.WriteLine("У вас положительный баланс за этот период!");
                                        break;
                                    case < 0:
                                        Console.WriteLine("Внимание! Расходы превышают доходы за этот период.");
                                        break;
                                    default:
                                        Console.WriteLine("Доходы и расходы сбалансированы за этот период.");
                                        break;
                                }

                                _logger.LogInformation(
                                    $"Просмотр разницы доходов и расходов для счета {account.Name} за период {startDate:d} - {endDate:d}: {difference:C2}");
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при анализе доходов и расходов.");
                                _logger.LogError("Ошибка при анализе доходов и расходов", ex);
                            }

                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            
                            break;
                        }

                    // Группировка операций по категориям на аккаунте
                    case 6:
                        {
                            try
                            {
                                Console.Write("Введите ID счёта для анализа: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    Console.WriteLine("Неверный формат ID счёта.");
                                    _logger.LogWarning("Анализ доходов и расходов отменен: введен неверный ID счета");
                                    break;
                                }

                                // Проверяем существование счёта
                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    Console.WriteLine($"Счёт с ID {accountId} не найден.");
                                    _logger.LogWarning($"Просмотр операций отменен: счет с ID {accountId} не найден");
                                    break;
                                }

                                Console.Write("Введите начальную дату периода (dd.MM.yyyy): ");
                                DateTime startDate;
                                if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out startDate))
                                {
                                    Console.WriteLine("Неверный формат даты.");
                                    _logger.LogWarning(
                                        "Анализ доходов и расходов отменен: введена неверная дата начала периода");
                                    break;
                                }

                                Console.Write("Введите конечную дату периода (dd.MM.yyyy): ");
                                DateTime endDate;

                                while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out endDate) || endDate < startDate)
                                {
                                    Console.WriteLine(endDate < startDate
                                        ? "Конечная дата должна быть позже начальной. Пожалуйста, введите дату заново:"
                                        : "Неверный формат даты. Пожалуйста, введите дату в формате DD.MM.YYYY:");
                                }

                                endDate = endDate.AddDays(1).AddTicks(-1);

                                // Создаём и выполняем команду.
                                GroupOperationsByCategoryCommand command = new(
                                    _analyticsFacade,
                                    account.Id,
                                    startDate,
                                    endDate);

                                _commandInvoker.ExecuteWithTimeMeasurement(command,
                                    "Группировка операций по категориям");

                                Dictionary<ICategory, decimal> categorySums = command.GetResult();

                                Console.WriteLine($"\nОперации по категориям для счета '{account.Name}':");
                                Console.WriteLine($"За период с {startDate:d} по {endDate:d}:");
                                Console.WriteLine("------------------------------------------------------");

                                if (categorySums.Count == 0)
                                {
                                    Console.WriteLine("Нет данных для отображения.");
                                }
                                else
                                {
                                    // Сортируем категории: сначала доходы, потом расходы, внутри по сумме убывания
                                    List<KeyValuePair<ICategory, decimal>> sortedCategories = categorySums
                                        .OrderBy(c => c.Key.Type)
                                        .ThenByDescending(c => c.Value)
                                        .ToList();

                                    Console.WriteLine("ДОХОДЫ:");
                                    bool hasIncomes = false;
                                    decimal totalIncome = 0;
                                    foreach (KeyValuePair<ICategory, decimal> item in sortedCategories.Where(c =>
                                                 c.Key.Type == CategoryType.Income && c.Value > 0))
                                    {
                                        hasIncomes = true;
                                        Console.WriteLine($"{item.Key.Name}: {item.Value:C2}");
                                        totalIncome += item.Value;
                                    }

                                    Console.WriteLine(!hasIncomes
                                        ? "Нет доходов за выбранный период"
                                        : $"Итого доходов: {totalIncome:C2}");

                                    Console.WriteLine("\nРАСХОДЫ:");
                                    bool hasExpenses = false;
                                    decimal totalExpense = 0;
                                    foreach (KeyValuePair<ICategory, decimal> item in sortedCategories.Where(c =>
                                                 c.Key.Type == CategoryType.Expense && c.Value > 0))
                                    {
                                        hasExpenses = true;
                                        Console.WriteLine($"{item.Key.Name}: {item.Value:C2}");
                                        totalExpense += item.Value;
                                    }

                                    Console.WriteLine(!hasExpenses
                                        ? "Нет расходов за выбранный период"
                                        : $"Итого расходов: {totalExpense:C2}");

                                    // Выводим итоговую разницу
                                    if (hasIncomes || hasExpenses)
                                    {
                                        Console.WriteLine("\n------------------------------------------------------");
                                        Console.WriteLine(
                                            $"Общий итог (доходы - расходы): {totalIncome - totalExpense:C2}");
                                    }
                                }

                                _logger.LogInformation(
                                    $"Просмотр операций по категориям для счета {account.Name} за период {startDate:d} - {endDate:d}");
                            }
                            
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка при группировке операций по категориям.");
                                _logger.LogError("Ошибка при группировке операций по категориям", ex);
                            }

                            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }
                }
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
                (1, "Показать существующие счета"),
                (2, "Показать существующие категории"),
                (3, "Показать все осуществлённые операции на всех счетах"),
                (4, "Показать разницу доходов и расходов за период по счёту"),
                (5, "Показать разницу доходов и расходов за период по счёту"),
                (6, "Группировка операций по категориям на аккаунте"),
                (0, "Назад")
            ];
        }
    }
}