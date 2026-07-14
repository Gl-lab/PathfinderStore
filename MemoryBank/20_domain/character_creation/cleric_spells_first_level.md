# Cleric spells первого уровня

## Граница каталога v1

Каталог предназначен для создания Cleric первого уровня по Player Core. Он не является полным справочником всех заклинаний Pathfinder 2e и не моделирует их игровые эффекты.

В baseline входят:

- `16` common divine cantrips rank 1 из Player Core;
- `23` common divine spells rank 1 из Player Core;
- `16` дополнительных rank-1 spells, на которые ссылаются granted spells текущего Deity catalog;
- `39` initial rank-1 focus spells текущего primary Domain catalog.

Итого каталог содержит `94` уникальных `SpellDefinition`. Player Core 2 и обычные заклинания выше первого rank в этот срез не входят. Для Metal и Wood используются initial domain spells из Divine Mysteries, потому что эти домены уже присутствуют у Green Faith в текущем deity baseline.

## Identity и metadata

Единый ID имеет вид `spell.<slug>`. `DeityGrantedSpell` и `ClericDomain.InitialFocusSpell` обязаны разрешаться в один `SpellDefinition` с тем же ID.

`SpellDefinition` хранит только данные, необходимые для выбора и отображения:

- name, rank и kind (`Cantrip`, `Spell`, `Focus`);
- исходные traditions и traits;
- rarity;
- book/page source.

Персонаж не хранит копию определения заклинания.

## Доступность Cleric

- cantrip доступен из общего списка, только если это common divine cantrip rank 1;
- prepared spell доступен из общего списка, только если это common divine spell rank 1;
- rank-1 granted spell доступен дополнительно только Cleric выбранного божества;
- granted spell при подготовке Cleric считается divine, но его исходная tradition в общем каталоге не изменяется;
- focus spells не входят в prepared spell list и применяются отдельным domain/focus flow.

Количество cantrips, prepared slots, Divine Font slots и сохранение loadout относятся к slice 2.3.

## Проверяемые инварианты

- все spell IDs уникальны;
- каждый rank-1 deity grant разрешается в каталоге;
- каждый initial domain focus spell разрешается в definition с kind `Focus`;
- клиент получает metadata через API, но не вычисляет deity-specific eligibility самостоятельно.
