# Задания к модулю Interoperating with Unmanaged Code

### Задание 1

Создайте библиотеку для Power State Management на основе [Power Management API](https://msdn.microsoft.com/en-us/library/windows/desktop/bb968807(v=vs.85).aspx). Библиотека в минимальном варианте должна поддерживать следующий функционал:
*  Получение текущей информации (на основе функции CallNtPowerInformation) об управлении питанием такую как:
   *  LastSleepTime
   *  LastWakeTime
   *  SystemBatteryState 
   *  SystemPowerInformation
*  Резервировать и удалять hibernation файл (также см. функцию CallNtPowerInformation)
*  Переводить компьютер в состояние сна/гибернации (см. SetSuspendState)

*Примечание. Набор функций может быть расширен по согласованию с ментором*

### Задание 2

На основе данной библиотеки создайте COM компонент, который будет доступен из скриптовых языков и VBA (с поддержкой IDispatch)

### Задание 3

Напишите тестовые приложения и скрипты (на базе VBScript/JScript), тестирующие данные библиотеки.
