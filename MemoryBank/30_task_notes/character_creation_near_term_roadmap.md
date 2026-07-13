# Character Creation Near-Term Roadmap

## Цель

Зафиксировать ближайшую последовательность задач после реализации `Ancestry`, `Background` и `Class`, чтобы следующие изменения завершали character creation вертикальными срезами и не смешивали независимые доменные подсистемы.

Roadmap отражает инженерный порядок работ. Фактический статус карточек после восстановления MCP Vikunja необходимо синхронизировать с трекером.

## Текущее состояние

Создание персонажа уже включает:

- имя и базовые сведения;
- `Ancestry`, heritage и ancestry feat;
- ancestry boosts/flaws;
- `Background` и два background boosts;
- `Class` и key ability boost;
- четыре финальных свободных boosts;
- сохранение draft через EF, API/read-модель и отображение во frontend.

Стартовый pipeline характеристик завершён. Class rules, ancestry effects и background grants пока остаются декларативными.

## Ближайшая очередь

### 1. Четыре финальных свободных boosts

Статус: реализовано. Подробности и проверки зафиксированы в [Final Free Boosts Implementation Plan](final_free_boosts_implementation.md).

Завершить стартовый pipeline характеристик отдельным обратимым пакетом из четырёх boosts.

Проверяемый результат:

- пользователь выбирает ровно четыре разные характеристики;
- каждый выбор применяется после class boost;
- замена пакета откатывает только предыдущие final boosts;
- выбор сохраняется через EF и возвращается API/read-моделью;
- wizard и карточка персонажа отображают пакет;
- доменные, интеграционные и frontend-тесты проходят.

### 2. Derived statistics v1: Hit Points

После завершения всех стартовых boosts добавить первый вычисляемый срез листа персонажа.

Ability modifiers уже вычисляются из scores и отображаются в карточке. Новая часть этого среза — maximum HP и единое использование существующего Constitution modifier.

Проверяемый результат:

- maximum HP учитывает ancestry HP, heritage override, class HP и Constitution modifier;
- derived values не принимаются от клиента и не дублируются как редактируемое состояние;
- список и карточка персонажа возвращают и отображают HP и modifiers;
- смена любого исходного package приводит к корректному пересчёту.

В эту задачу не входят AC, saves, perception и proficiency bonuses.

### 3. Skills и Lore foundation

Создать типизированный каталог skills/Lore и фактически применить background skill grants, которые сейчас возвращаются декларативно.

Проверяемый результат:

- в домене есть стабильные идентификаторы skills и отдельная модель Lore;
- background обучает указанному skill и Lore без разбора текстовых правил;
- повторяющиеся grants объединяются по явным правилам;
- trained skills сохраняются и доступны в character read-модели;
- frontend показывает skill training, полученный на этапе создания.

Skill increases, выбор дополнительных trained skills от Intelligence и расчёт полного skill modifier следует выделить отдельно, если они заметно расширят scope.

### 4. Proficiency foundation для Class

Заменить декларативные starting proficiencies классов типизированной моделью, не реализуя class features и spellcasting целиком.

Проверяемый результат:

- типизированы proficiency rank и категории: perception, saves, attacks, defenses и class DC;
- восемь классов Player Core выдают стартовые proficiencies из каталога;
- grants доступны в read-модели и отображаются в карточке;
- правила повышения proficiency по уровню остаются за пределами задачи.

### 5. Обязательные class choices

После появления общего proficiency/choice foundation разделить обязательные развилки классов на небольшие самостоятельные задачи.

Приоритетный порядок:

1. Rogue's Racket — в том числе racket-зависимая key ability;
2. Cleric doctrine и deity boundary;
3. Wizard school/thesis;
4. Witch patron;
5. spellcasting flows для Bard, Cleric, Druid, Witch и Wizard.

Каждый flow должен хранить stable id выбора и применять только те effects, для которых уже существует типизированная подсистема. Deity и spells требуют собственных каталогов и не должны добавляться строковыми заглушками.

## После ближайшей очереди

Следующими крупными направлениями останутся:

- languages и дополнительные языки от Intelligence;
- ancestry effects, которые сейчас представлены декларативно;
- feats и class features;
- equipment, starting wealth и inventory integration;
- AC, saves, perception, speed и остальные derived statistics;
- явная финализация draft и правила изменения завершённого персонажа.

Эти направления не входят в ближайшую очередь, пока не завершены первые четыре пункта roadmap.

## Правила выполнения roadmap

- Каждая строка roadmap реализуется отдельным ограниченным vertical slice: domain, persistence, API, frontend, tests и документация.
- Перед реализацией данные и неоднозначные правила нормализуются отдельным первым этапом.
- После каждого этапа оставшийся план пересматривается, но расширение scope оформляется отдельной задачей.
- После завершения всех этапов каждой задачи проводится отдельное code review и повторяются затронутые проверки.
- Известные ошибки legacy `Store.Application` не смешиваются с character creation задачами.

## Связанные документы

- [Class package implementation](class_package_implementation.md)
- [Backend MVP](mvp_character_creation_backend.md)
- [Frontend MVP](mvp_character_creation_frontend.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
- [Known gaps](../20_domain/character_creation/known_gaps.md)
