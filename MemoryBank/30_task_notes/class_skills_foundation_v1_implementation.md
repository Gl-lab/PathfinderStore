# Class Skills Foundation v1 — Implementation Plan

## Проблема

Восемь классов `Player Core` уже содержат декларативное правило дополнительных trained skills, но выбор не применяется к персонажу. Fixed class skills, ограниченный выбор Fighter и формула `N + Intelligence modifier` существуют только в текстовых summary.

Из-за этого:

- созданный персонаж не получает полный стартовый набор trained skills;
- конфликты с Background, Rogue's Racket и Deity разрешаются отдельными специальными механизмами;
- будущие `Druidic Order` и `Witch Patron` не имеют общего слоя, в который можно добавить skill grant;
- числовые skill modifiers нельзя строить на завершённом training state.

## Ожидаемый результат

После применения четырёх final boosts пользователь выбирает все обязательные class skills или Lore topics. Сервер вычисляет точное количество дополнительных выборов из итогового Intelligence modifier, объединяет class grants с уже применёнными Background/Racket/Deity grants и сохраняет effective training с provenance.

Новый character creation request без полного class-skill package отклоняется. Legacy characters без class-skill training остаются читаемыми.

## Источники и baseline

Baseline — восемь классов `Player Core`, уже нормализованные в:

- [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md);
- [`../20_domain/character_creation/skill_catalog_player_core.md`](../20_domain/character_creation/skill_catalog_player_core.md);
- [Player Core Character Creation, Step 7](https://2e.aonprd.com/Rules.aspx?ID=2027&Redirected=1);
- [Player Core Skills rules](https://2e.aonprd.com/Rules.aspx?ID=171).

Step 7 разрешает выбрать skills, а Lore rules определяют каждую Lore subcategory как отдельный skill. Поэтому дополнительные class choices допускают как один из 16 general skills, так и custom Lore topic. Ограничение только general skills было бы проектным отклонением от baseline.

Матрица v1:

| Class | Base grants | Additional skills |
|---|---|---:|
| Bard | Occultism, Performance | `4 + INT` |
| Cleric | Religion; divine skill уже выдаётся Deity | `2 + INT` |
| Druid | Nature; order skill будет отдельным source | `2 + INT` |
| Fighter | Acrobatics или Athletics | `3 + INT` |
| Ranger | Nature, Survival | `4 + INT` |
| Rogue | Stealth; racket grants уже отдельные sources | `7 + INT` |
| Witch | patron skill будет отдельным source | `3 + INT` |
| Wizard | Arcana | `2 + INT` |

## Граница задачи

### Входит

- типизированные base skill grants в `CharacterClass`;
- `AdditionalSkillCountBase` вместо разбора текстового rule summary;
- fixed и finite-choice grant, включая `Fighter: Acrobatics or Athletics`;
- replacement при конфликте base grant с ранее полученным training;
- точное число дополнительных skills/Lore после final boosts;
- general skill или custom Lore для каждого дополнительного choice;
- единая проверка уникальности и доступности effective targets;
- provenance для class base и additional grants;
- create request, builder/handler и domain application;
- class catalog API contract;
- существующая character training read-модель;
- отдельный wizard step после final boosts, review и details;
- backend/frontend tests, MemoryBank и финальный code review.

### Не входит

- `Druidic Order` и `Witch Patron` catalogs;
- higher proficiency ranks и skill increases;
- skill feats;
- skill/Lore numeric modifiers;
- level-up и retraining endpoints;
- редактирование уже сохранённого character;
- перенос training в отдельные relational tables.

## Доменные решения

### Class catalog

`CharacterClass` получает:

- `InitialSkillGrants` — ordered typed descriptors;
- `AdditionalSkillCountBase` — целое неотрицательное число.

Каждый `ClassSkillGrantDescriptor` содержит:

- stable `Id`, например `class.bard.skill.occultism`;
- один или несколько допустимых `SkillIds`;
- один option означает fixed grant;
- несколько options означают обязательный finite choice.

Additional training использует один stable source id класса, например `class.bard.skill.additional`. Несколько trained targets могут иметь одинаковый source id: provenance указывает на одно class rule, а identity training задаётся `SkillId`.

### Request choices

Create request получает:

- `ClassSkillGrantChoices` с `GrantId`, optional `SelectedSkillId` и optional replacement target;
- `AdditionalClassTrainingChoices` как ordered collection general-skill или custom-Lore targets.

`ClassTrainingTargetChoice` содержит ровно одно из полей `SkillId` или `CustomLoreTopic`. Для каждого base grant передаётся одна choice entry. Fixed grant не принимает `SelectedSkillId`; finite grant требует option из descriptor. Replacement target требуется только тогда, когда исходный target уже trained более ранним source. General-skill duplicate можно заменить general skill или Lore; duplicate Lore должен заменяться другим Lore согласно общему правилу skills.

### Порядок применения

Canonical application order:

1. Background training;
2. Racket или Deity training;
3. final boosts;
4. class base skill grants;
5. class additional skill/Lore choices.

Class training применяется отдельным методом `SetClassTraining` только после полного final boost package. Это гарантирует, что `N + Intelligence modifier` использует итоговый стартовый Intelligence.

### Resolver и атомарность

`ClassTrainingResolver` получает class catalog entry, request choices, skill catalog, итоговый Intelligence modifier и ранее применённые non-class `TrainedSkills`/`TrainedLore`.

Resolver обязан:

- проверить точное множество grant choices без пропусков и лишних ids;
- проверить finite options;
- потребовать replacement только при реальном конфликте;
- запретить replacement, если конфликта нет;
- нормализовать custom Lore через существующий `LoreTopic` policy;
- запретить неизвестные, повторные и уже trained targets в обеих training collections;
- проверить точное число `AdditionalClassTrainingChoices`;
- вернуть полный skill/Lore training result, не меняя aggregate при ошибке.

`DraftCharacter.SetClassTraining` сначала строит результат, затем атомарно заменяет только sources `class.<id>.skill.*` в `TrainedSkills` и `TrainedLore`. Background, Racket и Deity training не изменяются.

Повторный `SetFinalFreeBoosts` удаляет ранее применённые class-skill sources перед изменением ability scores. После этого class skill package должен быть применён заново с новым count.

### Persistence и legacy

Новые колонки не требуются. Effective class training сохраняется в существующих `TrainedSkills` и `TrainedLore` JSONB:

- base/replacement entry хранит descriptor `Id` как `SourceGrantId`;
- additional entries хранят `class.<class>.skill.additional`;
- general target хранится как `SkillId`;
- custom Lore хранит нормализованные `LoreId` и display name.

Legacy character с `SelectedClassId`, но без class skill source entries, читается без ошибки. Backfill не выполняется. Новые create requests всегда проходят `SetClassTraining`.

## Application и API

### Class catalog

`CharacterClassDto` получает:

- `InitialSkillGrants` с id и options;
- `AdditionalSkillCountBase`.

`AdditionalSkills` descriptor строится из того же `AdditionalSkillCountBase`, а не из второго hardcoded count. Он может сохраниться для display, но application/frontend не извлекают из `Summary` числа или skill ids.

### Create character

`CreateCharacterHandler` применяет class skills после `SetFinalFreeBoosts`.

Validation layer проверяет non-null collections и базовую форму. Полная catalog-aware проверка остаётся в domain resolver, чтобы frontend и handler не дублировали правила.

### Read model

Существующий `CharacterTrainingDto` возвращает class skills вместе с остальными sources. `CharacterClassPackageDto` получает вычисляемый `AdditionalSkillCount`, чтобы details мог объяснить правило для сохранённого level-1 character.

## Frontend flow

Wizard расширяется до девяти steps:

1. основные данные;
2. Ancestry;
3. heritage/ancestry feat;
4. ancestry boosts;
5. Background;
6. Class;
7. final boosts;
8. class skills;
9. review.

Class Skills step:

- показывает fixed grants read-only;
- показывает select для finite base grants;
- показывает replacement select только при конфликте с Background/Racket/Deity или ранее разрешённым base grant;
- показывает ordered slots дополнительных choices с выбором general skill либо вводом custom Lore topic;
- вычисляет count из `AdditionalSkillCountBase` и preview итогового Intelligence modifier;
- исключает уже trained и уже выбранные general/Lore targets;
- сбрасывается при изменении Background, Class, Racket, Deity или final boosts.

Review показывает effective class skill choices. Details продолжает использовать persisted training state и source ids.

Frontend helpers не интерпретируют class id и не читают числа из rule summaries.

## Этапы выполнения

1. Зафиксировать и проверить matrix, stable ids, ordering, duplicate/replacement policy и legacy boundary.
2. Добавить domain models, расширить `CharacterClass` и заполнить восемь catalog entries.
3. Реализовать `ClassTrainingResolver` и domain tests для fixed, finite, replacement, general/Lore additional count, reapply и invalid inputs.
4. Добавить `DraftCharacter.SetClassTraining`, builder/handler/request/validator и application/integration tests.
5. Расширить catalog/read DTO и mapper tests.
6. Подтвердить существующее JSONB persistence через round-trip tests и `dotnet ef migrations has-pending-model-changes`; migration не создавать, если model не изменился.
7. Добавить frontend types/helpers/tests и девятый wizard step, review/details и localization.
8. Обновить MemoryBank и выполнить полные проверки активного backend/frontend scope.
9. Провести отдельный code review, исправить замечания и повторить затронутые проверки.

После каждого этапа оставшийся scope пересматривается. `Druidic Order`, `Witch Patron`, numeric modifiers и feats не добавляются неявно.

## Review плана перед реализацией

Review выполнен до изменения production code. Исправлены следующие замечания:

1. **Порядок относительно final boosts.** Первоначальная идея выбора на Class step давала неверный count при финальном boost Intelligence. Class training вынесен после полного ability pipeline.
2. **Lore как допустимый skill choice.** Первоначальная граница только из 16 general skills противоречила общему правилу Player Core, где каждая Lore subcategory является отдельным skill. В request, resolver, persistence и frontend добавлен общий general-skill/custom-Lore target.
3. **Дублирование catalog data.** `AdditionalSkillCountBase` объявлен единственным источником числа; display descriptor должен строиться из него, а не содержать независимый hardcoded count.
4. **Атомарность и stale state.** Resolver сначала валидирует полный новый result. Aggregate заменяет class sources только после успеха, а повторный final boost package очищает зависимый class training.
5. **Persistence boundary.** Новые selection columns не вводятся: effective result и provenance уже помещаются в существующие `TrainedSkills`/`TrainedLore` JSONB. Требуется round-trip и pending-model проверка.

Открытых замечаний, блокирующих реализацию, после review нет.

## Статус реализации

Реализовано и проверено 14 июля 2026 года:

- typed class skill matrix и `AdditionalSkillCountBase` добавлены для восьми классов;
- domain resolver объединяет Background, Racket, Deity и class training, поддерживает replacement и custom Lore;
- create-flow применяет class training после final boosts и сохраняет effective result в существующие JSONB;
- catalog/read API возвращает class grants и вычисленное число дополнительных выборов;
- wizard расширен до девяти шагов, class training показан в review и существующей карточке training;
- EF подтвердил отсутствие изменений модели и необходимости новой migration.

## Code review реализации

Review выполнен после реализации до коммита. Найдены и исправлены:

1. Нормализованные дубли в options descriptor могли пройти первичную проверку — validation перенесена на trimmed values и добавлена проверка `skill.` prefix.
2. Frontend первоначально не учитывал custom Lore из Background при проверке повторов — добавлена совместимая с domain canonical identity и тест повтора.
3. Изменение Racket/Deity/background training после возврата на предыдущий шаг могло оставить stale class choices — добавлен reset по изменению effective previous targets.
4. Интеграционный round-trip не доказывал сохранение class-owned Lore — сценарий расширен custom `Warfare Lore` с проверкой provenance.
5. Устранено новое nullable warning в вызове `LoreTopic.CreateCustom` после явной XOR validation.

После исправлений открытых замечаний в scope 1.1 нет.

## Критерии готовности

- все восемь class entries содержат проверенную typed skill matrix;
- additional count вычисляется из итогового Intelligence modifier после final boosts;
- Fighter выбирает ровно Acrobatics или Athletics;
- fixed/finite grant conflict требует валидный unused replacement;
- additional general/Lore choices имеют точное количество, уникальны и не пересекаются с existing training;
- повторное применение заменяет только class skill sources;
- повторная замена final boosts удаляет stale class skills;
- новый create request без полного package отклоняется;
- legacy characters без class skills читаются;
- class skills и Lore сохраняются в существующих JSONB и возвращаются read-моделью;
- wizard/review/details показывают class training;
- backend/frontend checks и финальный code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review C# style, domain invariants, API compatibility и frontend reset behavior.
