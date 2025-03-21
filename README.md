# BHW1
Consol App for Banking

Инструкция по запуску: есть .sln файл, который собирает проект.

a. Общая идея решения
Проект реализует систему учета банковских операций с поддержкой:

Управления данными:

Создание счетов (BankAccount), категорий (Category), операций (Operation).

Обновление баланса счетов при добавлении операций (доходы/расходы).

Аналитики:

Расчет разницы между доходами и расходами за период.

Группировка операций по категориям.

Импорта/Экспорта:

Поддержка JSON и CSV (реализовано через паттерны Visitor и Template Method).

Сохранение идентификаторов объектов и связей между ними.

Паттернов проектирования:

Команды (ICommand) с декораторами для логирования времени выполнения.

Фабрики для создания операций с валидацией.

Шаблонные методы для импорта данных.

Изменения в функционале:

Исправлен импорт JSON (десериализация вложенного свойства Data).

Добавлен полноценный CSV-импорт/экспорт с обработкой экранированных полей.

Восстановление баланса счетов при импорте данных.
