# Rogue's Racket — Implementation Plan

## Проблема

Rogue сейчас создаётся только с безусловной Dexterity key ability, а обязательный `Rogue's Racket` остаётся декларативным class choice. Из-за этого нельзя выбрать альтернативную key ability Mastermind, Ruffian или Scoundrel, применить racket skill training и выдать Ruffian proficiency в medium armor.

Простое сохранение racket id недостаточно: выбор влияет сразу на уже типизированные Ability, Skills и Proficiency subsystems. Кроме того, class/racket skill training может совпасть с Background training, а правило создания требует заменить повторный trained skill другим навыком по выбору.

## Источник и baseline

Baseline — четыре racket из `Player Core`:

| Racket | Stable id | Дополнительная key ability | Skill grants | Proficiency grants |
|---|---|---|---|---|
| Mastermind | `rogue_racket.mastermind` | `Intelligence` | Society + выбор Arcana/Nature/Occultism/Religion | — |
| Ruffian | `rogue_racket.ruffian` | `Strength` | Intimidation | Medium Armor: Trained |
| Scoundrel | `rogue_racket.scoundrel` | `Charisma` | Deception + Diplomacy | — |
| Thief | `rogue_racket.thief` | — | Thievery | — |

Dexterity остаётся доступной для любого racket. Rogue также получает фиксированный class grant Stealth независимо от racket; он входит в этот срез, поскольку участвует в том же duplicate-training resolver.

Источник: [Rogue class and Player Core rackets](https://2e.aonprd.com/Rackets.aspx).

## Ожидаемый результат

При выборе Rogue пользователь обязан выбрать один из четырёх baseline rackets и допустимую для него key ability. Сервер применяет racket skills, разрешает повторное training с Background через явный replacement choice и добавляет medium armor proficiency Ruffian.

Смена class/racket/key ability атомарно заменяет только class/racket effects и не повреждает ancestry, background или final boosts. API/read-модель и frontend показывают выбранный racket и фактически применённые typed effects.

## Граница задачи

### Входит

- нормализованный каталог четырёх `Player Core` rackets со stable ids;
- доступные key abilities: Dexterity + racket option;
- обязательный racket choice только для Rogue;
- фиксированный Rogue class training в Stealth;
- racket skill grants, включая finite choice Mastermind;
- replacement choices при duplicate training между Background и Rogue/class/racket grants;
- Ruffian grant `Medium Armor: Trained` через proficiency resolver;
- обратимая смена class/racket package;
- persistence выбранного racket и resolved skill choices;
- application/API/read-модель;
- frontend wizard, review и character details;
- EF migration, tests, MemoryBank и финальный code review.

### Не входит

- `7 + Intelligence modifier` дополнительных Rogue skills;
- Rogue feat и skill feat первого уровня;
- Sneak Attack, Surprise Attack и боевые эффекты racket;
- Mastermind Recall Knowledge/off-guard rule;
- Ruffian weapon eligibility и critical specialization;
- Scoundrel Feint/Step rule;
- Thief Dexterity-to-damage rule;
- level-up skill increases и proficiency progression;
- rackets не из `Player Core`, включая Avenger и legacy Eldritch Trickster;
- универсальный framework всех будущих class choices.

Боевые эффекты racket остаются стабильными декларативными descriptors с `ClassFeatureRules` dependency.

## Доменные решения

### Racket catalog

`RogueRacket` содержит:

- `Id`, `Name`, `Source`;
- nullable `AlternativeKeyAbility`;
- typed `SkillGrants`;
- typed `ProficiencyGrants`;
- декларативные `Effects`;
- `DeferredDependencies`.

Каталог живёт вне `DraftCharacter` и возвращает только четыре записи `Player Core`.

### Class package

`DraftCharacter` получает nullable `SelectedRogueRacketId`. Инварианты:

- для `class.rogue` racket обязателен;
- для остальных классов racket запрещён;
- Rogue key ability должна быть Dexterity либо альтернативой выбранного racket;
- Thief допускает только Dexterity;
- замена package сначала откатывает старый class boost и class/racket effects, затем применяет новый пакет;
- final free boosts временно откатываются и повторно применяются тем же способом, который уже используется при замене предшествующих packages, если это требуется текущими aggregate invariants.

### Skill training и duplicates

Текущий `TrainedSkills` становится source-aware объединением Background, фиксированного Rogue class grant и racket grants.

Правила resolver:

1. Каждый grant имеет stable `SourceGrantId`.
2. Один `SkillId` не может присутствовать в effective training дважды.
3. Если новый class/racket grant совпадает с уже применённым Background skill, запрос обязан содержать replacement skill.
4. Replacement выбирается только из general skill catalog, не может уже быть trained и не может повторяться между replacement choices.
5. Mastermind knowledge choice сначала разрешается как обычный racket grant, затем участвует в duplicate resolution.
6. Лишние, неизвестные или неполные choices отклоняются сервером.
7. Удаление Background или смена racket удаляет только grants соответствующего source и пересчитывает конфликты атомарно.

Перед реализацией нужно определить единый DTO выбора, переиспользующий форму `GrantId + TargetId`, без отдельной строки custom Lore.

### Proficiency resolution

Ruffian становится первым независимым источником поверх class baseline. Добавляется минимальный multi-source resolver:

- группировка по `Target.Id`;
- effective rank равен максимальному rank;
- sources остаются различимыми внутри доменной модели или диагностического результата;
- character read-модель возвращает один effective grant на target;
- смена Ruffian на другой racket удаляет только medium armor source.

Отдельное persistence proficiency snapshot не требуется: class и racket ids уже определяют grants.

## API-контракт

### Catalog

Предпочтительный endpoint:

```text
GET /api/classes/rogue/rackets
```

Запись содержит id, name, source, alternative key ability, typed skill/proficiency grants, declarative effects и deferred dependencies.

### Create character

В запрос добавляются nullable поля, обязательные только для Rogue:

```json
{
  "rogueRacketId": "rogue_racket.mastermind",
  "rogueTrainingChoices": [
    {
      "grantId": "rogue_racket.mastermind.skill.knowledge",
      "selectedSkillId": "skill.arcana",
      "replacementSkillId": null
    }
  ]
}
```

Replacement choices используют те же stable grant ids либо отдельный явно именованный source id, финализируемый на этапе нормализации.

### Read-модель

`CharacterClassPackageDto` получает nullable racket package. `CharacterDto.Training` возвращает объединённые skills с source ids, а `CharacterDto.Proficiencies` — effective class+racket baseline.

Legacy-персонаж Rogue без racket остаётся читаемым, но помечается неполным package; новые create requests без racket отклоняются.

## Persistence

- добавить nullable `SelectedRogueRacketId`;
- resolved skill grants продолжают сохраняться в существующем JSONB training snapshot;
- proficiency grants не дублируются в БД;
- migration создаётся только через `dotnet ef`;
- legacy rows не требуют backfill и сохраняют nullable racket id.

## Frontend

- при выборе Rogue загрузить и показать racket catalog;
- пересчитать доступные key abilities после выбора racket;
- сбрасывать недопустимую key ability при смене racket;
- показать Mastermind finite skill choice;
- запросить replacement skill для каждого конфликта с Background training;
- показать typed effects в review;
- в details отобразить racket, объединённое training и Ruffian medium armor;
- для остальных классов не показывать Rogue-specific controls.

## Этапы выполнения

1. Нормализовать четыре racket, stable grant/effect ids, key abilities и точный duplicate-resolution contract.
2. Реализовать racket catalog, domain models и unit-тесты.
3. Рефакторить source-aware skill training и добавить duplicate/replacement resolver с unit-тестами.
4. Добавить class+racket proficiency resolver и проверить Ruffian medium armor.
5. Расширить aggregate/builder/application validation и обеспечить обратимую замену package.
6. Добавить API catalog, create/read contracts и integration/controller tests.
7. Добавить EF migration и round-trip/legacy tests.
8. Обновить frontend wizard, review, details, локализацию и tests.
9. Обновить MemoryBank и выполнить backend/frontend проверки.
10. Провести отдельный code review, исправить замечания и повторить проверки.

После каждого этапа план пересматривается. Универсализация для других class choices допускается только если она уменьшает текущий код без включения Cleric/Wizard/Witch flows.

## Критерии готовности

- каталог возвращает ровно четыре `Player Core` rackets;
- Rogue невозможно создать без racket;
- non-Rogue невозможно создать с racket;
- Dexterity допустима для любого racket;
- Mastermind дополнительно допускает Intelligence, Ruffian Strength, Scoundrel Charisma, Thief не добавляет альтернативу;
- Stealth и racket skill grants применяются типизированно;
- Mastermind требует один допустимый knowledge skill;
- duplicate Background/Rogue training требует валидный replacement;
- Ruffian получает effective Medium Armor: Trained;
- смена racket удаляет старые skill/proficiency effects и корректно заменяет key ability boost;
- legacy Rogue без racket читается;
- backend/frontend checks проходят;
- финальный code review не содержит незакрытых замечаний.

## Статус

- план и предварительная граница задачи зафиксированы;
- этап 1 завершён: stable ids и duplicate replacement contract нормализованы в [`../20_domain/character_creation/rogue_rackets_player_core.md`](../20_domain/character_creation/rogue_rackets_player_core.md);
- этапы 2–8 завершены: реализованы каталог, training/proficiency resolvers, aggregate/builder, API/read-model, EF migration и frontend flow;
- миграция `AddRogueRacket` создана через `dotnet ef`;
- проверки после review: Domain 86/86, Infrastructure 83/83, frontend 23/23, production build и lint проходят;
- этап 10 завершён: code review выявил и закрыл риск очистки class/racket training при поздней замене Background; открытых замечаний нет.
