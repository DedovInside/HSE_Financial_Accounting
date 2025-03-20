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
   3.1. Класс `BankAccountFacade` и его интерфейс `IBankAccountFacade` - фасад для управления операциями, связанными с классом `BankAccount`: создание, удаление, обновление имени, получение конкретного аккаунта, получение всех аккаунтов
   3.2. Класс `CategoryFacade` и его интерфейс `ICategoryFacade` - фасад для управления операциями, связанными с классом `Category`: создание, удаление, обновление имени, получение конкретной категории, получение всех категорий, получение категорий по их типу.
   3.3. Класс `OperationFacade` и его интерфейс `IOperationFacade` - фасад для управления операциями, связанными с классом `operation`: создание, удаление, обновление описания, получение конкретной операции, получение всех операций по всем аккаунтам, получение всех операций по конкретному аккаунту, получение операций по их типу.
   3.4. Класс `AnalyticsFacade` и его интерфейс `IAnalyticsFacade` - фасад для управления получением аналитических данных: получение всех имеющихся банковских счетов, получение всех зарегистрированных категорий, получение всех осуществлённых операций, получение всех осуществлённых операций на аккаунте, подсчёт разницы доходов и расходов за период по аккаунту, группировка операций по категориям для аккаунта.

Как можно заметить, очень удобно, что всё это даёт нам ряд преимуществ:
 1. Фасады (`BankAccountFacade`, `CategoryFacade`, `OperationFacade`, `AnalyticsFacade`) скрывают сложную логику работы с доменной моделью
 2. Каждый фасад отвечает за свой набор операций с конкретным типом сущностей
 3. Фасады легко поддаются модульному тестированию
 4. Добавление новой функциональности требует минимальных изменений в клиентском коде
 5. Легко можно расширить аналитические возможности через `AnalyticsFacade`
 6. Фасады инкапсулируют работу с репозиториями (`IRepository<T>`) (о них чуть позже)


4. Для работы с импортом данных был реализован паттерн Шаблонный метод (Template Method), поскольку в этом сценарии отличается только часть парсинга данных из разных форматов. Логика паттерна располагается в папке `DataImport`. Как он устроен:
   4.1. Абстрактный базовый класс `DataImporter` - определяет скелет алгоритма в методе `ImportData()`. Содержит абстрактный метод `ParseFile()`, который должны реализовать наследники. Содержит приватный метод `ProcessData()` с общей для всех форматов логикой обработки данных.
   4.2. Класс `CsvDataImporter` - парсинг CSV-файлов. Конкретная реализация.
   4.3. Класс `JsonDataImporter` - парсинг JSON-файлов. Конкретная реализация.
   4.4. Класс `YamlDataImporter` - парсинг YAML-файлов. Конкретная реализация.
   - Нюанс: использование DTO (Data Transfer Object). Я решил использовать данную структуру, посчитав её удобной для этого случая. У неё есть несколько важных функций:
     1. Унификация результатов парсинга: все парсеры возвращают единый формат (FinancialData). Это позволяет методу `ProcessData()` работать с любым форматом.
     2. Разделение процессов парсинга и импорта: DTO служат промежуточным слоем между внешним представлением и доменной моделью. Это упрощает добавление новых форматов импорта
     3. Упрощение маппинга данных: структура DTO (BankAccountDto, CategoryDto, OperationDto) соответствует структуре файлов. Это делает процесс десериализации более прямолинейным.
     4. Изоляция деталей хранения: FinancialData скрывает детали реализации хранения в разных форматах. Это позволяет независимо менять формат файлов и структуру доменной модели.
   - Это повзоляет достичь ряда преимуществ.
