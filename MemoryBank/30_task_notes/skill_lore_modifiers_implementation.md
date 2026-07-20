# Skill и Lore Modifiers — Implementation Plan

## Статус

Завершено 20 июля 2026 года. Vertical slice реализован, проверен и прошёл отдельное ревью перед коммитом.

## Проблема

Карточка показывает только названия trained skills и Lore. Пользователь не видит числовые modifiers, key ability и разницу между trained и untrained general skills, поэтому не может использовать карточку для проверок навыков.

## Ожидаемый результат

Backend возвращает вычисляемые modifiers и breakdown для всех general skills каталога и для каждого Lore entry персонажа первого уровня. Frontend показывает total, rank, key ability и составляющие, не пересчитывая значения самостоятельно.

## Доменная модель

- Переиспользовать `ProficiencyBonusCalculator` и `ProficiencyBasedStatistic` из задачи 3.1.
- Расширить расчёт перегрузкой, принимающей rank и source grant ids без искусственного `ProficiencyTarget`.
- General skill имеет rank Trained при наличии в `DraftCharacter.TrainedSkills`, иначе Untrained.
- Lore entries в текущем baseline существуют только как trained и используют Intelligence.
- Trained modifier первого уровня: ability modifier + 3; Untrained: только ability modifier.

## Application и API

- Добавить в `CharacterDerivedStatisticsDto` блок skill modifiers с `General` и `Lore`.
- Для каждой записи вернуть id, name, key ability, ability modifier, rank, proficiency bonus, total и source grant ids.
- General skills строятся по полному серверному `ISkillRepository`; порядок должен быть детерминированным.
- Существующий `CharacterTrainingDto` сохраняет семантику списка фактических источников training и не превращается в каталог всех навыков.
- Persistence и create request не меняются.

## Frontend

- Расширить TypeScript-контракт derived statistics.
- Заменить краткий список training в details view на числовую таблицу всех general skills и имеющихся Lore entries.
- Показывать signed total, локализованные ability/rank и полученный от сервера breakdown.
- Не вычислять rank, proficiency bonus или total на клиенте.

## Проверки

- Domain test перегрузки общего калькулятора для trained и untrained skill cases.
- Application/integration test проверяет полный каталог, разные key abilities, trained/untrained и Lore.
- Frontend unit test покрывает группировку/представление skill rows; lint, test и build проходят.
- После реализации проводится отдельное ревью API completeness, provenance и UI semantics.

## Вне scope

- skill feats;
- temporary/item/status/circumstance bonuses;
- proficiency progression и повышение Lore выше Trained;
- action rolls и DC;
- дополнительные skills из будущих книг.

## Результат реализации

- Read-модель содержит все 16 general skills server catalog и все сохранённые Lore entries.
- Trained/Untrained rank, key ability, proficiency bonus, total и provenance вычисляются на backend.
- Существующий `CharacterTrainingDto` не изменил семантику и по-прежнему описывает только фактические источники training.
- Details view показывает адаптивную двухколоночную таблицу general skills и отдельный Lore section.
- Persistence и create request не изменялись.

## Результат ревью

- Проверены trained и untrained ветви, разные key abilities и Intelligence для Lore.
- Полнота каталога закреплена integration assertion на 16 entries; порядок детерминирован по имени.
- Frontend не вычисляет rank, proficiency bonus или total и только форматирует серверный breakdown.
- Legacy/read tests, frontend lint и production build проходят; оставшихся замечаний по diff нет.
