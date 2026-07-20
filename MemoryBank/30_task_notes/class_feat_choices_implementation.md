# Class Feat Choices — Priority 5.3

## Проблема

Единый feat catalog уже содержит class feats, но создание персонажа не материализует обязательные class feat slots. Из-за этого Fighter, Ranger и Rogue можно завершить без положенного выбора, а class choices Bard, Cleric, Druid и Wizard остаются отдельными descriptors и не входят в общий feat inventory.

## План

1. Ввести stable `FeatChoice` (`SourceId`, `FeatId`) и resolver обязательных slots первого уровня.
2. Требовать top-level выборы из `CharacterClassRuleKind.ClassFeatChoice` для Fighter, Ranger и Rogue.
3. Требовать дополнительные Wizard choices для Experimental Spellshaping и School of Unified Magical Theory.
4. Материализовать fixed grants от Bard Muse, Druidic Order, Cloistered Cleric и Improved Familiar Attunement; условный Warpriest grant оставить typed/deferred до weapon rules.
5. Валидировать class trait, level, category, специальные ограничения spellshape slot и отсутствие повторов.
6. Сохранить choices как JSON-массив stable pairs через EF migration, вернуть их в `CharacterDto.Feats`, добавить wizard UI и тесты.
7. Провести отдельное review и только после полного прогона сделать коммит.

## Критерии готовности

- Fighter, Ranger и Rogue нельзя создать без одного корректного class feat.
- Wizard требует 0–2 дополнительных choices в зависимости от School/Thesis; каждый slot имеет собственный stable source id.
- Feat другого класса, уровня или недопустимый для spellshape slot отклоняется сервером.
- Granted и selected class feats имеют различимый acquisition/source provenance и переживают persistence round-trip.
- Wizard показывает только применимые class feats и включает выборы в review/details.

## Границы

Механические эффекты feats не исполняются в этом slice. Условный `Deadly Simplicity` Warpriest не выдаётся без weapon catalog и проверки favored weapon. Общий Rogue skill feat slot и feats из ancestry effects относятся к следующим slices.

## Реализовано

- `FeatChoice` хранит stable `SourceId` и `FeatId`; `DraftCharacter.SelectedClassFeatChoices` сохраняется в `jsonb` миграцией `AddClassFeatChoices`.
- `CharacterFeatResolver` выводит обязательные slots из class rules, Arcane School и Arcane Thesis, проверяет category, level, class trait, spellshape boundary и уникальность.
- Fighter, Ranger и Rogue требуют один top-level class feat. Experimental Spellshaping требует `Reach Spell` или `Widen Spell`; Unified Magical Theory добавляет независимый Wizard slot.
- Bard Muse, Druidic Order, Cloistered Cleric и Improved Familiar Attunement теперь возвращают fixed class feats как `Granted`/`ClassChoice`.
- Write-side запрещает выбрать feat, уже выданный class choice; legacy rows с пустым новым JSON-полем остаются читаемыми.
- Реализованные catalog dependencies удалены из class/Muse/Order/School/Thesis descriptors; daily thesis choice более высоких уровней остаётся deferred.
- Wizard показывает только feats своего класса, spellshape slot дополнительно ограничен двумя применимыми feats; все class choices попадают в request и review.

## Проверки

- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено, 200 tests.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 242 tests.
- `npm test -- --run` — пройдено, 79 tests.
- `npm run build` — пройдено; остаётся существующее предупреждение Vite о размере chunk.
- `dotnet ef migrations has-pending-model-changes ... --no-build` — модель синхронизирована.

## Результат review

Во время отдельного review найден и исправлен конфликт selected/granted: сочетание Unified Magical Theory и Improved Familiar Attunement больше не позволяет выбрать `Familiar` повторно. После исправления проверены completeness обязательных slots, class/spellshape availability, JSON persistence, legacy-read boundary, provenance fixed grants и очистка устаревших `ClassFeatCatalog` dependencies. Открытых замечаний не осталось.
