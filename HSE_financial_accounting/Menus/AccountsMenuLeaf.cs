using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Commands.AccountsCommands;

namespace HSE_financial_accounting.Menus
{
    public class AccountsMenuLeaf : BaseMenuComponent
    {
        private readonly IBankAccountFacade _accountFacade;
        private readonly IOperationFacade _operationFacade;
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        public override string Name => "Управление счетами (банковскими аккаунтами)";

        public AccountsMenuLeaf(IBankAccountFacade accountFacade, IOperationFacade operationFacade, ILogger logger, CommandInvoker commandInvoker)
        {
            _accountFacade = accountFacade;
            _operationFacade = operationFacade;
            _logger = logger;
            _commandInvoker = commandInvoker;
        }

        public override void Display()
        {
            while (true)
            {
                (int index, string text)[] options = GetOptions();
                int selectedOption = RunMenu(options, "Управление счетами:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Возврат в главное меню");
                    break;
                }

                switch (selectedOption)
                {
                    case 1:
                        {
                            try {
                                Console.WriteLine("Введите имя счета: ");
                                string accountName = Console.ReadLine() ?? string.Empty;
                                
                                if (string.IsNullOrWhiteSpace(accountName))
                                {
                                    _logger.LogWarning("Создание счёта отменено: название не указано");
                                    Console.WriteLine("Название счёта не может быть пустым.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }
                                
                                Console.Write("Введите начальный баланс: ");
                                if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
                                {
                                    _logger.LogWarning("Создание счёта отменено: некорректный баланс");
                                    Console.WriteLine("Некорректный баланс.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }
                                
                                if (balance < 0)
                                {
                                    _logger.LogWarning("Создание счёта отменено: баланс не может быть отрицательным");
                                    Console.WriteLine("Баланс не может быть отрицательным.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }
                                
                                CreateBankAccountCommand command = new(_accountFacade, accountName, balance);
                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Создание нового счёта");
                                _logger.LogInformation($"Создан счёт: {accountName} с балансом {balance}");
                                Console.WriteLine($"Счет \"{accountName}\" успешно создан с начальным балансом {balance:C2}!");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при создании счёта", ex);
                            }
                            
                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            
                            break;
                        }
                    case 2:
                        {
                            try
                            {
                                List<IBankAccount> accounts = _accountFacade.GetAllBankAccounts().ToList();
                                if (accounts.Count == 0)
                                {
                                    _logger.LogWarning("Изменение имени счёта отменено: нет счетов для изменения");
                                    Console.WriteLine("Нет счетов для изменения.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.Write("\nВведите ID счёта для изменения имени: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    _logger.LogWarning("Изменение имени счёта отменено: некорректный ID");
                                    Console.WriteLine("Некорректный ID.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    _logger.LogWarning(
                                        $"Обновление имени счета отменено: счет с ID {accountId} не найден");
                                    Console.WriteLine($"Счет с ID {accountId} не найден.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.WriteLine($"Текущее имя счета: {account.Name}");
                                Console.Write("Введите новое имя счета: ");
                                string newName = Console.ReadLine() ?? string.Empty;

                                if (string.IsNullOrWhiteSpace(newName))
                                {
                                    _logger.LogWarning("Изменение имени счёта отменено: новое имя не указано");
                                    Console.WriteLine("Новое имя счета не может быть пустым.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                UpdateAccountNameCommand command = new(_accountFacade, accountId, newName);
                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Изменение имени счёта");
                                _logger.LogInformation($"Имя счёта с ID {accountId} изменено на \"{newName}\"");
                                Console.WriteLine($"Название счёта успешно изменено на \"{newName}\"!");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при изменении имени счёта", ex);
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }
                    case 3:
                        {
                            try
                            {
                                Console.WriteLine(
                                    $"\nВнимание! Удаление счета приведет к удалению всех связанных с ним операций.");
                                List<IBankAccount> accounts = _accountFacade.GetAllBankAccounts().ToList();
                                if (accounts.Count == 0)
                                {
                                    _logger.LogWarning("Удаление счёта отменено: нет счетов для удаления");
                                    Console.WriteLine("Нет счетов для удаления.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.Write("\nВведите ID счёта для удаления: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid accountId))
                                {
                                    _logger.LogWarning("Удаление счёта отменено: некорректный ID");
                                    Console.WriteLine("Некорректный ID.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                IBankAccount? account = _accountFacade.GetBankAccount(accountId);
                                if (account == null)
                                {
                                    _logger.LogWarning($"Удаление счета отменено: счет с ID {accountId} не найден");
                                    Console.WriteLine($"Счет с ID {accountId} не найден.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                DeleteBankAccountCommand command = new(_accountFacade, _operationFacade, accountId);
                                _commandInvoker.ExecuteWithTimeMeasurement(command, "Удаление счёта");

                                _logger.LogInformation($"Счёт с ID {accountId} удален");
                                Console.WriteLine($"Счет успешно удален со всеми операциями!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при удалении счёта", ex);
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
            // Логика обработки в Display через RunMenu
            return false; // Лист не завершает приложение напрямую
        }

        private (int index, string text)[] GetOptions()
        {
            return
            [
                (1, "Создать новый счет"),
                (2, "Изменить имя счета"),
                (3, "Удалить счет"),
                (0, "Назад")
            ];
        }
    }
}