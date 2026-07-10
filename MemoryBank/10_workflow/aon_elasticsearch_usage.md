# AoN Elasticsearch: как использовать в следующих сессиях

## Когда читать

Читать перед исследованием данных Archives of Nethys: feats, spells, classes, ancestries, backgrounds, skills и других PF2e-сущностей.

## Зачем

При последующих поисках по Archives of Nethys лучше сразу использовать AoN Elasticsearch, а не массово обходить HTML-страницы через `Invoke-WebRequest`.

Причины:
- быстрее;
- меньше сетевых таймаутов;
- данные приходят в структурированном виде;
- удобнее фильтровать по `primary_source`, `trait`, `category`, `level`, `url`, `summary`, `prerequisite`, `requirement`, `trigger`, `frequency`.

Базовый endpoint:

```text
https://elasticsearch.aonprd.com/aon-test/_search
```

## Когда использовать

Использовать этот путь по умолчанию, если нужно:
- найти feats, spells, actions и другие сущности AoN;
- отфильтровать данные только по `Player Core` или другому источнику;
- получить краткое summary без парсинга HTML;
- собрать каталог по trait, классу, ancestry, уровню или источнику;
- быстро получить `url` конкретной AoN-страницы.

HTML-страницы AoN стоит открывать уже после этого:
- для точечной ручной верификации;
- если нужен полный rules text;
- если в индексе не хватает конкретного поля.

## Как запрашивать из PowerShell

Пример POST-запроса:

```powershell
$body = @{
  size = 20
  query = @{
    query_string = @{
      query = 'category:feat AND primary_source:"Player Core" AND trait:(Fighter) AND NOT trait:Archetype'
    }
  }
} | ConvertTo-Json -Depth 5

Invoke-WebRequest `
  -UseBasicParsing `
  -Method Post `
  -Uri 'https://elasticsearch.aonprd.com/aon-test/_search' `
  -ContentType 'application/json' `
  -Body $body |
Select-Object -ExpandProperty Content
```

## Как запрашивать из Python

```python
import json
import urllib.request

payload = {
    "size": 20,
    "query": {
        "query_string": {
            "query": 'category:feat AND primary_source:"Player Core" AND trait:(Fighter) AND NOT trait:Archetype'
        }
    }
}

request = urllib.request.Request(
    "https://elasticsearch.aonprd.com/aon-test/_search",
    data=json.dumps( payload ).encode( "utf-8" ),
    headers={
        "User-Agent": "Mozilla/5.0",
        "Content-Type": "application/json",
    },
    method="POST",
)

with urllib.request.urlopen( request, timeout=30 ) as response:
    result = json.loads( response.read().decode( "utf-8" ) )
```

## Практические query-string шаблоны

### Все class feats только из Player Core для одного класса

```text
category:feat AND primary_source:"Player Core" AND trait:(Fighter) AND NOT trait:Archetype
```

### То же, но с исключением mythic / kingdom

```text
category:feat AND primary_source:"Player Core" AND trait:(Fighter) AND NOT trait:Archetype AND NOT trait:kingdom AND NOT trait:mythic
```

### Все ancestry feats только из Player Core для конкретной ancestry

```text
category:feat AND primary_source:"Player Core" AND trait:(Dwarf)
```

### Все spells из Player Core для одной tradition / trait

```text
category:spell AND primary_source:"Player Core" AND trait:(Arcane)
```

### Конкретная сущность по имени

```text
category:feat AND primary_source:"Player Core" AND name:"Double Slice"
```

## Какие поля обычно полезны

Из `hits[]. _source` чаще всего нужны:
- `name`
- `id`
- `url`
- `level`
- `summary`
- `actions`
- `trait`
- `primary_source`
- `primary_source_raw`
- `prerequisite`
- `requirement`
- `trigger`
- `frequency`
- `category`

## Важные замечания

- В AoN Elasticsearch записи базового PF2e-слоя часто помечены как `Core Rulebook`, хотя в проектной документации и на страницах AoN это тот же baseline, который мы обычно называем `Player Core`.
- Для каталогов, связанных с текущим MVP, безопаснее считать `Core Rulebook` и `Player Core` одним источником, если рядом нет явного `Player Core 2` или другого book tag.
- Для строгой фильтрации по книге использовать именно `primary_source:"Player Core"`, а не просто `source:"Player Core"`.
- `source` может содержать вторичные источники и переиздания, из-за чего в выборку могут попадать записи не из основной книги.
- Shared feats с несколькими class traits нужно осознанно дублировать в каждом релевантном классе, если собирается каталог по классам.
- Если нужен полный rules text, после отбора записей через elastic открывать конкретные `url` на AoN.

## Рекомендуемый workflow

1. Сначала сделать выборку через AoN Elasticsearch.
2. Отфильтровать по `primary_source`, `category` и `trait`.
3. Сохранить структурированный список `name + level + url + summary`.
4. Только затем точечно открывать HTML-страницы для спорных или важных записей.
