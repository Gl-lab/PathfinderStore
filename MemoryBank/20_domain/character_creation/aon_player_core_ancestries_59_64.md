# AoN Player Core Ancestries 59-64

## Назначение

Документ фиксирует проверенные данные по шести базовым ancestry из remastered-линейки PF2e (`Player Core`) по материалам Archives of Nethys.

Цель документа:

- дать стабильный источник данных для будущих задач по расширению каталога ancestry;
- зафиксировать доменные правила, которые прямо следуют из источника.

Расхождения между текущим кодом и remastered-данными вынесены в [implementation_notes.md](implementation_notes.md).

Дата проверки источников: `2026-03-28`.

## Источники

- `Dwarf`: <https://2e.aonprd.com/Ancestries.aspx?ID=59>
- `Elf`: <https://2e.aonprd.com/Ancestries.aspx?ID=60>
- `Gnome`: <https://2e.aonprd.com/Ancestries.aspx?ID=61>
- `Goblin`: <https://2e.aonprd.com/Ancestries.aspx?ID=62>
- `Halfling`: <https://2e.aonprd.com/Ancestries.aspx?ID=63>
- `Human`: <https://2e.aonprd.com/Ancestries.aspx?ID=64>

Все страницы помечены как не-legacy версии и ссылаются на `Player Core`.

## Нормализованная модель данных

Рекомендуемое представление ancestry для проекта:

- `Type`
- `SourceBook`
- `SourcePage`
- `BaseHitPoints`
- `Size`
- `BaseSpeed`
- `FixedBoosts`
- `FreeBoostCount`
- `Flaws`
- `Vision`
- `GrantedRules`
- `GrantedItems`
- `Languages`
- `AdditionalLanguageRule`

`GrantedRules` и `GrantedItems` нужны, потому что remastered ancestry дают не только boosts/flaws и зрение.

## Нормализованные данные

### Dwarf

- `SourceBook`: `Player Core`
- `SourcePage`: `43`
- `BaseHitPoints`: `10`
- `Size`: `Medium`
- `BaseSpeed`: `20`
- `FixedBoosts`: `Constitution`, `Wisdom`
- `FreeBoostCount`: `1`
- `Flaws`: `Charisma`
- `Vision`: `Darkvision`
- `GrantedItems`: `Clan Dagger` (получает бесплатно при создании персонажа)
- `GrantedRules`: продажа `Clan Dagger` является сильным культурным табу, но это скорее narrative/domain lore rule, а не строгий combat rule
- `Languages`: `Common`, `Dwarven`
- `AdditionalLanguageRule`: дополнительные языки = `Intelligence modifier`, список по умолчанию: `Gnomish`, `Goblin`, `Jotun`, `Orcish`, `Petran`, `Sakvroth`

### Elf

- `SourceBook`: `Player Core`
- `SourcePage`: `47`
- `BaseHitPoints`: `6`
- `Size`: `Medium`
- `BaseSpeed`: `30`
- `FixedBoosts`: `Dexterity`, `Intelligence`
- `FreeBoostCount`: `1`
- `Flaws`: `Constitution`
- `Vision`: `Low-Light Vision`
- `GrantedRules`: нет отдельного базового ancestry feature вне зрения
- `GrantedItems`: нет
- `Languages`: `Common`, `Elven`
- `AdditionalLanguageRule`: дополнительные языки = `Intelligence modifier`, список по умолчанию: `Draconic`, `Empyrean`, `Fey`, `Gnomish`, `Goblin`, `Kholo`, `Orcish`

### Gnome

- `SourceBook`: `Player Core`
- `SourcePage`: `51`
- `BaseHitPoints`: `8`
- `Size`: `Small`
- `BaseSpeed`: `25`
- `FixedBoosts`: `Constitution`, `Charisma`
- `FreeBoostCount`: `1`
- `Flaws`: `Strength`
- `Vision`: `Low-Light Vision`
- `GrantedRules`: нет отдельного базового ancestry feature вне зрения
- `GrantedItems`: нет
- `Languages`: `Common`, `Fey`, `Gnomish`
- `AdditionalLanguageRule`: дополнительные языки = `Intelligence modifier`, список по умолчанию: `Draconic`, `Dwarven`, `Elven`, `Goblin`, `Jotun`, `Orcish`

### Goblin

- `SourceBook`: `Player Core`
- `SourcePage`: `55`
- `BaseHitPoints`: `6`
- `Size`: `Small`
- `BaseSpeed`: `25`
- `FixedBoosts`: `Dexterity`, `Charisma`
- `FreeBoostCount`: `1`
- `Flaws`: `Wisdom`
- `Vision`: `Darkvision`
- `GrantedRules`: нет отдельного базового ancestry feature вне зрения
- `GrantedItems`: нет
- `Languages`: `Common`, `Goblin`
- `AdditionalLanguageRule`: дополнительные языки = `Intelligence modifier`, список по умолчанию: `Draconic`, `Dwarven`, `Gnomish`, `Halfling`, `Kholo`, `Orcish`

### Halfling

- `SourceBook`: `Player Core`
- `SourcePage`: `59`
- `BaseHitPoints`: `6`
- `Size`: `Small`
- `BaseSpeed`: `25`
- `FixedBoosts`: `Dexterity`, `Wisdom`
- `FreeBoostCount`: `1`
- `Flaws`: `Strength`
- `Vision`: `None`
- `GrantedRules`: `Keen Eyes` at level 1
- `GrantedItems`: нет
- `Languages`: `Common`, `Halfling`
- `AdditionalLanguageRule`: дополнительные языки = `Intelligence modifier`, список по умолчанию: `Dwarven`, `Elven`, `Gnomish`, `Goblin`

### Human

- `SourceBook`: `Player Core`
- `SourcePage`: `63`
- `BaseHitPoints`: `8`
- `Size`: `Medium`
- `BaseSpeed`: `25`
- `FixedBoosts`: нет
- `FreeBoostCount`: `2`
- `Flaws`: нет
- `Vision`: `None`
- `GrantedRules`: нет отдельного базового ancestry feature
- `GrantedItems`: нет
- `Languages`: `Common`
- `AdditionalLanguageRule`: дополнительные языки = `1 + Intelligence modifier`

## Доменные выводы

### 1. Базовые ancestry больше нельзя сводить только к boosts/flaws и зрению

Из этих шести ancestry уже есть как минимум два дополнительных вида стартовых эффектов:

- выдача предмета при создании (`Dwarf` -> `Clan Dagger`);
- выдача пассивного правила/feature (`Halfling` -> `Keen Eyes`).

Следствие для модели:
- если проект будет двигаться к remastered-совместимости, `Ancestry` должен поддерживать не только числовые параметры и vision-флаги;
- нужен механизм для хранения стартовых feature rules и стартовых granted items.

### 2. Vision должна быть нормализована как единое значение, а не как два bool-флага

Сейчас в домене используются `Darkvision` и `LowLightVision` как отдельные bool-поля.

Для remastered-данных устойчивее модель:
- `VisionType.None`
- `VisionType.LowLight`
- `VisionType.Darkvision`

Причина:
- у halfling в remastered-базе нет low-light vision, но есть отдельная sensory-like feature `Keen Eyes`;
- единый enum уменьшает риск противоречивых комбинаций вроде одновременных `true/true`.

### 3. Language rules являются частью ancestry-данных

Во всех шести ancestry есть стартовые языки и правило вычисления дополнительных языков.

Следствие:
- если проект будет показывать полную карточку персонажа или экспортировать sheet, `Languages` нельзя оставлять вне ancestry-каталога;
- `Human` отличается от остальных правилом `1 + Intelligence modifier`, это отдельная доменная ветка.

### 4. Free boosts остаются совместимыми с текущим MVP

По указанным remastered-страницам:
- `Human` имеет `2` free boosts;
- остальные пять ancestry имеют `2 fixed + 1 free + 1 flaw`.

Это совместимо с текущими MVP-инвариантами по распределению ancestry boosts.

### 5. Источник для будущих задач нужно считать remastered, а не legacy

Все переданные страницы ведут на актуальные версии ancestry и отдельно указывают наличие legacy-версии.

Следствие:
- для новых задач по данным ancestry безопаснее брать именно эти страницы как источник истины;
- legacy-значения не должны подмешиваться без отдельного явного решения.

## Рекомендация для будущих задач

Если задача касается только MVP создания персонажа, можно оставить текущую упрощённую модель.

Если задача касается:
- расширения каталога ancestry;
- полной карточки персонажа;
- импорта правил из AoN;
- языков, ancestry features или стартовых предметов;

то этот документ следует считать более актуальным источником, чем текущий `AncestryRepository` и часть старых правил в `domain_rules_mvp.md`.

Текущие расхождения реализации собраны в [implementation_notes.md](implementation_notes.md).
