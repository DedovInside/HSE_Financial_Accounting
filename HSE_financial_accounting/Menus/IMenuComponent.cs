namespace HSE_financial_accounting.Menus
{
    public interface IMenuComponent
    {
        void Display(); // Отображает меню
        bool HandleInput(ConsoleKeyInfo key); // Обрабатывает ввод, возвращает true, если нужно выйти
        string Name { get; } // Название для отображения в родительском меню
    }
}