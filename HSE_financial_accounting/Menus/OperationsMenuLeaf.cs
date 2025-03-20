using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Commands.OperationsCommands;
using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Menus
{
    public class OperationsMenuLeaf : BaseMenuComponent
    {
        private readonly IOperationFacade _operationFacade;
        private readonly IBankAccountFacade _accountFacade;
        private readonly ICategoryFacade _categoryFacade;
        
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        
        public override string Name => "Управление операциями";

        public OperationsMenuLeaf(
            IOperationFacade operationFacade, 
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            ILogger logger, 
            CommandInvoker commandInvoker)
        {
            _operationFacade = operationFacade;
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
                int selectedOption = RunMenu(options, "Управление операциями:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Возврат в главное меню");
                    break;
                }

                switch (selectedOption)
                {
                    // Создать новую операцию дохода
                    case 1:
                        {
                            try
                            {
                                OperationType type = OperationType.Income;
                                // 1. Получаем счет
                                Console.Write("Введите ID счета: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    _logger.LogWarning("Создание операции отменено: неверный ID счета");
                                    Console.WriteLine("Неверный формат ID счета.");
                                    break;
                                }

                                // Проверяем, существует ли такой счет
                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    _logger.LogWarning($"Создание операции отменено: счет с ID {accountId} не найден");
                                    Console.WriteLine($"Счет с ID {accountId} не найден.");
                                    break;
                                }

                                // 2. Получаем категорию
                                Console.Write("Введите ID категории: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid categoryId))
                                {
                                    _logger.LogWarning("Создание операции отменено: неверный ID категории");
                                    Console.WriteLine("Неверный формат ID категории.");
                                    break;
                                }

                                // Проверяем, существует ли такая категория и того ли она типа
                                ICategory? selectedCategory = _categoryFacade.GetCategory(categoryId);
                                if (selectedCategory == null || selectedCategory.Type.ToString() != type.ToString())
                                {
                                    _logger.LogWarning(
                                        $"Создание операции отменено: категория с ID {categoryId} не найдена или имеет неверный тип");
                                    Console.WriteLine(
                                        $"Категория с ID {categoryId} не найдена или имеет неверный тип.");
                                    break;
                                }

                                // 3. Получаем сумму
                                Console.Write("Введите сумму: ");
                                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                                {
                                    _logger.LogWarning("Создание операции отменено: неверная сумма");
                                    Console.WriteLine("Неверная сумма. Сумма должна быть положительным числом.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                // 4. Получаем описание (необязательное поле)
                                Console.Write("Введите описание (необязательно): ");
                                string description = Console.ReadLine() ?? "";

                                // Создаём операцию через команду
                                CreateOperationCommand command = new CreateOperationCommand(
                                    _operationFacade,
                                    type,
                                    accountId,
                                    amount,
                                    DateTime.Now,
                                    description,
                                    categoryId);

                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Создание операции");

                                _logger.LogInformation(
                                    $"Создана операция категории Доход: сумма: {amount}, счет: {account.Name}");
                                Console.WriteLine("Операция успешно создана!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при создании операции", ex);
                                Console.WriteLine("Произошла ошибка при создании операции.");
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Создать новую операцию расхода
                    case 2:
                        {
                            try
                            {
                                OperationType type = OperationType.Expense;
                                // 1. Получаем счет
                                Console.Write("Введите ID счета: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    _logger.LogWarning("Создание операции отменено: неверный ID счета");
                                    Console.WriteLine("Неверный формат ID счета.");
                                    break;
                                }

                                // Проверяем, существует ли такой счет
                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    _logger.LogWarning($"Создание операции отменено: счет с ID {accountId} не найден");
                                    Console.WriteLine($"Счет с ID {accountId} не найден.");
                                    break;
                                }

                                // 2. Получаем категорию
                                Console.Write("Введите ID категории: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid categoryId))
                                {
                                    _logger.LogWarning("Создание операции отменено: неверный ID категории");
                                    Console.WriteLine("Неверный формат ID категории.");
                                    break;
                                }

                                // Проверяем, существует ли такая категория и того ли она типа
                                ICategory? selectedCategory = _categoryFacade.GetCategory(categoryId);
                                if (selectedCategory == null || selectedCategory.Type.ToString() != type.ToString())
                                {
                                    _logger.LogWarning(
                                        $"Создание операции отменено: категория с ID {categoryId} не найдена или имеет неверный тип");
                                    Console.WriteLine(
                                        $"Категория с ID {categoryId} не найдена или имеет неверный тип.");
                                    break;
                                }

                                // 3. Получаем сумму
                                Console.Write("Введите сумму: ");
                                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                                {
                                    _logger.LogWarning("Создание операции отменено: неверная сумма");
                                    Console.WriteLine("Неверная сумма. Сумма должна быть положительным числом.");
                                    break;
                                }

                                // 5. Проверяем, достаточно ли средств на счете
                                if (account.Balance < amount)
                                {
                                    _logger.LogWarning("Создание операции отменено: недостаточно средств на счете");
                                    Console.WriteLine("Недостаточно средств на счете.");
                                    break;
                                }

                                // 4. Получаем описание (необязательное поле)
                                Console.Write("Введите описание (необязательно): ");
                                string description = Console.ReadLine() ?? "";

                                // Создаём операцию через команду
                                CreateOperationCommand command = new(
                                    _operationFacade,
                                    type,
                                    accountId,
                                    amount,
                                    DateTime.Now,
                                    description,
                                    categoryId);

                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Создание операции");

                                _logger.LogInformation(
                                    $"Создана операция категории Доход: сумма: {amount}, счет: {account.Name}");
                                Console.WriteLine("Операция успешно создана!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при создании операции", ex);
                                Console.WriteLine("Произошла ошибка при создании операции.");
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Обновить описание операции по ID
                    case 3:
                        {
                            try
                            {
                                Console.Write("Введите ID операции: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid operationId))
                                {
                                    _logger.LogWarning("Обновление описания отменено: неверный ID операции");
                                    Console.WriteLine("Неверный формат ID операции.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                // Проверяем, существует ли такая операция
                                IOperation? operation = _operationFacade.GetOperation(operationId);
                                if (operation == null)
                                {
                                    _logger.LogWarning(
                                        $"Обновление описания отменено: операция с ID {operationId} не найдена");
                                    Console.WriteLine($"Операция с ID {operationId} не найдена.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.WriteLine($"Текущее описание: {operation.Description}");
                                Console.Write("Введите новое описание: ");
                                string newDescription = Console.ReadLine() ?? "";

                                UpdateOperationDescriptionCommand command = new(_operationFacade, operationId,
                                    newDescription);
                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Обновление описания операции");

                                _logger.LogInformation($"Обновлено описание операции ID: {operationId}");
                                Console.WriteLine("Описание операции успешно обновлено!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при обновлении описания операции", ex);
                                Console.WriteLine("Произошла ошибка при обновлении описания операции.");
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Отменить операцию по ID
                    case 4:
                        {
                            try
                            {
                                Console.Write("Введите ID операции для удаления: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid operationId))
                                {
                                    _logger.LogWarning("Удаление операции отменено: неверный ID операции");
                                    Console.WriteLine("Неверный формат ID операции.");
                                    break;
                                }

                                // Проверяем, существует ли такая операция
                                IOperation? operation = _operationFacade.GetOperation(operationId);
                                if (operation == null)
                                {
                                    _logger.LogWarning(
                                        $"Удаление операции отменено: операция с ID {operationId} не найдена");
                                    Console.WriteLine($"Операция с ID {operationId} не найдена.");
                                    break;
                                }

                                // Проверяем, можно ли удалить операцию дохода, не уйдя в минус
                                if (operation.Type == OperationType.Income)
                                {
                                    IBankAccount?
                                        account = _accountFacade.GetBankAccount(operation
                                            .BankAccountId); // Аккаунт точно существует, т.к. операция существует
                                    if (account.Balance < operation.Amount)
                                    {
                                        _logger.LogWarning(
                                            "Удаление операции дохода отменено: недостаточно средств на счете");
                                        Console.WriteLine(
                                            $"Недостаточно средств на счете. Текущий баланс: {account.Balance:C2}, сумма операции: {operation.Amount:C2}");
                                        Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                        Console.ReadKey();
                                        break;
                                    }
                                }

                                // Удаляем операцию через команду
                                DeleteOperationCommand command = new(_operationFacade, operationId);
                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Удаление операции");

                                _logger.LogInformation($"Удалена операция ID: {operationId}");
                                Console.WriteLine("Операция успешно удалена!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при удалении операции", ex);
                                Console.WriteLine("Произошла ошибка при удалении операции.");
                            }
                            
                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
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
                (1, "Создать новую операцию дохода"),
                (2, "Создать новую операцию расхода"),
                (3, "Обновить описание операции по ID"),
                (4, "Отменить операцию по ID"),
                (0, "Назад")
            ];
        }
    }
}