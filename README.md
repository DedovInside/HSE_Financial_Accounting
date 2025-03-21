﻿# HSE_Financial_Accounting
Конструирование Программного Обеспечения. Контрольная работа №1. Паттерны проектирования. 

Напомню условие: Перед вами поставлена задача разработать классы доменной модели ключевого модуля будущего приложения – модуль «Учет финансов». Доменная модель классов должна быть реализована с соблюдением принципов SOLID, ключевыми идеями GRASP: High Cohesion и Low Coupling, а также рядом паттернов GoF: порождающих; структурных и поведенческих.

Решение было разделено на два проекта:

1. `HSE_financial_accounting` - основной проект, содержащий доменную модель, все классы, интерфейсы, реализацию всех паттернов, консольный UI и т.д.
2.  `HSE_financial_accounting.Tests` проект с юнит тестами
 
## Объяснение с идеями моего решения

Я решил следовать по большей части идеям из условия. Всё было удобно разделено по отдельным папкам для читаемости.
1. Была создана доменная модель, располагающаяся в папке `Models` и содержащая три основных класса `BankAccount`, `Category`, `Operation`. Для каждого из классов реализован свой интерфейс (папка `Interfaces` в папке `Models`): `IBankAccount`, `ICategory`, `IOperation`.
2. Был реализован паттерн Абстрактная фабрика (Abstract Factory). Его реализация располагается в папке `Factories` На текущий момент он содержит два класса:
   -  `FinanceEntityCreator` - абстрактный класс, который определяет интерфейс для создания семейства связанных финансовых объектов
   -   `StandardFinanceCreator` - конкретная реализация абстрактной фабрики, создающая конкретные экземпляры финансовых объектов

 
    Таким образом, реализуется один из порождающих паттернов GoF. Это даёт нам ряд преимуществ:
      - Инкапсуляция процесса создания объектов - скрывает детали реализации
      - Возможность замены семейства объектов - можно будет заменить StandardFinanceCreator на другую реализацию
      - Согласованность создаваемых объектов - обеспечивает, что все объекты создаются в рамках одного "семейства"
      - Упрощение расширения - новые типы финансовых сущностей могут быть добавлены с минимальными изменениями

3. Для работы с нашей доменной моделью был реализован паттерн Фасад (Facade). Его реализации располагаются в папке Facades. В нём, как и по условию, упакованы все методы для работы с нашей доменной моделью. Реализация представляет собой четыре класса и четыре соответствующих интерфейса:
   - Класс `BankAccountFacade` и его интерфейс `IBankAccountFacade` - фасад для управления операциями, связанными с классом `BankAccount`: создание, удаление, обновление имени, получение конкретного аккаунта, получение всех аккаунтов
   - Класс `CategoryFacade` и его интерфейс `ICategoryFacade` - фасад для управления операциями, связанными с классом `Category`: создание, удаление, обновление имени, получение конкретной категории, получение всех категорий, получение категорий по их типу.
   - Класс `OperationFacade` и его интерфейс `IOperationFacade` - фасад для управления операциями, связанными с классом `operation`: создание, удаление, обновление описания, получение конкретной операции, получение всех операций по всем аккаунтам, получение всех операций по конкретному аккаунту, получение операций по их типу.
   - Класс `AnalyticsFacade` и его интерфейс `IAnalyticsFacade` - фасад для управления получением аналитических данных: получение всех имеющихся банковских счетов, получение всех зарегистрированных категорий, получение всех осуществлённых операций, получение всех осуществлённых операций на аккаунте, подсчёт разницы доходов и расходов за период по аккаунту, группировка операций по категориям для аккаунта.


   Как можно заметить, это очень удобно и даёт нам ряд преимуществ:
      - Фасады (`BankAccountFacade`, `CategoryFacade`, `OperationFacade`, `AnalyticsFacade`) скрывают сложную логику работы с доменной моделью
      - Каждый фасад отвечает за свой набор операций с конкретным типом сущностей
      - Фасады легко поддаются модульному тестированию
      - Добавление новой функциональности требует минимальных изменений в клиентском коде
      - Легко можно расширить аналитические возможности через `AnalyticsFacade`
      - Фасады инкапсулируют работу с репозиториями (`IRepository<T>`) (о них чуть позже)


4. Для работы с импортом данных был реализован паттерн Шаблонный метод (Template Method), поскольку в этом сценарии отличается только часть парсинга данных из разных форматов. Логика паттерна располагается в папке `DataImport`. Как он устроен:
   - Абстрактный базовый класс `DataImporter` - определяет скелет алгоритма в методе `ImportData()`. Содержит абстрактный метод `ParseFile()`, который должны реализовать наследники. Содержит приватный метод `ProcessData()` с общей для всех форматов логикой обработки данных.
   - Класс `CsvDataImporter` - парсинг CSV-файлов. Конкретная реализация.
   - Класс `JsonDataImporter` - парсинг JSON-файлов. Конкретная реализация.
   - Класс `YamlDataImporter` - парсинг YAML-файлов. Конкретная реализация.
   - Нюанс: использование DTO (Data Transfer Object). Я решил использовать данную структуру, посчитав её удобной для этого случая. У неё есть несколько важных функций:
     1. Унификация результатов парсинга: все парсеры возвращают единый формат (FinancialData). Это позволяет методу `ProcessData()` работать с любым форматом.
     2. Разделение процессов парсинга и импорта: DTO служат промежуточным слоем между внешним представлением и доменной моделью. Это упрощает добавление новых форматов импорта
     3. Упрощение маппинга данных: структура DTO (BankAccountDto, CategoryDto, OperationDto) соответствует структуре файлов. Это делает процесс десериализации более прямолинейным.
     4. Изоляция деталей хранения: FinancialData скрывает детали реализации хранения в разных форматах. Это позволяет независимо менять формат файлов и структуру доменной модели.

   
   Паттерн повзоляет достичь ряда преимуществ:
     1. Уменьшение дублирования кода: общая логика обработки данных определена один раз в базовом классе. Только специфичный код парсинга находится в наследниках.
     2. Простота добавления новых форматов: для поддержки нового формата достаточно создать новый класс-наследник. Нужно реализовать только метод `ParseFile()`.
     3. Поддержка принципа Open/Closed: код открыт для расширения (новые импортеры). Закрыт для модификации (основной алгоритм не меняется).
     4. Последовательный процесс импорта: гарантируется правильный порядок импорта (сначала счета, потом категории, затем операции). Поддерживаются зависимости между объектами.


5. Для работы с экспортом данных был реализован паттерн Посетитель (Visitor). Логика паттерна располагается в папке `DataImport`. Как он устроен:
   - Интерфейс посетителя `IExportVisitor`
   - Класс CsvExportVisitor - экспорт в CSV. Конкретный посетитель.
   - Класс JsonExportVisitor - экспорт в JSON. Конкретный посетитель.
   - Класс YamlExportVisitor - экспорт в YAML. Конкретный посетитель.
   - Клиент-координатор `DataExporter`

   Обоснование использование данного паттерна можно представить так:
   - Разделение алгоритмов и структуры данных: логика экспорта (серализации) полностью отделена от доменных объектов. Мы можем добавлять новые форматы экспорта без изменения модели данных.
   - Единый интерфейс для разных операций: независимо от формата (CSV, JSON, YAML), используется одинаковый процесс. Каждый посетитель реализует свою специфичную логику сериализации.
   - Централизованный сбор данных: каждый посетитель накапливает данные в процессе "посещения" объектов. Это позволяет эффективно организовать структуру выходного файла.
   - Соблюдение принципа единственной ответственности: каждый посетитель отвечает только за сериализацию в свой формат. `DataExporter` отвечает только за обход объектов.


6. Для реализация каждого пользовательского сценария использовались паттерны Команда (`Command`) + Декоратор (`Decorator`). Логика паттернов располагается в папке `Commands`. Как всё устроено:
   - Интерфейс команды `ICommand` — определяет общий метод `Execute()` для всех команд
   - Далее идут папки с конкретными командами:
   - `AccountsCommands` - папка, содержащая три класса-команды для работы с аккаунтами-счетами. Каждый для своей команды: `CreateBankAccountCommand`, `DeleteBankAccountCommand`, `UpdateAccountNameCommand`;
   - `CategoriesCommands` - папка, содержащая три класса-команды для работы с категориями. Каждый для своей команды: `CreateCategoryCommand`, `DeleteCategoryCommand`, `UpdateCategoryNameCommand`;
   - `OperationsCommands` - папка, содержащая три класса-команды для работы с операциями. Каждый для своей команды: `CreateOperationCommand`, `DeleteOperationCommand`, `UpdateOperationDescriptionCommand`;
   - `AnalyticsCommans` - папка, содержащая шесть классов-команд для работы с аналитикой. Каждый для своей команды: `CalculateIncomeExpenseDifferenceCommand`, `GetAllAccountsCommand`, `GetAllCategoriesCommand`, `GetAllOperationsCommand`, `GetOperationsByAccountCommand`, `GroupOperationsByCategoryCommand`;
   - `ExportCommands` - папка, содержащая один класс-команду для работы с экспортом данных в файл. `ExportDataCommand`;
   - `ImportCommands` - папка, содержащая класс команду для работы с импортом данных из файла. `ImportDataCommand`;
  
   Преимущества такого подхода можно выявить следующие:
   - Инкапсуляция операций — каждый пользовательский сценарий инкапсулирован в отдельном классе;
   - Параметризация клиентов — клиент передает все необходимые параметры при создании команды;
   - Расширяемость — добавление новых операций не требует изменения существующего кода;
   - Централизованное выполнение — все команды выполняются через единый интерфейс;

   Паттерн же декоратор имеет такую структуру:
   - Базовый компонент — интерфейс `ICommand`
   - Конкретный декоратор — `TimeMeasurementDecorator`: оборачивает любую команду. Добавляет функциональность измерения времени выполнения. Логирует результаты через переданный делегат (тут и реализация статистики из условия (я реализовал это так))
  

   Преимущества декоратора в данном случае:
   - Принцип единственной ответственности — логика измерения времени отделена от бизнес-логики;
   - Повторное использование кода — измерение времени реализовано один раз для всех команд;
   - Динамическое добавление функциональности — любую команду можно обернуть в декоратор;
   - Комбинирование декораторов — возможность создания цепочки декораторов;
  

   Как это всё взаимодействует? Класс `CommandInvoker` связывает эти два паттерна:
   - Принимает исходную команду
   - Оборачивает её в TimeMeasurementDecorator
   - Выполняет обёрнутую команду
   - Логирует результаты через внедрённый логгер (я использовал консольный в основном коде. Располагается в папке `Logging`. Интерфейс `ILogging` и класс `ConsoleLogger`)


 7. Говоря о 6 паттерне и о Прокси (Proxy) в частности, я в начале хотел реализовать его. Это можно увидеть по папке `Repositories`:
    - Интерфейс `IRepository<T>`. Определяет общий контракт для всех репозиториев: `Add(T entity)` (добавление сущности), `Update(T entity)` (обновление сущности), `Delete(Guid id)` (удаление сущности по ID), `GetById(Guid id)` — получение сущности по ID, `GetAll()` — получение всех сущностей;
    - Класс `InMemoryRepository<T>` - базовая реализация интерфейса. Хранит данные в памяти с использованием `Dictionary<Guid, T>`;
    - Классы `DataBaseRepository<T>` и `CachedRepository<T>` - заготовки, которые я в сущности и не использую. Предполагались мною, как реализация паттерна Прокси, но я не очень понял, нужна ли БД или нет, поэтому реализовал другой паттерн (о нём далее)


 8. Для реализации взаимодействия программы с пользователем я решил сделать интерактивное меню. Я решил использовать паттерн Компоновщик (Composite) для построения древовидной структуры меню. Этот паттерн позволяет создать иерархическую структуру объектов, где с отдельными и составными объектами можно работать единообразно. Логика паттерна располагается в папке `Menus`. Как всё устроено:
    - Интерфейс `IMenuComponent` - базовый интерфейс для всех элементов меню;
    - Абстрактный класс `BaseMenuComponent` - реализует общую функциональность для всех компонентов меню (Метод `RunMenu()` для отображения и обработки пользовательского ввода; базовая реализация интерфейса `IMenuComponent`; форматирование текста с использованием цветовой подсветки)
    - Композитный класс `MainMenuComposite` - реализует составной узел (композит). Хранит коллекцию дочерних компонентов `_children`. Метод `AddComponent()` для добавления новых элементов. Отображает пункты меню на основе дочерних компонентов. Делегирует выполнение выбранному дочернему компоненту.
    - Листовые компоненты (Leaf) - несколько специализированных классов меню. `AccountsMenuLeaf` - управление счетами. `CategoriesMenuLeaf` - управление категориями. `OperationsMenuLeaf` - управление операциями. `AnalyticsMenuLeaf` - аналитические функции. `ExportMenuLeaf` - функции экспорта. `ImportMenuLeaf` - функции импорта.
   
    Преимущества реализации:
    - Модульность - каждое подменю инкапсулировано в своем классе;
    - Расширяемость - легко добавлять новые пункты меню;
    - Низкая связность - подменю ничего не знают о родительском меню;
    - Единообразие использования - отображение любого меню происходит через метод Display();
    - Разделение ответственности - каждый компонент отвечает только за свою функциональность;


    Соответственно в каждом меню происходит обработка вводимых пользователем данных, подробное информирование о работе и текущем состоянии. Обеспечивается гибкая, расширяемая и удобная в сопровождении структура меню нашего приложения.

9. `Program.cs` - является точкой входа в приложение и отвечает за конфигурацию и запуск системы. Основные компоненты:
    - Использование DI-контейнера для регистрации сервисов. Получение сервисов в методе `Main`.
    - В методе `Main` также происходит построение меню с использованием паттерна Компоновщик: создание корневого компонента `MainMenuComposite`. Добавление листовых компонентов в главное меню через `mainMenu.AddComponent()`  

Таким образом, приложение соответсвует требованиям и готово к использованию. Было продемонстрировано чёткое соблюдение принципов SOLID, ключевых идей GRASP: High Cohesion и Low Coupling, а также ряда паттернов GoF.


Ну и наконец можно сказать про тестирование. Мой проект включает комплексную систему тестирования с использованием `xUnit` и библиотеки мокирования `Moq`. Основные направления тестирования:
- Тесты паттерна Команда
- Тесты доменной модели
- Тесты фасадов
- Тесты экспорта/импорта



Используемые шаблоны тестирования:
- Arrange-Act-Assert: Четкое разделение фаз теста
- Мокирование: Изоляция тестируемых компонентов
- Диспозиция: Корректная очистка временных файлов в тестах экспорта
- Проверка последовательности: Использование MockSequence для проверки порядка вызовов

Тесты обеспечивают высокое покрытие кода по условию задания и изолированную проверку функциональности каждого компонента системы, что способствует надежности приложения и позволяет безопасно вносить изменения. Для `Program.cs` и меню взаимодействий с пользователем тесты решил не писать, потому что их лучше писать для бизнес логики, а не для UI или настройки DI контейнера.
