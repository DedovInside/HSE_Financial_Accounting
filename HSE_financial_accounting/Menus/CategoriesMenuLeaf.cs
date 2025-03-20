using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Commands.CategoriesCommands;
using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;

namespace HSE_financial_accounting.Menus
{
    public class CategoriesMenuLeaf : BaseMenuComponent
    {
        private readonly ICategoryFacade _categoryFacade;
        private readonly IOperationFacade _operationFacade;
        private readonly ILogger _logger;
        private readonly CommandInvoker _commandInvoker;
        
        public override string Name => "Управление категориями";

        public CategoriesMenuLeaf(ICategoryFacade categoryFacade, IOperationFacade operationFacade , ILogger logger, CommandInvoker commandInvoker)
        {
            _categoryFacade = categoryFacade;
            _operationFacade = operationFacade;
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

                switch (selectedOption)
                {
                    // Создание новой категории
                    case 1:
                        {
                            try
                            {
                                Console.Write("Введите название категории: ");
                                string name = Console.ReadLine() ?? string.Empty;

                                if (string.IsNullOrWhiteSpace(name))
                                {
                                    _logger.LogWarning("Создание категории отменено: название не указано");
                                    Console.WriteLine("Название категории не может быть пустым.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.WriteLine("Выберите тип категории:");
                                Console.WriteLine("1. Доход");
                                Console.WriteLine("2. Расход");
                                Console.Write("Ваш выбор: ");

                                if (!int.TryParse(Console.ReadLine(), out int typeChoice) ||
                                    (typeChoice != 1 && typeChoice != 2))
                                {
                                    _logger.LogWarning("Создание категории отменено: неверный тип");
                                    Console.WriteLine("Неверный выбор типа категории.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                CategoryType type = typeChoice == 1 ? CategoryType.Income : CategoryType.Expense;

                                CreateCategoryCommand commandNCreateCategoryCommand = new(_categoryFacade, name, type);
                                _commandInvoker.ExecuteWithTimeMeasurement(commandNCreateCategoryCommand,
                                    "Создание категории");

                                _logger.LogInformation($"Создана категория: {name} (тип: {type})");
                                Console.WriteLine($"Категория '{name}' успешно создана!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при создании категории", ex);
                                Console.WriteLine("Произошла ошибка при создании категории.");
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Изменить имя одной из текущих категорий
                    case 2:
                        {
                            try
                            {
                                IEnumerable<ICategory> allCategories = _categoryFacade.GetAllCategories();
                                List<ICategory> categories = allCategories.ToList();

                                if (categories.Count == 0)
                                {
                                    Console.WriteLine("Нет созданных категорий для обновления.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.WriteLine("\nСписок категорий для обновления:");
                                foreach (ICategory category in categories)
                                {
                                    Console.WriteLine(
                                        $"ID: {category.Id} | Тип: {category.Type} | Название: {category.Name}");
                                }

                                Console.Write("\nВведите ID категории для обновления: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid categoryId))
                                {
                                    _logger.LogWarning("Обновление категории отменено: неверный ID");
                                    Console.WriteLine("Неверный формат ID.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    return;
                                }

                                // Проверяем, существует ли категория
                                ICategory? categoryToUpdate = _categoryFacade.GetCategory(categoryId);
                                if (categoryToUpdate == null)
                                {
                                    _logger.LogWarning($"Категория с ID {categoryId} не найдена");
                                    Console.WriteLine($"Категория с ID {categoryId} не найдена.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.Write($"Введите новое название для категории '{categoryToUpdate.Name}': ");
                                string newName = Console.ReadLine() ?? string.Empty;

                                if (string.IsNullOrWhiteSpace(newName))
                                {
                                    _logger.LogWarning("Обновление категории отменено: название не указано");
                                    Console.WriteLine("Название категории не может быть пустым.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    return;
                                }

                                UpdateCategoryNameCommand updateCategoryNameCommand =
                                    new(_categoryFacade, categoryId, newName);
                                _commandInvoker.ExecuteWithTimeMeasurement(updateCategoryNameCommand,
                                    "Обновление названия категории");

                                _logger.LogInformation($"Категория обновлена: {categoryId}, новое название: {newName}");
                                Console.WriteLine($"Название категории успешно изменено на '{newName}'!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при обновлении категории", ex);
                                Console.WriteLine("Произошла ошибка при обновлении категории.");
                            }

                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey();
                            break;
                        }

                    // Удалить категорию по ID
                    case 3:
                        {
                            try
                            {
                                List<ICategory> categoriesToDelete = _categoryFacade.GetAllCategories().ToList();
                                if (categoriesToDelete.Count == 0)
                                {
                                    Console.WriteLine("Нет созданных категорий для удаления.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                Console.Write("\nВведите ID категории для удаления: ");
                                if (!Guid.TryParse(Console.ReadLine(), out Guid categoryId))
                                {
                                    _logger.LogWarning("Удаление категории отменено: неверный ID");
                                    Console.WriteLine("Неверный формат ID.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                ICategory? categoryToDelete = _categoryFacade.GetCategory(categoryId);
                                if (categoryToDelete == null)
                                {
                                    _logger.LogWarning($"Категория с ID {categoryId} не найдена");
                                    Console.WriteLine($"Категория с ID {categoryId} не найдена.");
                                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                    Console.ReadKey();
                                    break;
                                }

                                DeleteCategoryCommand deleteCategoryCommand =
                                    new(_categoryFacade, _operationFacade, categoryId);
                                _commandInvoker.ExecuteWithTimeMeasurement(deleteCategoryCommand, "Удаление категории");
                                _logger.LogInformation($"Категория удалена: {categoryToDelete.Name} с ID {categoryId}");
                                Console.WriteLine($"Категория '{categoryToDelete.Name}' успешно удалена!");
                            }
                            
                            catch (Exception ex)
                            {
                                _logger.LogError("Ошибка при удалении категории", ex);
                                Console.WriteLine("Произошла ошибка при удалении категории.");
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
                (1, "Создать новую категорию"),
                (2, "Просмотреть все категории"),
                (3, "Изменить имя категории"),
                (4, "Удалить категорию"),
                (0, "Назад")
            ];
        }
    }
}