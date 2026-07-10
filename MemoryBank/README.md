# MemoryBank

`MemoryBank` хранит рабочую память проекта: устойчивые решения, правила разработки, доменные правила и локальные заметки к крупным задачам.

## Как читать

Перед началом каждой сессии обязательно читать:

1. [`00_project/project_overview.md`](00_project/project_overview.md)
2. [`10_workflow/sandbox.md`](10_workflow/sandbox.md)
3. [`10_workflow/vikunja.md`](10_workflow/vikunja.md)

При разработке C# дополнительно читать:

- [`10_workflow/feedback_csharp_style.md`](10_workflow/feedback_csharp_style.md)

При работе с EF migrations дополнительно читать:

- [`10_workflow/ef.md`](10_workflow/ef.md)

При исследовании данных Archives of Nethys дополнительно читать:

- [`10_workflow/aon_elasticsearch_usage.md`](10_workflow/aon_elasticsearch_usage.md)

## Разделы

### [`00_project/`](00_project)

Человекочитаемый обзор проекта: цель, стек, архитектура, структура solution, dev setup и текущий статус модулей.

### [`10_workflow/`](10_workflow)

Правила работы в проекте: sandbox, Vikunja, EF migrations, C# style и исследовательский workflow.

### [`20_domain/`](20_domain)

Нормативные доменные правила. Этот раздел важнее исследовательских заметок и должен быть источником истины для реализации.

Сейчас основной доменный раздел:

- [`20_domain/character_creation/`](20_domain/character_creation)

### [`30_task_notes/`](30_task_notes)

Локальные статусные заметки по крупным задачам и эпикам. Это не замена Vikunja: задачник остаётся source of truth по статусу карточек, а здесь хранится инженерный контекст.

### [`90_research/`](90_research)

Исследовательский архив: сырые и полусырые материалы, выгрузки, проверочные заметки. Материал отсюда не считается нормативным, пока не перенесён или не обобщён в `20_domain/`.

## Приоритет источников

Если источники расходятся:

1. Актуальная карточка Vikunja задаёт текущий scope работы.
2. `20_domain/` задаёт доменные правила.
3. `10_workflow/` задаёт правила процесса и инструментов.
4. `30_task_notes/` объясняет контекст крупных задач.
5. `90_research/` используется как архив исследований, но не как норма.

