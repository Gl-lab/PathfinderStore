# Task 40 Documentation Inventory

## Scope

Инвентаризация выполнена для всех markdown-файлов в `MemoryBank` после первичной раскладки по новой структуре.

Дата проверки: `2026-07-10`.

## Сводка

- Всего markdown-файлов: `30`.
- Основные разделы уже разнесены корректно:
  - `00_project/` — проектный обзор и состояние CharacterManagement;
  - `10_workflow/` — правила работы и инструменты;
  - `20_domain/` — нормативная доменная документация;
  - `30_task_notes/` — локальные заметки к крупным задачам;
  - `90_research/` — исследовательский архив.
- Старые корневые папки MemoryBank для character creation rules и temp research больше не используются в ссылках.
- Абсолютные пути к локальному workspace в документации не найдены.
- Явные устаревшие формулировки про отсутствующий skill/background catalog больше не найдены.
- Все markdown-файлы имеют H1.
- Относительные markdown-ссылки внутри `MemoryBank` проверены.

## Файлы без H1

Статус: исправлено.

Все living markdown-документы в `MemoryBank` имеют явный H1.

## Устаревшие или рискованные документы

### `00_project/project_pathfinder_character_domain.md`

Статус: исправлено на шаге 2 задачи `#40`.

На момент инвентаризации документ устаревал относительно текущего состояния backend MVP:

- всё ещё говорит, что `CreateCharacterHandler` пустой;
- упоминает `DeleteCharacterHandler` как `NotImplementedException`;
- упоминает `GetRacesHandler` и `RacesController`, хотя текущий контекст уже перешёл к ancestry API;
- описывает `CharacterController` как почти полностью закомментированный;
- список “что нужно для MVP” не совпадает с `project_overview.md` и `mvp_character_creation_backend.md`.

Выполненное решение: документ переписан как актуальная карта `CharacterManagement`, а общий `project_overview.md` переписан как человекочитаемый вход в проект.

### `30_task_notes/task_14_integration_test_context.md`

Статус: исправлено на шаге 5 задачи `#40`.

Текущее состояние:

- добавлен H1;
- документ явно помечен как historical note;
- ссылка добавлена в `30_task_notes/README.md`.

## Раздел `00_project`

Текущее состояние после шага 2:

- `project_overview.md` стал главным человекочитаемым обзором проекта.
- `project_pathfinder_character_domain.md` стал актуальной картой `CharacterManagement`.
- Оба файла имеют H1 и ссылки на связанные разделы.

Рекомендация для будущей полировки: после выделения `known_gaps.md` и `implementation_notes.md` проверить, не нужно ли вынести часть gaps из `project_pathfinder_character_domain.md` в доменный раздел.

## Раздел `10_workflow`

Статус: исправлено на шаге 3 задачи `#40`.

Текущее состояние после шага 3:

- все файлы в `10_workflow/` имеют H1;
- все файлы в `10_workflow/` имеют блок `Когда читать`;
- `sandbox.md`, `vikunja.md`, `ef.md` приведены к единому рабочему формату;
- старые ссылки на workflow-документ Vikunja в `AGENTS.md` заменены на новый путь внутри `10_workflow/`.

Рекомендация для будущей полировки: при появлении новых workflow-файлов использовать тот же шаблон: H1, `Когда читать`, главное правило, детали/troubleshooting.

## Раздел `20_domain/character_creation`

Статус: исправлено на шаге 4 задачи `#40`.

Текущее состояние после шага 4:

- `README.md` разделяет normative rules, catalogs, gaps и implementation notes.
- `known_gaps.md` фиксирует отсутствующие и частично готовые доменные данные.
- `implementation_notes.md` фиксирует расхождения текущего кода с remastered baseline и целевыми правилами.
- `catalog_inventory_status.md` оставлен как компактная карта готовности каталогов и ссылается на `known_gaps.md`.
- `aon_player_core_ancestries_59_64.md` больше не смешивает источник данных с расхождениями реализации.

Рекомендация для будущей полировки: при задачах по `Background`, `Class`, spells, deity, equipment или languages обновлять сначала `known_gaps.md`, затем переносить подтверждённые данные в отдельные catalog documents.

## Раздел `30_task_notes`

Текущее состояние после шага 5:

- Новый формат уже есть у `task_16`, `task_32`, `mvp_character_creation_backend`, `mvp_character_creation_frontend`.
- `task_14_integration_test_context.md` помечен как historical note и добавлен в `README.md`.

Рекомендация:

- после завершения задачи `#40` обновить `task_40_inventory.md` или заменить его итоговым статусом.

## Раздел `90_research`

Текущее состояние:

- Research archive отделён правильно.
- Файлы крупные, особенно `aon_player_core_class_feats.md`.
- Их не стоит переписывать как часть быстрой уборки: это архив данных.

Рекомендация:

- не нормализовать research-файлы насильно;
- улучшить только `90_research/README.md`, добавив правило “не нормативно, пока не promoted в `20_domain/`”.

## Проверки для финала задачи #40

- `Select-String` по старым корневым путям и абсолютным workspace-путям возвращает пустой результат. ✅
- У всех living docs есть H1. ✅
- У workflow docs есть блок `Когда читать`. ✅
- `project_pathfinder_character_domain.md` больше не противоречит текущему backend MVP. ✅
- `20_domain/character_creation/README.md` различает rules, catalogs, gaps и implementation notes. ✅
- `20_domain/character_creation/known_gaps.md` и `implementation_notes.md` созданы. ✅
- `AGENTS.md` не содержит ссылок на старый путь workflow-документа Vikunja. ✅
- Относительные markdown-ссылки внутри `MemoryBank` проверены. ✅
