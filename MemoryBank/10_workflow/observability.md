# Локальные логи: Serilog и Seq

## Назначение

`Pathfinder.Web` пишет структурные события через `ILogger<T>`. Serilog выводит их в консоль и в локальный Seq. Seq хранит события между перезапусками и предоставляет web UI для поиска.

Seq является необязательной диагностической зависимостью. Если container остановлен, backend продолжает работать и писать в консоль.

## Запуск

Из корня репозитория:

```powershell
docker compose -f compose.observability.yml up -d
```

Проверить состояние:

```powershell
docker compose -f compose.observability.yml ps
Invoke-WebRequest http://localhost:5341 -UseBasicParsing
```

Web UI:

```text
http://localhost:5341
```

После запуска Seq запустить `Pathfinder.Web` в окружении `Development`. Настройки sink находятся в `Pathfinder.Web/appsettings.Development.json`. Пример конфигурации для новой среды находится в `Pathfinder.Web/appsettings.Sample.json`.

## Где искать события

Открыть Seq UI и перейти в stream событий.

Все события backend:

```text
Application = 'Pathfinder.Web'
```

Ошибки backend:

```text
Application = 'Pathfinder.Web' and @Level = 'Error'
```

Один HTTP-запрос:

```text
TraceId = '<trace id>'
```

Одна MassTransit-цепочка:

```text
CorrelationId = '<correlation id>'
```

Account lifecycle пользователя:

```text
UserId = 42
```

Registration flow:

```text
EventType = 'UserRegisteredEvent' or SourceContext like '%EnsureAccountExistsHandler%'
```

На событии публикации доступны одновременно HTTP `TraceId` и MassTransit `CorrelationId`. Скопировать `CorrelationId` из него и применить соответствующий фильтр, чтобы перейти от HTTP-части к consumer и account handler.

## Остановка и данные

Остановить container, сохранив события:

```powershell
docker compose -f compose.observability.yml down
```

Данные находятся в named volume `pathfinder-seq-data` и переживают обычный `down` и повторный `up`.

Полностью удалить локальные события:

```powershell
docker compose -f compose.observability.yml down --volumes
```

Команда с `--volumes` необратимо удаляет локальное хранилище Seq и должна выполняться только явно.

## Retention

Для локальной разработки установить в Seq retention policy на 7 дней. Политика задаётся в административном UI Seq после первого запуска. Она ограничивает рост named volume; отсутствие retention не должно считаться готовой локальной конфигурацией для длительной работы.

Проверить активную политику можно через административный UI или локальный API:

```powershell
Invoke-RestMethod http://localhost:5341/api/retentionpolicies
```

## Безопасность

- Seq опубликован только на `127.0.0.1` и запускается без аутентификации исключительно для локальной разработки.
- Не публиковать порт Seq во внешнюю сеть с текущей конфигурацией.
- Не логировать пароли, JWT, connection strings, API keys, полные request bodies, email и персональные данные без отдельного обоснования.
- Для сетевого или командного deployment включить аутентификацию и передавать Seq API key через environment variable или user secrets.

## Диагностика доставки

Если события есть в консоли, но отсутствуют в Seq:

1. Проверить, что backend запущен в окружении `Development`.
2. Проверить `docker compose -f compose.observability.yml ps`.
3. Открыть `http://localhost:5341`.
4. Проверить значение `Serilog:WriteTo:1:Args:serverUrl`.
5. Проверить отсутствие фильтра в Seq UI, скрывающего события.

После восстановления Seq новые события снова отправляются sink. Недоставленные события, созданные во время остановки Seq, не считаются гарантированно сохранёнными: локальная конфигурация не использует durable buffer.
