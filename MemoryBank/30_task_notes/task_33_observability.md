# Task #33: локальный контур логирования и диагностики

## Проблема

Сейчас backend пишет сообщения через стандартный `Microsoft.Extensions.Logging`, но у проекта нет общего формата событий, централизованного сборщика и постоянного места просмотра. После завершения процесса консольная история теряется, а события HTTP-запроса, регистрации, MassTransit message и создания `CharacterManagement.Account` нельзя уверенно связать в одну цепочку.

## Целевой сценарий

Для локальной разработки используется следующий контур:

1. Код продолжает зависеть от `ILogger<T>` и пишет структурные события с именованными свойствами.
2. Serilog подключается как logging provider в `Pathfinder.Web`.
3. Все события выводятся в читаемую консоль для быстрого feedback loop.
4. События уровня `Information` и выше отправляются в локальный Seq.
5. Seq запускается отдельным Docker Compose service, хранит данные в named volume и предоставляет web UI.
6. Если Seq недоступен, backend продолжает работать и писать в консоль; диагностический sink не становится обязательной runtime-зависимостью приложения.

Полноценные OpenTelemetry traces, Elasticsearch и production deployment в эту задачу не входят.

## Почему Seq

- поддерживает структурные события Serilog без дополнительного конвейера преобразования;
- даёт web UI, полнотекстовый поиск и фильтры по свойствам;
- достаточно одного локального container и volume;
- соответствует масштабу учебного проекта и не требует ELK/Loki/Grafana stack.

## Стандарт событий

Каждое диагностически значимое событие должно иметь стабильный message template и только необходимые свойства.

Общие свойства:

- `Application` — имя приложения;
- `Environment` — окружение;
- `RequestId` и `TraceId` — HTTP-контекст, если он существует;
- `UserId` — внутренний идентификатор пользователя, если он известен;
- `MessageId`, `CorrelationId`, `ConversationId` — MassTransit metadata для message flow;
- `EventType` или `SourceContext` — тип события или категория logger.

Для registration flow нужны отдельные события:

- регистрация принята;
- пользователь сохранён;
- `UserRegisteredEvent` публикуется;
- событие получено consumer;
- `CharacterManagement.Account` создан;
- создание пропущено, потому что account уже существует;
- обработка завершилась ошибкой.

Не логировать пароль, JWT, connection string, полный request body, email и персональные данные без отдельной необходимости. Исключения логировать вместе с контекстными идентификаторами, но клиенту по-прежнему возвращать безопасное сообщение.

## Корреляция

На границе HTTP использовать `Activity.TraceId` как основной идентификатор пользовательской операции. При публикации MassTransit должен сохранить доступный correlation context; consumer всегда логирует `MessageId`, `CorrelationId` и `ConversationId` из `ConsumeContext`.

Если автоматического переноса HTTP trace в `CorrelationId` недостаточно, публикация должна явно установить correlation identifier. Конкретный механизм фиксируется integration-тестом, а не предположением о поведении транспорта.

## Сбор и хранение

Локальный Compose публикует единый Seq HTTP endpoint только на loopback-интерфейсе. Этот endpoint обслуживает ingestion API и web UI.

Данные Seq хранятся в named Docker volume и переживают перезапуск container. Версии image и NuGet packages фиксируются явно. Пароли и API keys не коммитятся; локальные значения задаются через environment variables или user secrets.

Для локальной среды задаётся ограниченное хранение данных. Рекомендуемая стартовая политика — 7 дней либо небольшой лимит объёма. Точное значение должно быть отражено в инструкции и проверено в выбранной версии Seq.

## Где смотреть

Документация должна содержать:

- команду запуска Seq;
- адрес web UI;
- способ проверить ingestion health;
- фильтр по `TraceId`/`CorrelationId` для всей цепочки;
- фильтр по `UserId` для account lifecycle;
- фильтр ошибок `@Level = 'Error'` с ограничением по приложению;
- способ остановить stack без удаления volume и отдельную явную команду очистки данных.

## Этапы реализации

1. Добавить Serilog bootstrap и конфигурацию console + Seq sinks.
2. Добавить локальный Compose service и безопасные настройки по умолчанию.
3. Унифицировать события registration/event/account flow.
4. Добавить exception logging в CharacterManagement API либо единый узкий middleware/filter, не дублирующий сообщения.
5. Проверить корреляцию HTTP и MassTransit integration-тестом.
6. Добавить runbook с командами и готовыми Seq-фильтрами.

Runbook: [`../10_workflow/observability.md`](../10_workflow/observability.md).

## Критерии готовности

- backend запускается и обслуживает запросы при доступном и недоступном Seq;
- события одновременно видны в консоли и Seq UI;
- после регистрации всю цепочку можно найти одним `TraceId` или `CorrelationId`;
- по событиям различимы account created и idempotent skip;
- ошибки CharacterManagement API содержат exception и контекст в Seq, но не раскрывают внутренние детали клиенту;
- перезапуск Seq не удаляет сохранённые события;
- документация позволяет новому разработчику поднять сборщик и найти registration flow без знания реализации.

## Результат проверки

Проверено 11 июля 2026:

- Seq `2025.2` запускается из `compose.observability.yml`, UI отвечает на `http://localhost:5341`;
- `Pathfinder.Web` отправляет события в Seq и продолжает работать при недоступном Seq;
- HTTP-событие содержит `Application`, `Environment`, `TraceId`, `RequestId`, method, path, status code и elapsed time;
- событие сохранилось под тем же event ID после `docker compose restart seq`;
- в Seq создана retention policy `7.00:00:00`;
- integration-тест registration flow подтверждает создание `Secure.User`, передачу `UserRegisteredEvent` и создание `CharacterManagement.Account`;
- все 30 тестов `CharacterManagement.Infrastructure.Tests` проходят.
