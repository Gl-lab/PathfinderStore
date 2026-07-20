# Priority 5 — Final Cross-Review

## Цель и scope

Проверить как единый change set четыре последовательных slice Priority 5: Player Core feat catalog, общий ancestry/background feat inventory, обязательные class feat choices и поддерживаемые feat training effects. Review выполнен после отдельных plan/implementation/review/commit каждой задачи и ограничен персонажем первого уровня.

## Проверенный change set

- `ead7d56` — Player Core feat catalog: 40 ancestry, 31 skill и 47 class feat definitions;
- `7b8baa9` — общий inventory ancestry/background feats с provenance и persistence;
- `f65a2a7` — обязательные class feat choices и fixed class grants;
- `9712373` — поддерживаемые постоянные feat training effects;
- `57325b7` — исправление обязательного Rogue skill feat slot, prerequisites и frontend filtering по результатам cross-review.

## План review

1. Сверить полноту и уникальность stable ids, category, level, traits, source и typed dependencies каталога.
2. Проверить selected/granted provenance для Ancestry, Background, Class и ClassChoice sources.
3. Проверить обязательность, доступность, уникальность и persistence ancestry, background skill, class и Rogue skill feat choices.
4. Проверить supported prerequisites и границу feats, требующих дополнительного parameter choice.
5. Проверить fixed training grants, replacement conflicts, provenance и единое использование effective training в DTO и modifiers.
6. Проверить очистку зависимых choices при смене ancestry/background/class и legacy-read совместимость migrations.
7. Проверить wizard, review/details, backend/frontend tests, production builds, EF model и `git diff --check`.

## Найденные замечания

### 1. Wizard мог повторно выбрать уже выданный Familiar

Сочетание School of Unified Magical Theory и Improved Familiar Attunement позволяло выбрать `Familiar` в дополнительный school slot, хотя тот же feat уже был granted Thesis.

Исправление: write-side и frontend исключают duplicate selected/granted feat; сценарий закреплён тестом.

### 2. Martial Performance был ошибочно классифицирован как weapon proficiency

Feat не выдаёт weapon training, поэтому первоначальная dependency создавала ложное ожидание эффекта.

Исправление: dependencies отражают фактическую связь с composition spell и combat rules; неподдерживаемый weapon grant больше не заявляется.

### 3. Rogue SkillFeatChoice оставался декларативным

После первых четырёх slice Rogue требовал class feat, но его отдельное правило `SkillFeatChoice` не создавало обязательный slot. Это нарушало полноту class package и критерий prerequisite validation.

Исправление: общий resolver теперь материализует class и skill categories, Rogue требует оба выбора, frontend показывает подходящие skill feats. Поддерживаемые prerequisites на trained skill и другой feat валидируются после class training. `Assurance`, `Specialty Crafting` и `Terrain Expertise` не предлагаются, пока не смоделирован их дополнительный parameter choice.

Других открытых замечаний по catalog identity, provenance, choice cleanup, persistence, effective training и deferred boundaries не найдено.

## Результат

Priority 5 завершён. Персонаж первого уровня получает единый типизированный inventory ancestry, background skill и class feats; сервер различает selected/granted acquisition, владеет обязательными choices и поддерживаемыми prerequisites, а frontend показывает source/category/provenance и unresolved dependencies.

Домен исполняет только полностью определённые постоянные training effects: ancestry Lore feats, `Prairie Rider` и `Bardic Lore`. `Natural Skill`, `Gnome Obsession`, `Ancestral Longevity`, Human general/class choices, parameterized skill feats, weapon/combat/spell/inventory effects и higher-level progression намеренно остаются отдельными typed dependencies или будущими slices.

## Финальные проверки

- Domain tests: `205/205` passed;
- Infrastructure/API tests: `243/243` passed;
- `Pathfinder.Web` build: passed без ошибок;
- EF: модель синхронизирована с migrations;
- frontend: `80/80` tests, ESLint и production build passed;
- `git diff --check`: passed для файлов Priority 5.

Production build сохраняет существующее предупреждение Vite о размере основного chunk больше 500 kB. Существующие предупреждения `Pathfinder.Web` о legacy paging type conflicts/nullability не относятся к Priority 5. Открытых замечаний после исправлений нет.
