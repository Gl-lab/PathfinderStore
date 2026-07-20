# Saves и Perception — Implementation Plan

## Статус

Завершено 20 июля 2026 года. Vertical slice реализован, проверен и прошёл отдельное ревью перед коммитом.

## Проблема

Персонаж уже имеет итоговые ability modifiers и эффективные стартовые proficiency ranks, но карточка показывает только сами ранги. Пользователь не видит готовые к игре значения Perception, Fortitude, Reflex и Will и не может понять, из чего они сложились.

## Ожидаемый результат

Backend вычисляет четыре modifier для персонажа первого уровня и возвращает breakdown по ability и proficiency. Frontend показывает итог и составляющие в карточке персонажа. Вычисляемые значения не сохраняются и не принимаются от клиента.

## Доменная модель

- Добавить переиспользуемый `ProficiencyBonusCalculator`: `0` для Untrained, `level + 2/4/6/8` для Trained/Expert/Master/Legendary.
- Добавить модель итогового proficiency-based modifier с ability modifier, rank, proficiency bonus и total.
- Для Perception использовать Wisdom; для Fortitude — Constitution; для Reflex — Dexterity; для Will — Wisdom.
- Учитывать все effective grants через существующий `ProficiencyResolver`, включая class specialization.
- Текущий character creation baseline фиксирует level `1`; level не хранится в draft.

## Application и API

- Расширить `CharacterDerivedStatisticsDto` блоками Perception и Saving Throws.
- В breakdown вернуть target id/name, ability, ability modifier, rank, proficiency bonus, total и source grant ids.
- Переиспользовать один набор resolved proficiencies и для derived statistics, и для существующего списка proficiencies.
- Не добавлять persistence и не менять create request.

## Frontend

- Расширить TypeScript-контракт derived statistics.
- Добавить карточку Saves и Perception в details view.
- Показывать signed total, локализованные rank/ability и арифметический breakdown.
- Не пересчитывать значения на клиенте.

## Проверки

- Domain tests для каждого rank, untrained и invalid level.
- Domain tests для итогового modifier и ability mapping.
- Query/integration test подтверждает значения и применение повышающего proficiency grant.
- Frontend unit test покрывает presentation helper; lint, test и build проходят.

## Вне scope

- item, status и circumstance bonuses;
- senses и situational perception rules;
- изменения proficiency при progression;
- AC, attacks, damage и saving throw rolls.

## Результат реализации

- `ProficiencyBonusCalculator` и `ProficiencyBasedStatistic` дают единый серверный расчёт и переиспользуемую основу для Skill/Lore modifiers.
- Read-модель возвращает Perception, Fortitude, Reflex и Will с ability/proficiency breakdown и provenance.
- Details view показывает итоговые signed modifiers и серверные составляющие на русском и английском языках.
- Persistence и create request не изменялись.

## Результат ревью

- Повторное вычисление effective proficiencies в mapper устранено: один resolved набор используется и для derived statistics, и для общего списка владений.
- Подтверждено применение максимального effective rank и сохранение всех source grant ids.
- Клиент только форматирует полученные значения и не вычисляет total.
- Новых замечаний по итоговому diff не осталось.
