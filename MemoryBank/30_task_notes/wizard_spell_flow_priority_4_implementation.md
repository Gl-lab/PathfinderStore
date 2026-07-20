# Wizard Spell Flow — Priority 4 Implementation Plan

## Цель

Завершить spell flow Wizard первого уровня поверх общего Player Core spell catalog: собрать spellbook, ежедневную подготовку, curriculum additions и school-specific slots, сохранить выборы и вернуть их в wizard/review/details.

## Проверенные правила Player Core

Источники: [Wizard](https://2e.aonprd.com/Classes.aspx?ID=39), стр. 192–200, и [Arcane Schools](https://2e.aonprd.com/ArcaneSchools.aspx).

- базовый spellbook содержит 10 уникальных common Arcane cantrips и 5 уникальных common Arcane rank-1 spells;
- формальная школа добавляет выбранный curriculum cantrip и 2 выбранных curriculum rank-1 spells в spellbook;
- Wizard готовит 5 уникальных cantrips и 2 rank-1 slots из spellbook; одинаковый rank-1 spell можно подготовить в оба базовых slots;
- формальная школа добавляет отдельный curriculum cantrip slot и отдельный rank-1 curriculum slot;
- Unified Magical Theory не имеет curriculum и дополнительных curriculum slots, но добавляет шестой common Arcane rank-1 spell в spellbook;
- initial school spell даёт Focus Pool 1;
- Arcane Bond даёт Drain Bonded Item; на первом уровне и обычная школа, и Unified имеют один доступный use для единственного доступного spell rank.

## Контракт

Create request хранит отдельно:

- 10 spellbook cantrips;
- 5 base rank-1 spells для формальной школы или 6 для Unified;
- curriculum cantrip и 2 curriculum rank-1 spells для формальной школы;
- 5 prepared cantrips и 2 prepared rank-1 base slots;
- prepared curriculum cantrip и prepared curriculum rank-1 school slot для формальной школы.

Сервер проверяет принадлежность Arcane tradition, common availability базовых choices, membership в spellbook/curriculum, уникальность spellbook/cantrip choices и допустимость повторов только в prepared rank-1 base slots.

Read API возвращает spellbook, prepared loadout, slot provenance, initial school focus spell, Focus Pool 1 и Arcane Bond resource. Legacy Wizard без новых persisted choices остаётся читаемым, но новый create flow требует полный loadout.

## Этапы

1. Добавить доменную модель и resolver с отдельной веткой Unified Magical Theory.
2. Добавить семь initial school focus spells в общий catalog и снять реализованные deferred dependencies.
3. Расширить aggregate, builder, validator, persistence и выполнить migration через `dotnet ef`.
4. Расширить read DTO/mapper, frontend types, wizard, review/details и локализацию.
5. Добавить domain, application/infrastructure и frontend tests.
6. Выполнить review задачи, исправить найденное, прогнать все релевантные gates и сделать отдельный commit.

## Границы

Advanced school spells, дополнительные spells/slots более высоких уровней, выбор дополнительного Unified class feat, фактическое расходование Drain Bonded Item, предмет spellbook и отложенные механики Arcane Thesis в этот срез не входят.

## Результат

Завершено 20 июля 2026 года.

- реализован server-owned `WizardSpellLoadoutResolver` для формальных школ и Unified Magical Theory;
- spellbook, curriculum additions, prepared base slots и school slots валидируются, сохраняются в JSONB/scalar columns и возвращаются в read API;
- общий каталог расширен семью initial Wizard school focus spells; initial school benefits больше не помечены как deferred;
- read package возвращает Focus Pool 1, initial school spell и одно применение Drain Bonded Item на первом уровне;
- wizard, review и character details показывают spellbook, curriculum, подготовленные spells и school magic;
- migration `20260720135602_AddWizardSpellLoadout` создана через `dotnet ef`; pending model changes отсутствуют;
- legacy Wizard без нового loadout остаётся читаемым, а новый HTTP create contract требует полный набор Wizard spell choices.

## Review задачи

Review выполнен после реализации. Проверены formal/Unified branches, provenance school slots, допустимый повтор одного spell в двух base prepared slots, очистка состояния при смене class/School, nullable legacy read и round-trip persistence/read API.

В review исправлена граница school slot: подготовленный curriculum spell проверяется по полному curriculum выбранной школы, а не только по двум стартовым curriculum spells, добавленным в spellbook. Это соответствует правилу Player Core о допустимости любого curriculum spell подходящего rank независимо от способа его добавления в spellbook.

Проверки:

- `CharacterManagement.Domain.Tests`: 194/194;
- `CharacterManagement.Infrastructure.Tests`: 223/223;
- frontend Vitest: 75/75;
- frontend ESLint: успешно;
- frontend production build: успешно; остаётся существующее предупреждение Vite о chunk больше 500 kB;
- `dotnet ef migrations has-pending-model-changes`: изменений модели после migration нет.
