# Priority 3 — Final Cross-Review

## Цель и scope

Проверить как единый change set два последовательных slice Priority 3: Saves/Perception и Skill/Lore modifiers. Review выполняется после отдельных review/commit каждой задачи и ограничен вычисляемой карточкой персонажа первого уровня.

## План review

1. Сверить формулу proficiency bonus и ability mapping для Perception, Fortitude, Reflex, Will, general skills и Lore.
2. Проверить единый level-1 baseline, effective proficiency grants и provenance training sources.
3. Проверить API completeness: breakdown вычисляется сервером, не сохраняется и не принимается create request.
4. Проверить полный каталог general skills, Untrained entries и Intelligence для Lore.
5. Проверить frontend semantics, локализацию rank/ability, отсутствие клиентского пересчёта и responsive layout.
6. Проверить legacy/read paths и обязательные зависимости mapper’ов.
7. Повторить domain/infrastructure tests, Web/Infrastructure build, EF model check, frontend test/lint/build и `git diff --check`.

## Найденные замечания

### 1. Дублирование level-1 baseline

Saves/Perception и Skill/Lore mapper’ы независимо хранили константу уровня `1`. При будущем добавлении progression это могло привести к разным формулам внутри одной read-модели.

Исправление: уровень определяется один раз в `CharacterDerivedStatisticsDtoMapper` и передаётся в skill mapper явным параметром.

### 2. Не был закреплён контракт skill catalog dependency

Полный список general skills требует `ISkillRepository`, но regression-test не фиксировал понятную ошибку конфигурации при его отсутствии.

Исправление: добавлен mapper test, проверяющий явный `InvalidOperationException` для complete character без skill repository.

### 3. Устаревший обзор проекта

`project_overview.md` и инженерная карта всё ещё описывали часть завершённого Cleric flow и derived statistics как незавершённые.

Исправление: актуальный character creation focus синхронизирован с завершёнными приоритетами 2 и 3; в открытых границах оставлены только действительно не реализованные combat statistics и progression.

Других открытых замечаний по формулам, source provenance, trained/untrained semantics, API contract и responsive presentation не найдено.

## Результат

Priority 3 завершён как единая server-derived character card. Perception, три saves, все 16 general skills и сохранённые Lore entries возвращают total и объяснимый ability/proficiency breakdown. Frontend отображает готовые значения, не владея формулой.

Финальные проверки:

- Domain tests: `178/178` passed;
- Infrastructure/API tests: `206/206` passed;
- `Pathfinder.Web` и `CharacterManagement.Infrastructure`: build passed без ошибок;
- EF: `No changes have been made to the model since the last migration`;
- frontend: `66/66` tests passed, lint passed, production build passed;
- `git diff --check`: passed.

Production build сохраняет существующее предупреждение Vite о размере основного chunk; оно не блокирует сборку и не создано Priority 3. Открытых замечаний по полезной карточке персонажа после исправлений не осталось.
