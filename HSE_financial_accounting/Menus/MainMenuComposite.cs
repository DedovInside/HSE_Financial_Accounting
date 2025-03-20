using HSE_financial_accounting.Logging;
namespace HSE_financial_accounting.Menus
{
    public class MainMenuComposite : BaseMenuComponent
    {
        private readonly List<IMenuComponent> _children = new();
        private readonly ILogger _logger;
        
        private (int index, string text)[] GetOptions()
        {
            return _children.Select((child, index) => (index + 1, child.Name))
                .Append((0, "Выход"))
                .ToArray();
        }
        
        public override string Name => "Главное меню";

        public MainMenuComposite(ILogger logger)
        {
            _logger = logger;
        }
        
        public void AddComponent(IMenuComponent component)
        {
            _children.Add(component);
        }

        public override void Display()
        {
            while (true)
            {
                (int, string)[] options = _children.Select((child, index) => (index + 1, child.Name))
                    .Append((0, "Выход"))
                    .ToArray();
                int selectedOption = RunMenu(options, "Выберите действие:");

                if (selectedOption == 0)
                {
                    _logger.LogInformation("Выход из приложения");
                    return;
                }

                IMenuComponent selectedChild = _children[selectedOption - 1];
                _logger.LogInformation($"Переход в меню: {selectedChild.Name}");
                selectedChild.Display();
            }
        }
        
        public override bool HandleInput(ConsoleKeyInfo key)
        {
            // Главное меню не обрабатывает ввод напрямую, это делает RunMenu
            return key.Key == ConsoleKey.Enter && RunMenu(GetOptions(), "Выберите действие:") == 0;
        }
    }
}