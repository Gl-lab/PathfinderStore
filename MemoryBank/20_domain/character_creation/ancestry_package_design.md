# Ancestry Package Design

## Цель и граница

Документ проектирует модель полного стартового ancestry-пакета для задачи #43. Он опирается на [базовый каталог ancestry](aon_player_core_ancestries_59_64.md) и [каталог heritage/ancestry feats](ancestry_choices_player_core.md).

В границу #43 входят каталог, выбор, валидация, хранение и выдача декларативных effects. Исполнение spells, навыков, экипировки, combat-эффектов и class-specific prerequisites не входит в границу: такие effects возвращаются как декларации с известными зависимостями.

## Разделение ответственности

| Слой | Хранит | Не хранит |
|---|---|---|
| `Ancestry` catalog | правила Player Core, набор heritage/feats, grants и effect descriptors | пользовательский выбор |
| `DraftCharacter` | `AncestryType`, выбранные stable ids heritage/feat и applied free boosts | копию каталога и текст правил |
| EF Core | выборы персонажа и существующие calculated scores | hardcoded Player Core catalog |
| API | идентификаторы choices, read-модель пакета и декларации эффектов | итоговые scores, заданные клиентом |

Идентификаторы в catalog должны быть стабильными строками из [ancestry_choices_player_core.md](ancestry_choices_player_core.md), например `elf.cavern` и `human.natural_skill`. Новые публичные контракты не используют термин `Race`.

## Каталожная модель

`Ancestry` — неизменяемое определение, возвращаемое `IAncestryRepository`. Оно расширяется следующими полями.

| Поле | Тип модели | Назначение |
|---|---|---|
| `AncestryType` | существующий enum | ключ ancestry |
| `Source` | `SourceReference` | `Book`, `Page`; для текущего набора — `Player Core` |
| `AbilityBoosts`, `AbilityFlaws`, `BaseHitPoints`, `Size`, `BaseSpeed` | существующие поля | базовый числовой пакет |
| `Vision` | `VisionType` | единственный источник базового зрения |
| `StartingLanguages` | `IReadOnlyList<LanguageId>` | автоматически известные языки |
| `AdditionalLanguageRule` | `AdditionalLanguageRule` | число и пул дополнительных языков |
| `GrantedItems` | `IReadOnlyList<GrantedItem>` | декларации автоматически выдаваемых items |
| `GrantedRules` | `IReadOnlyList<GrantedRule>` | базовые rules, например `halfling.keen_eyes` |
| `Heritages` | `IReadOnlyList<Heritage>` | допустимые heritage ancestry |
| `AncestryFeats` | `IReadOnlyList<AncestryFeat>` | допустимые ancestry feats |

### Vision

```text
VisionType
  None
  LowLight
  Darkvision
```

`Darkvision` и `LowLightVision` bool-поля удаляются из новых DTO и модели после миграции потребителей. Одновременное наличие двух значений невозможно.

Базовые значения Player Core: Dwarf и Goblin — `Darkvision`; Elf и Gnome — `LowLight`; Halfling и Human — `None`. Heritage с `VisionOverride` заменяет базовое значение при построении read-модели пакета: Cavern Elf и Umbral Gnome дают `Darkvision`, Twilight Halfling — `LowLight`.

### Languages

`LanguageId` — стабильный идентификатор каталога, например `common`, `dwarven`, `elven`; не локализованная строка. Пока общего language catalog нет, список и pool хранятся в hardcoded ancestry catalog и отдаются как ids.

`AdditionalLanguageRule` содержит:

- `AdditionalLanguageRuleType`: `IntelligenceModifier` или `OnePlusIntelligenceModifier`;
- `AllowedLanguageIds` для fixed pool;
- `UsesCommonAndUncommonLanguages` для Human.

Правило Human — `OnePlusIntelligenceModifier`. Остальные базовые ancestry используют `IntelligenceModifier` и свой pool из базового каталога. Отрицательный modifier не даёт отрицательное число дополнительных языков: final count ограничивается снизу нулём.

Выбор дополнительных языков не реализуется в #43: API возвращает rule/dependency, а future language task добавит typed selection и проверку pool.

### Granted items и rules

`GrantedItem` содержит `ItemId`, `Quantity` и `Source` без денежной стоимости, инвентарного слота или пользовательского editable snapshot. Для Dwarf фиксируется `ItemId = clan_dagger`, `Quantity = 1`.

`GrantedRule` содержит `RuleId`, `EffectKind` и короткий summary для read-модели. Он не хранит исполняемый код или произвольный JSON. Пример: `halfling.keen_eyes` имеет `EffectKind = RuleEffect`.

Выдача `GrantedItem` в инвентарь и исполнение `GrantedRule` выполняются только будущими item/rule-engine задачами. #43 отображает их как ancestry-derived grants.

## Heritage и ancestry feat

### Общая база

`Heritage` и `AncestryFeat` являются immutable catalog entries и включают:

- `Id`, `AncestryType`, `Name`, `Source`;
- `Rarity` (`Common`, `Uncommon`);
- `Restrictions` и `IncompatibleChoiceIds`;
- `Effects` — `IReadOnlyList<AncestryEffectDescriptor>`;
- `DeferredDependencies` — список типов недостающих подсистем.

`AncestryFeat` дополнительно содержит `Level` и `Prerequisites`. Для #43 допустим только `Level = 1`.

`AncestryEffectDescriptor` содержит только `EffectId`, `EffectKind` и `Summary`. Допустимые значения `EffectKind`: `RuleEffect`, `VisionOverride`, `BaseHpOverride`, `DeferredChoice`. Детальные executable payloads должны появляться вместе с соответствующей подсистемой как typed effects, а не как generic JSON.

### Политика доступности

`IAncestryChoiceAvailabilityPolicy` получает catalog entry и доверенный server-side context. Начальное правило: в обычном режиме доступны только `Common` choices; `Uncommon` choices требуют явного server-side разрешения. Клиент не передаёт признак доступности.

Таким образом, `Jinxed Halfling` не может быть выбран по умолчанию. Если policy разрешает его, валидатор также отклоняет `halfling.halfling_luck`.

### Deferred choices

`DeferredChoice` не сохраняется как свободный текст. В #43 он выдаётся как requirement с `EffectId` и dependency type. Когда будет готов соответствующий каталог, отдельная задача добавит typed selection, например `SelectedSpellId`, `SelectedSkillId` или `SelectedGeneralFeatId`, и собственную валидацию.

Это особенно относится к Ancient Elf, Versatile Human, Skilled Human и Heritage/feats с выбором spell, language или skill.

## Состояние DraftCharacter и доменные операции

В aggregate добавляются:

| Свойство | Хранение | Назначение |
|---|---|---|
| `SelectedHeritageId` | nullable varchar | выбранный stable heritage id |
| `SelectedAncestryFeatId` | nullable varchar | выбранный stable ancestry feat id |

Nullable состояние допускается только внутри незавершённого draft. Создание через текущий `POST /api/character` и будущая финализация требуют оба значения.

Основная операция — `SetAncestryPackage( currentAncestry, nextAncestry, heritageId, ancestryFeatId, availabilityPolicy )`.

Она выполняется атомарно:

1. подтверждает, что `currentAncestry` соответствует текущему `AncestryType` (если персонаж уже имеет применённый пакет);
2. находит heritage и feat в `nextAncestry`;
3. проверяет `Level = 1`, policy, restrictions, несовместимости и принадлежность entries ancestry;
4. откатывает fixed boosts/flaws/free boosts предыдущего ancestry с использованием переданного `currentAncestry` catalog entry;
5. очищает `SelectedHeritageId`, `SelectedAncestryFeatId` и ancestry-derived pending choices;
6. применяет базовые boosts/flaws нового ancestry и сохраняет новые stable ids.

`ChangeAncestryType` удаляется или становится private: он не имеет права обходить применение ancestry rules.

Важное ограничение текущей модели: `_ancestry` не персистируется. Поэтому домен не может использовать его для отката после rehydration. Application handler обязан передавать `currentAncestry`, полученный из repository по сохранённому `AncestryType`; это обеспечивает одинаковую смену ancestry до и после загрузки из БД. Базовые ability adjustments остаются текущими MVP-правилами; будущий event sourcing #11 заменит ручной rollback.

## Persistence

В таблицу `character_management.Character` добавляются два nullable столбца с ограничением длины:

- `SelectedHeritageId`;
- `SelectedAncestryFeatId`.

Каталог ancestry, language pool, effects и grants не персистируется: он versioned в коде и является source of truth. Новая миграция создаётся только через `dotnet ef` после реализации модели.

`AppliedFreeBoosts` продолжает храниться отдельно. Deferred choices и grants не сохраняются в #43, поскольку для них отсутствуют typed catalogs и engine; read-модель всегда восстанавливает их по выбранному ancestry/heritage/feat.

## API-контракты

### Catalog query

`GET /api/ancestries` возвращает:

- базовые свойства ancestry, включая единый `vision`;
- starting languages, additional language rule, granted items/rules;
- доступные по server policy heritage и feats 1 уровня;
- для entries: stable id, name, rarity, prerequisites/restrictions, effect descriptors и deferred dependencies.

Не возвращать два legacy bool-поля зрения и не выдавать `Uncommon` choice как доступный без policy.

### Character creation и чтение

`CreateCharacterRequestDto` добавляет обязательные `heritageId` и `ancestryFeatId`. API принимает только ids и free boosts; calculated ability scores, vision, grants и effects не принимаются от клиента.

`CharacterDto` возвращает `ancestryType`, выбранные ids и собранный read-only `AncestryPackageDto`, в который входят базовые и heritage-overridden vision, grants, languages и декларативные effects.

## Инварианты для тестов

- выбранные heritage и feat принадлежат `AncestryType` персонажа;
- feat имеет уровень `1`;
- `Uncommon` heritage отклоняется policy по умолчанию;
- `Jinxed Halfling` и `Halfling Luck` не могут быть выбраны вместе;
- без обоих choices создание/финализация отклоняется;
- Cavern Elf, Umbral Gnome и Twilight Halfling корректно override базовое зрение;
- Unbreakable Goblin корректно override базовые ancestry HP в read-модели;
- смена ancestry после rehydration очищает прошлые choices и возвращает ability adjustments к корректному состоянию;
- grants и deferred effects строятся из каталога, а не принимаются от клиента.

## Решение о последующей реализации

Следующий шаг реализует только эту модель и перечисленные инварианты. Каждая попытка исполнить spell, class, inventory, skill или combat effect должна быть вынесена в задачу соответствующей подсистемы и подключаться через typed `AncestryEffectDescriptor`.
