# Character Creation And Gameplay Roadmap

## Цель

Зафиксировать актуальную последовательность развития проекта от завершённого character creation к боевой карточке, кампаниям, партиям, предметам, инвентарю и торговле. Roadmap отделяет уже реализованный фундамент от следующих продуктовых целей и не смешивает независимые подсистемы правил, хранения и игрового процесса.

Vikunja остаётся источником истины по карточкам и их статусам. Перед созданием задач из этого roadmap необходимо получить список задач проекта `Pathfinder`, исключить дубли и после записи перепроверить UTF-8. Пока синхронизация с Vikunja не выполнена, новые пункты ниже считаются подготовленными task slices, а не подтверждёнными карточками трекера.

## Актуальное состояние

Создание персонажа уже включает:

- имя, концепцию и возраст;
- `Ancestry`, heritage, ancestry feat и ancestry boosts/flaws;
- `Background`, два boosts, фактический skill/Lore training и background skill feat;
- `Class`, key ability boost и четыре финальных свободных boosts;
- типизированные стартовые proficiencies;
- обязательные class choices всех восьми классов Player Core baseline;
- divine skill replacement, favored weapon proficiency, Divine Font и sanctification;
- полный Cleric spell loadout первого уровня: primary Domain, 5 cantrips, 2 prepared spells, 4 derived Font slots и Domain focus spell с Focus Pool `1`;
- единый Player Core feat catalog и inventory выбранных/granted ancestry, background skill и class feats;
- Player Core language catalog, starting/additional languages и server-side validation count/pool;
- completion report и явный persisted status `Draft`/`Completed` с owner-scoped финализацией;
- сохранение через EF, API/read-модели, список, карточку и удаление во frontend;
- вычисляемые ability modifiers и maximum HP первого уровня.

Стартовый pipeline характеристик, class skills, обязательных классовых выборов, spell loadout, feat inventory, languages, финализации и starting equipment первого уровня завершён. Поддерживаемые постоянные training effects feats влияют на Skills/Lore и modifiers; остальные spell/action/combat effects остаются typed dependencies. Runtime inventory, spellcasting lifecycle и большинство combat statistics ещё не реализованы.

## Завершённый фундамент

1. `Ancestry`, heritage и ancestry feat package — реализовано.
2. `Background` package — реализовано.
3. `Class` package и key ability boost — реализовано.
4. Четыре финальных свободных boosts — реализовано; см. [Final Free Boosts Implementation Plan](final_free_boosts_implementation.md).
5. Derived statistics v1: maximum HP — реализовано; см. [Hit Points v1 Implementation Plan](hit_points_v1_implementation.md).
6. Skills и Lore foundation для Background — реализовано; см. [Skills и Lore Foundation Implementation Plan](skills_lore_foundation_implementation.md).
7. Class proficiency foundation — реализовано; см. [Class Proficiency Foundation Implementation Plan](class_proficiency_foundation_implementation.md).
8. `Rogue's Racket` — реализовано; см. [Rogue's Racket Implementation Plan](rogue_racket_implementation.md).
9. `Cleric Doctrine` — реализовано; см. [Cleric Doctrine Implementation Plan](cleric_doctrine_implementation.md).
10. Player Core Deity catalog и обязательный выбор Deity — реализовано; см. [Cleric Deity Implementation Plan](cleric_deity_implementation.md).

## Зафиксированный порядок ближайших приоритетов

### Приоритет 1 — обязательные классовые выборы Player Core

**Цель.** Сделать class package структурно полным для всех восьми классов текущего `Player Core` baseline: каждый обязательный выбор первого уровня должен быть сделан, проверен сервером, сохранён и показан пользователю.

Это не означает полное покрытие всех правил `Player Core`. В этот приоритет не входят class feats, spell preparation, equipment и исполнение combat effects. `Fighter` не имеет отдельной классовой развилки первого уровня; его class feat относится к будущей feat subsystem. `Rogue's Racket`, `Cleric Doctrine` и `Cleric Deity` уже завершены.

#### 1.1. Class Skills Foundation v1

**Статус:** завершено 14 июля 2026 года; см. [Implementation Plan](class_skills_foundation_v1_implementation.md).

**Проблема.** `Druidic Order` и `Witch Patron` выдают skill training, а все классы используют fixed skills и формулу `N + Intelligence modifier`. Текущие Background, Racket и Deity resolvers не образуют общего class-skill flow.

**Ожидаемый результат.** После final boosts пользователь выбирает точное число class skills, а сервер объединяет все источники training и replacements с сохранением provenance.

**Критерии готовности:**

- для восьми классов нормализованы fixed skills и формулы дополнительных trained skills;
- количество выборов вычисляется из итогового Intelligence modifier;
- Background, Racket, Deity и class grants объединяются без дубликатов и stale choices;
- выборы валидируются, сохраняются и показываются в wizard, review и details;
- higher ranks, skill feats и числовые modifiers остаются за пределами задачи.

#### 1.2. Ranger Hunter's Edge

**Статус:** завершено 14 июля 2026 года; см. [Implementation Plan](ranger_hunters_edge_implementation.md).

**Проблема.** Ranger сохраняется без обязательного Hunter's Edge.

**Ожидаемый результат.** Ranger обязан выбрать Edge из проверенного Player Core catalog; stable id сохраняется и возвращается frontend, а combat effects остаются typed descriptors.

**Критерии готовности:**

- Ranger невозможно создать без Edge, а non-Ranger — с Edge;
- смена класса очищает выбор;
- persistence, API, wizard, review/details и tests покрывают vertical slice;
- action/combat subsystem не имитируется.

#### 1.3. Druidic Order

**Статус:** завершено 14 июля 2026 года; см. [Implementation Plan](druidic_order_implementation.md).

**Проблема.** Обязательный Order пока существует только как декларативная зависимость.

**Ожидаемый результат.** Druid выбирает Order; его trained skill применяется через общий resolver, а class feat и order spell возвращаются typed descriptors.

**Критерии готовности:**

- Druid требует Order, остальные классы запрещают его;
- skill grant и replacement используют Class Skills Foundation v1;
- выбор сохраняется и отображается во всём create/read flow;
- animal companion, feat и focus-spell mechanics не реализуются преждевременно.

#### 1.4. Bard Muse

**Статус:** завершено 14 июля 2026 года; см. [Implementation Plan](bard_muse_implementation.md).

**Проблема.** Bard не требует Muse, хотя этот выбор на первом уровне определяет granted class feat и дополнительный spell repertoire entry.

**Ожидаемый результат.** Bard выбирает Muse из типизированного каталога; выбор сохраняется, а feat и spell references возвращаются как проверенные descriptors без исполнения отсутствующих подсистем.

**Критерии готовности:**

- Bard требует Muse, остальные классы запрещают его;
- catalog использует stable ids для Muse, granted feat и spell reference;
- persistence, API, wizard, review/details и tests покрывают выбор и смену класса;
- feat application и spell repertoire остаются в следующих приоритетах.

#### 1.5. Witch Patron

**Статус:** завершено 14 июля 2026 года; см. [Implementation Plan](witch_patron_implementation.md).

**Проблема.** Witch сохраняется без Patron, поэтому не определены spell tradition, granted skill, lesson и familiar ability.

**Ожидаемый результат.** Patron обязателен для Witch; tradition и skill grant применяются типизированно, а lesson, familiar ability и spells остаются проверенными descriptors.

**Критерии готовности:**

- Witch требует Patron, остальные классы запрещают его;
- tradition и trained skill вычисляются из catalog entry, а не принимаются отдельно от клиента;
- skill conflict использует общий resolver;
- persistence, API, wizard, review/details и tests покрывают полный выбор;
- familiar и spell execution остаются отдельными подсистемами.

#### 1.6. Wizard Arcane School

**Статус:** завершено 14 июля 2026 года; см. [план и результат реализации](wizard_arcane_school_implementation.md).

**Проблема.** Wizard не требует Arcane School, поэтому отсутствует обязательный источник curriculum spells и school-specific benefits.

**Ожидаемый результат.** Wizard выбирает School по stable id; curriculum и granted benefits доступны typed descriptors, но spellbook и дополнительные slots ещё не применяются.

**Критерии готовности:**

- Wizard требует School, остальные классы запрещают его;
- curriculum references нормализованы без разбора текста;
- выбор сохраняется и отображается в create/read flow;
- фактическое наполнение spellbook и slots входит в будущий Wizard spell flow, а не в задачу выбора School.

#### 1.7. Wizard Arcane Thesis

**Статус:** завершено 14 июля 2026 года; см. [план и результат реализации](wizard_arcane_thesis_implementation.md).

**Проблема.** Обязательная Arcane Thesis отсутствует, а её разнородные эффекты нельзя смешивать с School curriculum в одной задаче.

**Ожидаемый результат.** Wizard независимо выбирает Thesis; supported effects типизированы, остальные имеют явные deferred dependencies.

**Критерии готовности:**

- Wizard требует Thesis одновременно со School;
- Thesis и School можно валидировать и заменять независимо;
- persistence, API, wizard, review/details и tests покрывают выбор;
- spell/familiar/item mechanics не имитируются частично.

**Статус приоритета 1:** завершён 14 июля 2026 года; выполнен [итоговый cross-review](priority_1_final_review.md). Новый персонаж каждого из восьми классов имеет все обязательные классовые выборы первого уровня. Feats и spell loadout остаются явно незавершёнными областями, а не скрытой частью этого критерия.

### Приоритет 2 — полноценный Cleric spell flow первого уровня

**Цель.** После завершения классовых выборов Cleric должен получить полный сценарий выбора и подготовки заклинаний первого уровня. Под «полноценным flow» здесь понимается character creation и spell loadout, а не исполнение эффектов заклинаний в игровом движке.

#### 2.1. Player Core Domain Catalog + Cleric Domain Choice

**Статус:** завершено 14 июля 2026 года; см. [Priority 2 Implementation Plan](cleric_spell_flow_priority_2_implementation.md).

- Cloistered Cleric выбирает один primary domain выбранной Deity;
- Warpriest и non-Cleric не могут передать этот выбор без отдельного источника правила;
- смена Doctrine или Deity очищает несовместимый domain;
- focus spell возвращается typed reference до готовности focus subsystem.

#### 2.2. Cleric Spell Catalog v1

**Статус:** завершено 14 июля 2026 года; нормативный baseline описан в [cleric_spells_first_level.md](../20_domain/character_creation/cleric_spells_first_level.md).

- каталог покрывает divine cantrips и spells, необходимые персонажу первого уровня;
- deity-granted и domain spell references разрешаются тем же catalog identity;
- API возвращает tradition, rank, traits и доступность без хранения копий spell rules в персонаже;
- полный каталог всех книг и ranks не включается неявно.

#### 2.3. Prepared Spells + Divine Font Loadout

**Статус:** завершено 14 июля 2026 года; выбор сохраняется как две группы IDs, а Divine Font slots вычисляются сервером.

- сервер вычисляет доступные prepared slots и отдельные Divine Font slots по правилам первого уровня;
- пользователь выбирает cantrips и prepared spells только из доступного Cleric списка;
- Heal/Harm для Font определяется сохранённым Deity choice;
- choices сохраняются, валидируются и показываются в wizard, review и details;
- клиент не передаёт количество slots или производные ограничения.

#### 2.4. Domain Focus Spell + Focus Pool

**Статус:** завершено 14 июля 2026 года; focus package вычисляется из выбранного Domain без дополнительного persistence.

- выбранный Domain выдаёт соответствующий focus spell;
- стартовый focus pool моделируется отдельно от prepared и Font slots;
- read-модель объясняет источник spell и pool;
- casting, damage, healing, conditions и расходование ресурсов во время игры остаются вне character creation.

Приоритет считается завершённым, когда новый Cleric первого уровня имеет валидные Domain, cantrips, prepared spells, Divine Font loadout и domain focus spell, а весь набор воспроизводится после чтения из БД.

**Статус приоритета 2:** завершён 14 июля 2026 года; выполнен [итоговый cross-review](priority_2_final_review.md). Все четыре slice реализованы последовательно и прошли общий backend/frontend/EF quality gate.

### Приоритет 3 — полезная карточка персонажа

#### 3.1. Saves и Perception

**Статус:** завершено 20 июля 2026 года; см. [Implementation Plan](saves_perception_implementation.md).

**Проблема.** Ability modifiers и proficiency ranks уже существуют, но пользователь не видит итоговые Fortitude, Reflex, Will и Perception.

**Ожидаемый результат.** Сервер вычисляет значения первого уровня и возвращает объяснимый breakdown; frontend показывает их в карточке.

**Критерии готовности:**

- Fortitude использует Constitution, Reflex — Dexterity, Will и Perception — Wisdom;
- proficiency bonus вычисляется из level `1` и effective rank;
- значения не сохраняются и не принимаются от клиента;
- item, status и circumstance bonuses остаются отдельными слоями.

#### 3.2. Skill и Lore Modifiers

**Статус:** завершено 20 июля 2026 года; см. [Implementation Plan](skill_lore_modifiers_implementation.md).

**Проблема.** Training виден как список, но карточка не показывает пригодные для игры числовые modifiers.

**Ожидаемый результат.** Каждый general skill и Lore entry получает вычисляемый modifier и breakdown с учётом ability и effective proficiency.

**Критерии готовности:**

- general skills используют key ability из каталога, Lore — Intelligence;
- trained/untrained состояние и level `1` учитываются сервером;
- все источники training из Class Skills Foundation дают одинаковый результат;
- frontend показывает modifier и rank без самостоятельного пересчёта.

AC, attacks, damage, current/temporary HP и equipment bonuses не входят в этот приоритет.

**Статус приоритета 3:** завершён 20 июля 2026 года; выполнен [итоговый cross-review](priority_3_final_review.md). Карточка возвращает и отображает server-derived Saves, Perception, modifiers всех general skills и сохранённых Lore entries первого уровня.

## Следующие приоритеты

Пункты ниже задают продуктовый порядок после завершения приоритетов 1–3. Они ещё не являются карточками Vikunja: перед декомпозицией нужно получить актуальный список задач проекта `Pathfinder`, исключить дубли и отдельно зафиксировать каждый vertical slice.

### Приоритет 4 — spell flow остальных классов первого уровня

**Статус:** завершён 20 июля 2026 года; выполнен [итоговый cross-review](priority_4_final_review.md). Общий Player Core spell catalog и spell flows Bard, Druid, Witch и Wizard реализованы с persistence, API, wizard/review/details и tests.

**Цель.** Довести создание всех spellcasting-классов текущего Player Core baseline до того же уровня целостности, который уже достигнут для Cleric: персонаж имеет валидный стартовый spell loadout, а карточка объясняет источник каждого spell, slot и focus resource.

**Проблема.** Bard, Druid, Witch и Wizard уже требуют обязательные class choices, но связанные с ними repertoire, preparation, spellbook, curriculum, order/familiar spells и focus resources остаются descriptors. Формально персонаж создаётся, но его основная классовая механика первого уровня не собрана.

**Предлагаемые slices:**

1. Обобщить `Cleric Spell Catalog v1` в tradition-aware Player Core spell catalog без копирования правил в состояние персонажа.
2. Реализовать Bard repertoire и cantrips с Muse-granted spell.
3. Реализовать Druid prepared loadout и Order focus spell/focus pool.
4. Реализовать Witch familiar spell storage, prepared loadout и Patron-granted spells.
5. Реализовать Wizard spellbook, prepared loadout, curriculum spells и school-specific slot.

**Критерии готовности:**

- каждый spellcasting-класс первого уровня невозможно сохранить без полного валидного loadout;
- ограничения tradition, rank, duplicates, granted spells и slot counts вычисляются сервером;
- смена Class, Muse, Order, Patron или School очищает только ставшие несовместимыми choices;
- persistence, API, wizard, review/details и round-trip tests покрывают каждый class flow;
- casting, расходование slots, rest lifecycle и исполнение spell effects остаются отдельной runtime-подсистемой.

### Приоритет 5 — feat subsystem первого уровня

**Статус:** завершён 20 июля 2026 года; выполнен [итоговый cross-review](priority_5_final_review.md). Все четыре slice реализованы последовательно: Player Core catalog, общий ancestry/background inventory, обязательные class/skill feat choices и поддерживаемые training effects.

**Цель.** Представить ancestry, background skill и class feats единым типизированным набором выбранных и granted feats, который можно валидировать, сохранять и показывать в карточке.

**Проблема.** Ancestry feat уже выбирается, Background содержит декларативный skill feat, а часть классов и class choices требует class feat. Эти источники не образуют общего feat inventory; поэтому карточка не показывает полный набор feats, а prerequisites и повторяющиеся grants нельзя проверять единообразно.

**Предлагаемые slices:**

1. Нормализовать Player Core feat catalog v1 со stable ids, category, level, traits, prerequisites и typed effect dependencies.
2. Подключить автоматически granted Background skill feats и существующие Ancestry feat choices к общей read-модели.
3. Добавить обязательные class feat choices первого уровня там, где их требует class package или class choice.
4. Реализовывать непосредственно применимые skill/proficiency grants отдельными resolvers; spell, action, combat и inventory effects оставлять typed dependencies до готовности соответствующей подсистемы.

**Критерии готовности:**

- сервер различает выбранные и granted feats и сохраняет provenance каждого источника;
- недоступный по category, level или prerequisite feat нельзя выбрать;
- смена ancestry/background/class очищает зависимые feat choices без потери независимых grants;
- wizard и карточка показывают название, источник, category и явно отложенные effects;
- задача не вводит частичный combat engine ради отдельных feat descriptions.

Все критерии выполнены в пределах явно зафиксированного baseline. Feats с дополнительными parameter choices, временным training, weapon/combat/spell/inventory mechanics и higher-level progression остаются типизированными deferred dependencies, а не частично исполненными правилами.

### Приоритет 6 — языки и финализация создания персонажа

**Статус:** завершён 20 июля 2026 года; выполнен [итоговый cross-review](priority_6_final_review.md). Все четыре slice реализованы отдельными коммитами и прошли общий backend/frontend/EF quality gate.

**Цель.** Закрыть оставшиеся универсальные choices первого уровня и ввести явную границу между редактируемым draft и завершённым персонажем.

**Исходная проблема.** Ancestry languages и дополнительные языки от положительного Intelligence modifier не выбирались. Сохранённый персонаж не имел явного состояния завершённости, поэтому неполный draft нельзя было отличить от валидного результата character creation.

**Выполненные slices:**

1. Language catalog и ancestry language rules — завершено; см. [нормативный каталог](../20_domain/character_creation/language_catalog.md).
2. Выбор дополнительных языков с server-computed count/pool — завершено; см. [implementation note](additional_languages_implementation.md).
3. Server-side completion validation обязательных пакетов, class choices, spells, feats, training и languages — завершено; см. [implementation note](character_completion_validation_implementation.md).
4. Явная команда финализации и read-модель статуса — завершено; см. [implementation note](character_finalization_implementation.md). Respec зафиксирован как отдельная будущая граница.

**Критерии готовности:**

- число дополнительных языков выводится из итогового Intelligence modifier и правил ancestry;
- duplicate и недоступные language choices отклоняются сервером;
- финализировать можно только полностью валидного персонажа;
- список и карточка явно различают draft и завершённого персонажа;
- frontend не определяет полноту персонажа собственной независимой формулой.

Все критерии выполнены. Источники server-side access к uncommon языкам и respec завершённого персонажа не маскируются частичной реализацией и остаются отдельными задачами.

### Приоритет 7 — starting equipment и inventory boundary

**Цель.** Дать завершённому персонажу валидный стартовый набор снаряжения и подготовить данные, от которых зависят AC, attacks, damage и bulk.

**Проблема.** В character creation нет equipment catalog, starting wealth, покупок или class kits. Без выбранных и экипированных armor, weapons и gear нельзя корректно вычислить боевые показатели. При этом существующий Store не должен неявно становиться владельцем правил персонажа.

**Предлагаемые slices:**

1. Зафиксировать ownership boundary между CharacterManagement catalog/loadout и будущим Store/Inventory — завершено; см. [архитектурное решение](../20_domain/character_creation/equipment_inventory_boundary.md).
2. Нормализовать минимальный Player Core equipment catalog v1 и starting wealth/class kit rules — завершено; см. [нормативный каталог](../20_domain/character_creation/equipment_starting_wealth_catalog.md).
3. Реализовать выбор стартового набора, валидацию стоимости и сохранение immutable item references с character-owned state — завершено.
4. Добавить equipped state, weapon/armor proficiency matching и bulk foundation — завершено.

**Критерии готовности:**

- персонаж получает только допустимое и оплаченное стартовое снаряжение;
- итоговая стоимость, proficiency applicability и bulk вычисляются сервером;
- catalog definitions не копируются целиком в character state;
- смена class package до финализации корректно инвалидирует несовместимый kit;
- Store остаётся отключённым, пока отдельное решение не передаст ему ownership runtime inventory.

**Статус приоритета 7:** завершён 21 июля 2026 года; выполнен [итоговый cross-review](priority_7_final_review.md). Starting equipment участвует в create/completion/read flow, а combat calculations и runtime inventory остаются следующими независимыми границами.

## Ближайшая задача перед приоритетом 8

### Единый русскоязычный доменный глоссарий

**Статус:** ближайшая незавершённая задача; локальная постановка подготовлена в [domain_glossary_task.md](domain_glossary_task.md). Карточка Vikunja ожидает синхронизации после восстановления MCP.

**Цель.** До добавления новых архитектурных контрактов выбрать однозначные русские термины для персонажа, кампании, партии, предметов, инвентаря, торговли и игрового процесса.

**Проблема.** В документации смешиваются `Bulk`, нагрузка, item definition, шаблон, variant, instance, inventory, loadout, effect, party, campaign, GM и другие понятия. Без нормативного словаря одинаковые сущности могут получить разные названия в документации, API, C# и пользовательском интерфейсе.

**Критерии готовности:**

- создан нормативный `MemoryBank/20_domain/glossary.md`;
- для каждого понятия определены канонический русский термин, технический эквивалент, определение и модуль-владелец;
- отдельно различены вид предмета, ревизия, конфигурация и физический экземпляр;
- выбран русский термин для `Bulk` и описано его внутреннее представление;
- различены подарок, взаимный обмен, покупка и продажа;
- роли ведущего и игрока определены внутри кампании;
- документация приведена к утверждённой терминологии;
- переименования существующего кода и API перечислены отдельно и не выполняются скрыто в этой задаче.

### Приоритет 8 — боевая карточка персонажа v1

**Цель.** Превратить завершённого персонажа первого уровня в пригодную для базовой игры карточку с серверно вычисляемыми защитой, атаками, уроном, DC и состоянием HP, не связывая расчёты с временным способом хранения стартового снаряжения.

**Проблема.** Карточка уже показывает maximum HP, Saves, Perception и skills, но не содержит AC, strike modifiers/damage, class/spell DC, current/temporary HP и equipment contributions. Прямое использование текущего `EquipmentRepository` в новых расчётах закрепит переходный каталог `CharacterManagement` как постоянную границу и усложнит последующий переход на экземпляры инвентаря.

**Предлагаемые slices:**

1. Граница чтения разрешённой экипировки: прикладной порт `CharacterManagement`, собственные типы результата и текущий адаптер поверх starting equipment без подключения нового Store/Inventory.
2. AC breakdown с armor, Dexterity cap и proficiency.
3. Strike attack/damage breakdown для equipped weapons и unarmed attacks с основой для будущих постоянных улучшений.
4. Class DC, spell attack и spell DC там, где они определены class/spellcasting package.
5. Current/temporary HP state через серверные команды с инвариантами относительно derived maximum HP.

**Критерии готовности:**

- итоговые значения и breakdown вычисляются сервером из ability, proficiency, feat и разрешённого equipment state;
- расчётные сервисы не зависят напрямую от `EquipmentRepository`, persistence-модели или будущих типов `ItemCatalog`/`Inventory`;
- текущий адаптер полностью поддерживает сохранённое starting equipment и не требует торгового модуля;
- item/status/circumstance bonuses представлены отдельными типизированными слоями;
- внутреннее полное описание предмета не сериализуется клиенту напрямую;
- проверка доступа к карточке отделена от расчётов, чтобы будущий ведущий мог получить доступ в рамках кампании без изменения доменных формул;
- current HP не может выходить за допустимые границы при изменении maximum HP;
- изменение current/temporary HP выполняется командами, а не прямой записью клиентского состояния;
- frontend только отображает значения и источники, не дублируя формулы;
- encounter actions, conditions, скрытые свойства и spell/feat effect execution не включаются в card v1.

## Следующий архитектурный горизонт

### Приоритет 9 — кампании и партии

**Цель.** Создать область игрового мира, в которой ведущий и игроки получают контекстные роли, а персонажи объединяются в партию.

**Проблема.** `TeamsController` является пустой legacy-заглушкой, глобальные роли `Secure` не выражают роль пользователя в конкретной игре, а будущие предметы и магазины пока не имеют границы изоляции между играми.

**Предлагаемые slices:**

1. Campaign aggregate, жизненный цикл и роль создателя-ведущего.
2. Членство, приглашение, выход и роли ведущего/игрока внутри кампании.
3. Первая партия кампании и назначение контролируемого персонажа игроку.
4. Серверная политика доступа к карточке и действиям персонажа.
5. Пустой корневой контейнер общего хранилища партии как контракт для будущего инвентаря.

**Критерии готовности:**

- один пользователь может быть ведущим в одной кампании и игроком в другой;
- обычный игрок управляет только назначенными ему персонажами;
- ведущий видит карточки персонажей своей кампании;
- операции другой кампании недоступны даже при знании идентификатора;
- глобальная системная роль администратора не подменяет предметное членство;
- модель допускает несколько партий в будущем, хотя первый срез поддерживает одну активную партию.

### Приоритет 10 — версионируемый каталог предметов v2

**Цель.** Создать целевой источник описаний предметов, пригодный для стандартных, составных и авторских предметов.

**Проблема.** Минимальный статический каталог `CharacterManagement` покрывает только starting equipment, не имеет неизменяемых ревизий и не поддерживает сочетание оружия, защиты, расходования, зарядов, прочности и скрытых эффектов.

**Предлагаемые slices:**

1. Устойчивый вид предмета и неизменяемая ревизия.
2. Основная категория для навигации и типизированные компоненты правил для поведения.
3. Глобальные описания и описания в области конкретной кампании.
4. Неизменяемая конфигурация из ревизии, размера, материала и постоянных улучшений.
5. Публикация, вывод из обращения и базовая административная проверка.
6. Реализация адаптера разрешённой экипировки из Priority 8 поверх нового каталога без переключения владения.

**Критерии готовности:**

- опубликованная ревизия не изменяется на месте;
- щит выражается сочетанием защиты щитом, атаки, экипировки и прочности, а не наследованием от брони;
- базовый кубик урона принадлежит ревизии, а размер, материал и постоянные улучшения — конфигурации;
- системный администратор управляет глобальными описаниями;
- ведущий может создавать описания только внутри собственной кампании;
- произвольный исполняемый код и непроверяемые формулы из административного ввода запрещены.

### Приоритет 11 — экземпляры и runtime-инвентарь

**Цель.** Перейти от ссылок на вид предмета к физическим экземплярам с владельцем, расположением и изменяемым состоянием.

**Проблема.** Два одинаковых предмета сейчас неразличимы, нельзя сохранить уникальное имя, историю, состояние или местоположение, а completed-character equipment остаётся character-owned JSON state.

**Предлагаемые slices:**

1. Item instance, обязательная область кампании и ссылка на точную конфигурацию.
2. Личные, партийные, магазинные и мировые корневые контейнеры.
3. Стопки, разделение и правила объединения.
4. Текущее расположение и неизменяемый журнал перемещений.
5. Версия экземпляра, идентификатор операции и защита от повторных изменений.
6. Миграция starting equipment завершённых персонажей в экземпляры с сохранением экипировки.
7. Переключение адаптера боевой карточки на новый источник.

**Критерии готовности:**

- экземпляр находится ровно в одном месте;
- обычная команда не переносит экземпляр между кампаниями;
- полная catalog definition не копируется в экземпляр;
- повтор одной операции не создаёт второй предмет и не уменьшает количество дважды;
- стартовое снаряжение существующих персонажей мигрировано без потери выбора и экипировки;
- character creation остаётся работоспособным без доступности `Commerce`.

### Приоритет 12 — обмен и общее хранилище партии

**Цель.** Позволить игрокам одной партии свободно передавать предметы без магазина и торговой комиссии.

**Проблема.** После появления экземпляров нет безопасной операции передачи между персонажами, общего владения партии и защиты от одновременного обмена или навязывания предмета.

**Предлагаемые slices:**

1. Подарок с подтверждением получателя.
2. Двустороннее предложение обмена с резервированием обеих сторон.
3. Атомарное подтверждение или полная отмена обмена.
4. Передача в общее хранилище и получение по политике партии.
5. Аудит действий игрока и отдельная принудительная команда ведущего.

**Критерии готовности:**

- оба участника состоят в одной активной партии;
- игрок может передать только предмет контролируемого персонажа;
- экипированный, зарезервированный, израсходованный или запрещённый к передаче предмет отклоняется;
- взаимный обмен выполняется целиком или не выполняется вовсе;
- изменение предмета делает старое предложение недействительным;
- общий контейнер принадлежит партии, а не конкретному персонажу;
- цена, магазин и торговая комиссия не участвуют.

### Приоритет 13 — магазины и ручная торговля

**Цель.** Дать кампании поселения и магазины с конечным ручным ассортиментом, покупкой и продажей.

**Проблема.** Нет владельца торговых предложений, склада, цены сделки и денег; подключение legacy Store вернёт старую модель и смешает товар с игровыми правилами предмета.

**Предлагаемые slices:**

1. Поселение и магазин в области кампании.
2. Склад конкретных экземпляров и каталожные предложения стандартных товаров.
3. Кошелёк и неизменяемый журнал денежных операций.
4. Резервирование предложения и денежных средств.
5. Покупка и продажа с передачей экземпляра через `Inventory`.
6. Политика цены и выкупа без автоматической генерации ассортимента.

**Критерии готовности:**

- уникальный экземпляр нельзя одновременно продать двум покупателям;
- повтор запроса не дублирует предмет или деньги;
- цена завершённой сделки сохраняется независимо от последующих изменений политики;
- магазин не вычисляет AC, атаку, урон или другие правила персонажа;
- продать экипированный, зарезервированный или ограниченный предмет нельзя;
- магазины и их склад изолированы кампанией.

### Приоритет 14 — инструменты ведущего и скрытые свойства

**Цель.** Позволить ведущему создавать авторские и уникальные предметы своей кампании и управлять скрытой информацией с аудитом.

**Проблема.** Общий каталог не покрывает авторский контент, а единое представление предмета либо раскрывает проклятие игроку, либо не позволяет серверу применить скрытый эффект.

**Предлагаемые slices:**

1. Черновик и публикация кампанийного описания из разрешённых компонентов.
2. Создание уникальной конфигурации и экземпляра.
3. Полное серверное и видимое наблюдателю представления предмета.
4. Знание персонажа и партии, идентификация и раскрытие свойства.
5. Скрытый эффект и запрет операции, например снятия или передачи проклятого предмета.
6. Принудительная выдача, перемещение и исправление ведущим с обязательной причиной.

**Критерии готовности:**

- авторский предмет одной кампании недоступен другой;
- игрок не получает скрытые свойства через API или breakdown;
- сервер применяет скрытое ограничение до его раскрытия;
- разные персонажи могут знать о предмете разное;
- ведущий видит полное описание только своей кампании;
- каждое принудительное действие ведущего отличимо от действия игрока и записано в аудит.

### Приоритет 15 — генерация ассортимента

**Цель.** Автоматически и воспроизводимо пополнять магазины с учётом кампании, поселения и специализации.

**Проблема.** Ручной склад не масштабируется, но непрозрачная случайная генерация не позволяет объяснить или повторить ассортимент.

**Предлагаемые slices:**

1. Версионируемое правило пополнения.
2. Ограничения по уровню, редкости, доступу, категориям и бюджету.
3. Веса расходуемых, постоянных и уникальных предметов.
4. Запуск пополнения с сохранённым seed и версией правила.
5. Предварительный просмотр и подтверждение ведущим.

**Критерии готовности:**

- один и тот же seed и версия правила воспроизводят результат;
- генератор не создаёт недоступные по политике предметы;
- уникальный экземпляр создаётся и учитывается ровно один раз;
- ведущий может просмотреть и отклонить результат до публикации;
- предыдущий ассортимент и завершённые сделки не меняются при обновлении правила.

### Приоритет 16 — расширенный жизненный цикл предметов

**Цель.** Добавить изменяемые игровые состояния предметов и команды, которыми сможет пользоваться будущий игровой процесс.

**Проблема.** Экземпляр пока нельзя расходовать, разрядить, сломать, починить или улучшить переносимой руной; прямое изменение полей из боевого кода нарушит границы владения состоянием.

**Предлагаемые slices:**

1. Заряды, расходование и восстановление.
2. Расходуемые предметы, боеприпасы и уменьшение стопки.
3. Твёрдость, текущая прочность, порог поломки, уничтожение и ремонт.
4. Прикрепляемые руны и перенос улучшений между совместимыми экземплярами.
5. Серверные команды изменения с expected version и operation id.
6. Интеграционные порты для будущего `Gameplay`/`Encounter` без реализации полного encounter engine.

**Критерии готовности:**

- заряды и прочность не выходят за допустимые границы;
- конкурентное расходование последнего заряда успешно только один раз;
- расходование корректно уменьшает стопку или завершает жизненный цикл уникального экземпляра;
- сломанный или уничтоженный предмет влияет на разрешённое описание согласно правилам;
- несовместимую руну нельзя прикрепить или перенести;
- `CharacterManagement` и клиент не переписывают состояние предмета напрямую.

## Сквозной quality gate

`Character Creation E2E Golden Path` сохраняется и расширяется по мере появления новых возможностей.

- тест поднимает browser, API и PostgreSQL в изолированной конфигурации;
- базовый сценарий проходит вход, wizard, сохранение, список и details;
- Priority 8 проверяет persisted choices и серверно вычисляемую боевую карточку;
- Priority 9 добавляет кампанию, приглашение, партию и доступ ведущего к карточке;
- Priority 11–12 добавляют экземпляры, миграцию, подарок, обмен и общее хранилище;
- Priority 13 добавляет репрезентативную покупку и продажу;
- Priority 14 проверяет, что скрытое свойство применяется сервером и не утекает игроку;
- unit, integration и frontend tests остаются обязательными для каждого ограниченного среза.

## После приоритетов 8–16

Крупными направлениями остаются:

- runtime spellcasting, расходование ресурсов, rest lifecycle и исполнение spell effects;
- дальнейшее исполнение ancestry/heritage/feat effects, зависящих от actions, conditions и resistances;
- правила respec и изменения завершённого персонажа;
- encounters и остальные runtime combat mechanics;
- progression/level-up как отдельный доменный модуль;
- ремесло, добыча и награды;
- регулируемый перенос персонажей и предметов между кампаниями;
- торговля между партиями и более сложная экономика.

Store/Inventory boundary больше не является открытым вопросом верхнего уровня: целевая архитектура зафиксирована в [TOGAF-lite architecture definition](../20_domain/store/target_architecture_togaf.md). Перед каждой реализацией остаются обязательными открытые решения соответствующего этапа.

## Правила выполнения roadmap

- Каждая задача реализуется ограниченным vertical slice: domain, persistence при необходимости, API, frontend, tests и документация.
- Перед реализацией данные и неоднозначные правила нормализуются; клиент не передаёт вычислимое состояние.
- Каталоги используют stable ids и неизменяемые ревизии, а отсутствующая подсистема обозначается typed dependency/descriptor, не строковой заглушкой.
- Роли ведущего и игрока разрешаются внутри кампании, а не глобальным клиентским флагом.
- Runtime-экземпляры, магазины и мировые контейнеры получают явную область кампании.
- Административная команда ведущего отличается от обычной команды игрока и требует аудита.
- После каждого этапа оставшийся scope пересматривается; расширение оформляется отдельной задачей.
- После реализации проводится отдельный code review и повторяются затронутые проверки.
- Legacy-проблемы `Store.*` и `TeamsController` не смешиваются с новой реализацией и не определяют целевую модель.

## Связанные документы

- [Единый русскоязычный доменный глоссарий — постановка](domain_glossary_task.md)
- [Целевая архитектура кампаний, предметов, инвентаря и торговли](../20_domain/store/target_architecture_togaf.md)
- [Действующая граница стартового снаряжения](../20_domain/character_creation/equipment_inventory_boundary.md)
- [Backend MVP](mvp_character_creation_backend.md)
- [Frontend MVP](mvp_character_creation_frontend.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
- [Known gaps](../20_domain/character_creation/known_gaps.md)
- [Class catalog](../20_domain/character_creation/class_catalog_player_core.md)
