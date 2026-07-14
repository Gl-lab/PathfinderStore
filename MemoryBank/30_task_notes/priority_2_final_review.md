# Priority 2 — Final Cross-Review

## Цель и scope

Проверить как единый change set plan commit `3203b0f` и четыре последовательных slice Priority 2: primary Domain choice, Cleric Spell Catalog v1, prepared spells с Divine Font и Domain focus spell с Focus Pool. Review выполняется после отдельных review/commit каждой подзадачи и до перехода к полезной карточке персонажа.

## План review

1. Сверить Domain, Deity и Spell catalogs по stable ids и referential integrity.
2. Проверить серверные инварианты выбора: Cloistered/Warpriest, 5 уникальных cantrips, 2 prepared slots, deity-specific access и раздельные Font/Focus resources.
3. Сопоставить create request, builder, aggregate, EF persistence, migrations и legacy read paths.
4. Проверить API и read-модели: пользовательские choices сохраняются, slot counts и Focus Pool вычисляются сервером.
5. Проверить frontend completeness, reset при смене class/Doctrine/Deity, защиту от устаревшего async response, review и details.
6. Проверить актуальность deferred dependencies и MemoryBank после завершения всех четырёх slice.
7. Повторить domain/infrastructure/frontend tests, Web build, EF model check, lint, production build и `git diff --check`.

## Review плана

План cross-review проверен относительно исходного implementation plan:

- scope ограничен Cleric первого уровня и не включает casting engine, progression, feats или equipment;
- persisted state содержит только Domain id и выбранные spell ids;
- derived Font slots, resolved definitions и Focus Pool не должны появляться в migrations;
- legacy Cleric остаётся читаемым с nullable/empty spell package;
- каждое найденное расхождение исправляется с regression-test до финального quality gate.

## Найденные замечания

### 1. Stale deferred SpellCatalog у Cleric

После реализации spell catalog и loadout `class.cleric.spellcasting` и верхнеуровневый class descriptor продолжали содержать `SpellCatalog` в `DeferredDependencies`. Frontend поэтому подписывал готовый first-level flow как отложенный.

Исправление: Cleric spellcasting descriptor теперь описывает preparation, Divine Font и Domain focus loadout без `SpellCatalog`; regression-test проверяет отсутствие stale dependency. Уже реализованные Deity и Doctrine choices также больше не объявляются deferred catalog dependencies.

### 2. Неполная legacy regression-проверка

Legacy Cleric без Doctrine уже проверял nullable Doctrine/Deity, но тест явно не фиксировал отсутствие Domain, spell loadout и Focus Pool.

Исправление: read-model test теперь проверяет все три nullable package, сохраняя границу между читаемым legacy row и строгим новым create flow.

Других открытых замечаний по domain atomicity, deity-specific spell access, duplicate prepared slots, persistence order, async reset и разделению prepared/Font/Focus ресурсов не найдено.

## Результат

Все четыре slice согласованы как единый vertical flow. Новый Cleric получает валидный Domain по Doctrine/Deity, выбирает 5 уникальных cantrips и 2 prepared rank-1 slots, получает 4 derived Heal/Harm Font slots и один initial Domain focus spell с Focus Pool `1`. После EF round-trip read-модель воспроизводит choices и вычисляет derived package из каталогов.

Финальные проверки:

- Domain tests: `155/155` passed;
- Infrastructure/API tests: `175/175` passed;
- `Pathfinder.Web` и `CharacterManagement.Infrastructure`: build passed;
- EF: `No changes have been made to the model since the last migration`;
- frontend: `55/55` tests passed, lint passed, production build passed;
- `git diff --check`: passed.

Production build сохраняет существующее предупреждение Vite о размере основного chunk; оно не блокирует сборку и не создано Priority 2. Открытых замечаний по Cleric spell flow после исправлений не осталось.
