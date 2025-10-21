# Current Task Board

## Mission
Получить приватную сборку ObscuraX для приложения `C:\Users\ios1ph\source\repos\BoosterX_2.0` (.NET Framework 4.8), которая добавляет защиту поверх Costura/Fody и Eazfuscator.NET и не оставляет следов использования ObscuraX.

## Выполнено
- [x] Переименовали весь стек (solution, проекты, пространства имён, CLI-команду) из `BitMono` в `ObscuraX` и убедились, что сборка `dotnet build ObscuraX.sln -c Release` проходит.
- [x] Очистили CLI от баннеров/версии и отключили watermark по умолчанию (суффикс сменён на `_secured` и включается только явно).

## Текущие задачи
- [ ] Создать проект `src/ObscuraX.BoosterX` и приватный пресет `resources/presets/boosterx.yml`, чтобы прогонять цепочку после Costura+Eazfuscator.
- [ ] Составить перечень протекций, которых нет в Eazfuscator (AntiDebugBreakpoints, DotNetHook, BitDotNet, BitMethodDotnet, BillionNops, NoNamespaces, BitTimeDateStamp, ObscuraX packer) и описать порядок их активации для BoosterX.
- [ ] Проверить поддержку .NET 4.8: убедиться, что таргет `net462` в CLI/Runtime корректно обслуживает запуск на машине с 4.8, и задокументировать, какие предупреждения нужно подавлять.
- [ ] Разнести брендинг по конфигам/логам: заменить оставшиеся упоминания ObscuraX на нейтральные значения в файлах конфигурации и логировании, подготовить скрытный режим вывода.
- [ ] Подготовить end-to-end скрипт: `msbuild BoosterX_2.0.sln /p:Configuration=Release` → запуск ObscuraX-пресета → smoke-тест ключевых сценариев BoosterX с замером времени запуска и нагрузки.

## Research & Notes
- Текущие директивы Eazfuscator (`ObfuscationSettings.cs` в BoosterX): resource encryption/compression, symbol name encryption (две пароли), control-flow obfuscation, resource sanitization. Наши протекции должны дополнять, но не конфликтовать.
- Документация Eazfuscator: https://learn.gapotchenko.com/eazfuscator.net/docs (главы 4–6).
- База по ObscuraX: `docs/source/protection-list` и `docs/source/protections` содержат требования к протекциям; проверять отметки Mono/.NET Core only.
- Costura/Fody сливает зависимости в exe – нельзя ломать загрузчик и точки входа; тестировать с живым артефактом BoosterX.

## Risks
- Аgressive IL-патчи (BitDotNet/BillionNops) могут нарушить WPF binding/reflection и ухудшить производительность — вводить по одному с замером холодного старта.
- AntiDebug/DotNetHook способны триггерить антивирус; планировать код-подпись выпусков.
- Репозиторий должен оставаться приватным, без исходников BoosterX. Для тестов использовать минимальные IL-фикстуры.
- При добавлении пакетов проверять лицензии и при необходимости подключать локальные фиды, чтобы не светить приватный стек вовне.
