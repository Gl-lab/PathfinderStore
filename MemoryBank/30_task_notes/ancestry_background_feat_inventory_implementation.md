# Ancestry и Background Feat Inventory — Priority 5.2

## Проблема

Выбранный ancestry feat сохранялся только как строка внутри ancestry package, а background skill feat оставался декларативным grant descriptor. API и карточка не различали выбранные и автоматически полученные feats, не сохраняли единый provenance и не могли разрешить обязательный выбор `Cat Fall`/`Quick Jump` у `Martial Disciple`.

## Ожидаемый результат

Каждый персонаж получает общую read-модель feats. Ancestry feat отмечен как выбранный из ancestry, background skill feat — как granted соответствующей предысторией. Оба разрешаются через единый feat catalog и показывают category, source, acquisition type и deferred dependencies. Выбор skill feat у `Martial Disciple` валидируется, сохраняется и восстанавливается после persistence round-trip.

## План реализации

1. Расширить background choice resolver, чтобы `SkillFeat` участвовал в том же catalog-option contract, что Skill и Lore choices.
2. Сохранить выбранный background skill feat id в `DraftCharacter` и добавить EF migration через `dotnet ef`.
3. Добавить `CharacterFeatResolver` с selected/granted acquisition и ancestry/background provenance.
4. Добавить `CharacterDto.Feats`, frontend contracts и отображение в wizard review/details card.
5. Покрыть fixed и choice grants, mapper, persistence и frontend behavior тестами; провести review до коммита.

## Реализованная модель

- `DraftCharacter.SelectedBackgroundSkillFeatId` хранит stable catalog id skill feat.
- `BackgroundTrainingResolver` разрешает ровно один фактический skill feat для реальных background packages; boost-only legacy test fixtures остаются совместимыми.
- Fixed grants сохраняются при создании нового персонажа. Для старых строк до миграции read resolver восстанавливает fixed background feat из каталога; неизвестный выбор старого `Martial Disciple` не угадывается.
- `CharacterFeat` содержит полное `FeatDefinition`, `AcquisitionType`, `SourceType` и `SourceId`.
- Ancestry choice возвращается как `Selected`/`Ancestry`; background skill feat — как `Granted`/`Background`.
- `CharacterDto.Feats` является общей read-моделью для списка и details API без копирования feat definitions в persistence.
- Wizard загружает ancestry и skill feat options через `GET /api/feats`, требует skill feat choice `Martial Disciple` и показывает metadata на review.
- Карточка показывает имя, category, selected/granted provenance, source book/page и явно отложенные effect dependencies.

## Persistence

- migration `AddBackgroundSkillFeat` добавляет nullable `SelectedBackgroundSkillFeatId varchar(100)` в `character_management.Character`;
- migration создана только через `dotnet ef`;
- nullable column сохраняет совместимость с существующими строками.

## Границы задачи

В задачу не входят class feat choices и class-granted feats, исполнение mechanics, general feat choices Human и перенос ancestry effects в executable rules. Эти части остаются следующими slices Priority 5.

## Проверки

- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено, 198 tests.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 242 tests.
- `npm test -- --run` — пройдено, 77 tests.
- `npm run build` — пройдено; остаётся существующее предупреждение Vite о размере chunk.
- `dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore` — пройдено.

## Результат review

Отдельное review завершено без открытых замечаний. Проверены:

- корректность selected/granted provenance и category validation в общем resolver;
- восстановление fixed background feat для legacy rows и отсутствие недостоверного fallback для choice grant;
- persistence round-trip выбранного skill feat и синхронность EF model с migration;
- обязательность выбора `Cat Fall`/`Quick Jump` в wizard;
- отображение source, acquisition type и deferred dependencies в review и карточке персонажа.
