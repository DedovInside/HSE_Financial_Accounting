namespace HSE_financial_accounting.Menus
{
    public abstract class BaseMenuComponent : IMenuComponent
    {
        private const string HighlightColor = "\u001b[36m";
        private const string ResetColor = "\u001b[0m";
        public abstract string Name { get; }

        public abstract void Display();
        public abstract bool HandleInput(ConsoleKeyInfo key);

        protected int RunMenu((int index, string text)[] options, string prompt)
        {
            int selectedOption = 0;
            bool isSelected = false;
            Console.CursorVisible = false;

            while (!isSelected)
            {
                Console.Clear();
                Console.WriteLine(
                    $"\nИспользуйте {HighlightColor}U{ResetColor} и {HighlightColor}D{ResetColor} для навигации, {HighlightColor}Enter{ResetColor} для выбора\n");
                Console.WriteLine($"{HighlightColor}{prompt}{ResetColor}");

                for (int i = 0; i < options.Length; i++)
                {
                    string prefix = selectedOption == i ? HighlightColor : "";
                    string suffix = selectedOption == i ? ResetColor : "";
                    Console.WriteLine($"{prefix}{options[i].index}. {options[i].text}{suffix}");
                }

                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.U:
                        selectedOption = selectedOption == 0 ? options.Length - 1 : selectedOption - 1;
                        break;
                    case ConsoleKey.D:
                        selectedOption = selectedOption == options.Length - 1 ? 0 : selectedOption + 1;
                        break;
                    case ConsoleKey.Enter:
                        isSelected = true;
                        break;
                }
            }

            Console.CursorVisible = true;
            Console.WriteLine($"\n{HighlightColor}Вы выбрали действие {options[selectedOption].index}{ResetColor}");
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить:");
            Console.ReadKey();
            return options[selectedOption].index;
        }
    }
}