# Character Creation Near-Term Roadmap

## Цель

Зафиксировать актуальную последовательность развития character creation после завершения базового pipeline характеристик, `Rogue` и стартового `Cleric` flow. Roadmap отделяет уже реализованный фундамент от следующих продуктовых целей и не смешивает независимые подсистемы skills, feats, spells, equipment и progression.

Vikunja остаётся источником истины по карточкам и их статусам. Перед созданием задач из этого roadmap необходимо получить список задач проекта `Pathfinder`, исключить дубли и после записи перепроверить UTF-8. Пока синхронизация с Vikunja не выполнена, новые пункты ниже считаются подготовленными task slices, а не подтверждёнными карточками трекера.

## Актуальное состояние

Создание персонажа уже включает:

- имя, концепцию и возраст;
- `Ancestry`, heritage, ancestry feat и ancestry boosts/flaws;
- `Background`, два boosts и фактический skill/Lore training;
- `Class`, key ability boost и четыре финальных свободных boosts;
- типизированные стартовые proficiencies;
- обязательные `Rogue's Racket`, `Cleric Doctrine` и `Cleric Deity`;
- divine skill replacement, favored weapon proficiency, Divine Font и sanctification;
- сохранение через EF, API/read-модели, список, карточку и удаление во frontend;
- вычисляемые ability modifiers и maximum HP первого уровня.

Стартовый pipeline характеристик завершён. Deity domains и granted spells пока возвращаются только как typed descriptors. Background skill feat, общие class skills, остальные обязательные class choices, spells, equipment и большинство derived statistics ещё не применяются.

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

**Проблема.** Обязательный Order пока существует только как декларативная зависимость.

**Ожидаемый результат.** Druid выбирает Order; его trained skill применяется через общий resolver, а class feat и order spell возвращаются typed descriptors.

**Критерии готовности:**

- Druid требует Order, остальные классы запрещают его;
- skill grant и replacement используют Class Skills Foundation v1;
- выбор сохраняется и отображается во всём create/read flow;
- animal companion, feat и focus-spell mechanics не реализуются преждевременно.

#### 1.4. Bard Muse

**Проблема.** Bard не требует Muse, хотя этот выбор на первом уровне определяет granted class feat и дополнительный spell repertoire entry.

**Ожидаемый результат.** Bard выбирает Muse из типизированного каталога; выбор сохраняется, а feat и spell references возвращаются как проверенные descriptors без исполнения отсутствующих подсистем.

**Критерии готовности:**

- Bard требует Muse, остальные классы запрещают его;
- catalog использует stable ids для Muse, granted feat и spell reference;
- persistence, API, wizard, review/details и tests покрывают выбор и смену класса;
- feat application и spell repertoire остаются в следующих приоритетах.

#### 1.5. Witch Patron

**Проблема.** Witch сохраняется без Patron, поэтому не определены spell tradition, granted skill, lesson и familiar ability.

**Ожидаемый результат.** Patron обязателен для Witch; tradition и skill grant применяются типизированно, а lesson, familiar ability и spells остаются проверенными descriptors.

**Критерии готовности:**

- Witch требует Patron, остальные классы запрещают его;
- tradition и trained skill вычисляются из catalog entry, а не принимаются отдельно от клиента;
- skill conflict использует общий resolver;
- persistence, API, wizard, review/details и tests покрывают полный выбор;
- familiar и spell execution остаются отдельными подсистемами.

#### 1.6. Wizard Arcane School

**Проблема.** Wizard не требует Arcane School, поэтому отсутствует обязательный источник curriculum spells и school-specific benefits.

**Ожидаемый результат.** Wizard выбирает School по stable id; curriculum и granted benefits доступны typed descriptors, но spellbook и дополнительные slots ещё не применяются.

**Критерии готовности:**

- Wizard требует School, остальные классы запрещают его;
- curriculum references нормализованы без разбора текста;
- выбор сохраняется и отображается в create/read flow;
- фактическое наполнение spellbook и slots входит в будущий Wizard spell flow, а не в задачу выбора School.

#### 1.7. Wizard Arcane Thesis

**Проблема.** Обязательная Arcane Thesis отсутствует, а её разнородные эффекты нельзя смешивать с School curriculum в одной задаче.

**Ожидаемый результат.** Wizard независимо выбирает Thesis; supported effects типизированы, остальные имеют явные deferred dependencies.

**Критерии готовности:**

- Wizard требует Thesis одновременно со School;
- Thesis и School можно валидировать и заменять независимо;
- persistence, API, wizard, review/details и tests покрывают выбор;
- spell/familiar/item mechanics не имитируются частично.

Приоритет считается завершённым, когда новый персонаж каждого из восьми классов имеет все обязательные классовые выборы первого уровня. Feats и spell loadout остаются явно незавершёнными областями, а не скрытой частью этого критерия.

### Приоритет 2 — полноценный Cleric spell flow первого уровня

**Цель.** После завершения классовых выборов Cleric должен получить полный сценарий выбора и подготовки заклинаний первого уровня. Под «полноценным flow» здесь понимается character creation и spell loadout, а не исполнение эффектов заклинаний в игровом движке.

#### 2.1. Player Core Domain Catalog + Cleric Domain Choice

- Cloistered Cleric выбирает один primary domain выбранной Deity;
- Warpriest и non-Cleric не могут передать этот выбор без отдельного источника правила;
- смена Doctrine или Deity очищает несовместимый domain;
- focus spell возвращается typed reference до готовности focus subsystem.

#### 2.2. Cleric Spell Catalog v1

- каталог покрывает divine cantrips и spells, необходимые персонажу первого уровня;
- deity-granted и domain spell references разрешаются тем же catalog identity;
- API возвращает tradition, rank, traits и доступность без хранения копий spell rules в персонаже;
- полный каталог всех книг и ranks не включается неявно.

#### 2.3. Prepared Spells + Divine Font Loadout

- сервер вычисляет доступные prepared slots и отдельные Divine Font slots по правилам первого уровня;
- пользователь выбирает cantrips и prepared spells только из доступного Cleric списка;
- Heal/Harm для Font определяется сохранённым Deity choice;
- choices сохраняются, валидируются и показываются в wizard, review и details;
- клиент не передаёт количество slots или производные ограничения.

#### 2.4. Domain Focus Spell + Focus Pool

- выбранный Domain выдаёт соответствующий focus spell;
- стартовый focus pool моделируется отдельно от prepared и Font slots;
- read-модель объясняет источник spell и pool;
- casting, damage, healing, conditions и расходование ресурсов во время игры остаются вне character creation.

Приоритет считается завершённым, когда новый Cleric первого уровня имеет валидные Domain, cantrips, prepared spells, Divine Font loadout и domain focus spell, а весь набор воспроизводится после чтения из БД.

### Приоритет 3 — полезная карточка персонажа

#### 3.1. Saves и Perception

**Проблема.** Ability modifiers и proficiency ranks уже существуют, но пользователь не видит итоговые Fortitude, Reflex, Will и Perception.

**Ожидаемый результат.** Сервер вычисляет значения первого уровня и возвращает объяснимый breakdown; frontend показывает их в карточке.

**Критерии готовности:**

- Fortitude использует Constitution, Reflex — Dexterity, Will и Perception — Wisdom;
- proficiency bonus вычисляется из level `1` и effective rank;
- значения не сохраняются и не принимаются от клиента;
- item, status и circumstance bonuses остаются отдельными слоями.

#### 3.2. Skill и Lore Modifiers

**Проблема.** Training виден как список, но карточка не показывает пригодные для игры числовые modifiers.

**Ожидаемый результат.** Каждый general skill и Lore entry получает вычисляемый modifier и breakdown с учётом ability и effective proficiency.

**Критерии готовности:**

- general skills используют key ability из каталога, Lore — Intelligence;
- trained/untrained состояние и level `1` учитываются сервером;
- все источники training из Class Skills Foundation дают одинаковый результат;
- frontend показывает modifier и rank без самостоятельного пересчёта.

AC, attacks, damage, current/temporary HP и equipment bonuses не входят в этот приоритет.

## Сквозной quality gate

`Character Creation E2E Golden Path` не конкурирует с продуктовыми приоритетами и добавляется как проверка выполняемых vertical slices.

- тест поднимает browser, API и PostgreSQL в изолированной конфигурации;
- проходит вход, wizard, сохранение, список и details;
- проверяет persisted choices и вычисляемые значения, а не только навигацию;
- по мере завершения приоритетов получает по одному репрезентативному сценарию для class choices, Cleric spells и derived card;
- unit и integration tests остаются обязательными.

## После ближайшего горизонта

Следующими крупными направлениями остаются:

- skill feat и class feat catalogs;
- languages и дополнительные языки от Intelligence;
- исполняемые ancestry/heritage/ancestry feat effects;
- equipment, starting wealth и inventory integration;
- AC, attacks, damage, current/temporary HP и остальные combat statistics;
- явная финализация draft и правила изменения завершённого персонажа;
- progression/level-up как отдельный доменный модуль.

Store не включается в character creation roadmap до отдельного решения об equipment/inventory boundary.

## Правила выполнения roadmap

- Каждая задача реализуется ограниченным vertical slice: domain, persistence при необходимости, API, frontend, tests и документация.
- Перед реализацией данные и неоднозначные правила нормализуются; клиент не передаёт вычислимое состояние.
- Каталоги используют stable ids, а отсутствующая подсистема обозначается typed dependency/descriptor, не строковой заглушкой.
- После каждого этапа оставшийся scope пересматривается; расширение оформляется отдельной задачей.
- После реализации проводится отдельный code review и повторяются затронутые проверки.
- Известные legacy-проблемы `Store.Application` не смешиваются с CharacterManagement задачами.

## Связанные документы

- [Backend MVP](mvp_character_creation_backend.md)
- [Frontend MVP](mvp_character_creation_frontend.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
- [Known gaps](../20_domain/character_creation/known_gaps.md)
- [Class catalog](../20_domain/character_creation/class_catalog_player_core.md)
