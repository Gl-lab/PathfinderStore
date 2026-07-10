# Правила написания кода (C#)

## Когда читать

Читать перед любой правкой C# кода в проекте.

## Форматирование

- Отступы: **пробелы, 4 пробела**
- Без финального переноса строки в конце файла
- Кодировка: `utf-8`

## Фигурные скобки

- Скобки **всегда** обязательны (`csharp_prefer_braces = true` — ошибка без них)
- Открывающая скобка **на новой строке** (`csharp_new_line_before_open_brace = all`)
- `else`, `catch`, `finally` — каждый с новой строки

```csharp
// ПРАВИЛЬНО
if ( condition )
{
    DoSomething();
}
else
{
    DoOther();
}

// НЕПРАВИЛЬНО — нет скобок
if ( condition )
    DoSomething();
```

## var — НЕ использовать

```csharp
// ПРАВИЛЬНО
int count = GetCount();
string name = GetName();
MyClass obj = new MyClass();

// НЕПРАВИЛЬНО
var count = GetCount();
var name = GetName();
```

## Именование

| Что | Стиль | Пример |
|---|---|---|
| Приватные/instance поля | `_camelCase` | `_myField` |
| Публичные поля | `_camelCase` | `_myField` |
| Статические поля | `_camelCase` | `_myField` |
| Константы | `PascalCase` | `MaxCount` |
| События | `OnPascalCase` | `OnValueChanged` |
| Методы, свойства, типы | `PascalCase` | `GetValue()` |
| Локальные переменные, параметры | `camelCase` | `myVar` |

## Модификаторы доступа

- **Всегда** указывать модификатор (ошибка без него для не-интерфейсных членов)
- Порядок: `public, private, protected, internal, static, extern, new, virtual, abstract, sealed, override, readonly, unsafe, volatile, async`
- Поля должны быть `readonly` где возможно (ошибка если можно добавить)

```csharp
// ПРАВИЛЬНО
private readonly int _value;
public static readonly string DefaultName = "default";

// НЕПРАВИЛЬНО — нет readonly, хотя можно добавить
private int _value;
```

## Expression-bodied члены

- **Конструкторы**: НИКОГДА не expression-bodied (ошибка)
- **Аксессоры get/set**: всегда expression-bodied (ошибка если не так)
- **Методы и свойства**: только если вмещаются в одну строку

```csharp
// ПРАВИЛЬНО
public int Value { get => _value; set => _value = value; }
public string Name => _name;
public int GetValue() => _value;

// НЕПРАВИЛЬНО — конструктор expression-bodied
public MyClass() => _value = 0; // ЗАПРЕЩЕНО

// ПРАВИЛЬНО — конструктор
public MyClass()
{
    _value = 0;
}
```

## Типы: предпочитать встроенные ключевые слова

```csharp
// ПРАВИЛЬНО для локальных, параметров, членов
int count;
string name;
bool isValid;

// ПРАВИЛЬНО для доступа к членам типа (не ключевое слово)
Int32.MaxValue
String.Empty

// НЕПРАВИЛЬНО
Int32 count;
int.MaxValue  // для доступа к членам
```

## Скобки в выражениях

Всегда явные скобки в арифметических, реляционных и двоичных операторах:

```csharp
// ПРАВИЛЬНО
int result = (a + b) * c;
bool check = (x > 0) && (y < 10);

// НЕПРАВИЛЬНО
int result = a + b * c;
```

## Пробелы

- Пробелы **внутри** скобок вызова методов: `Foo( arg1, arg2 )`
- Пробелы **внутри** скобок объявления параметров: `void Foo( int a, int b )`
- Пробелы **внутри** скобок для control flow, type cast, выражений: `if ( x )`, `( int )obj`
- Пробелы вокруг бинарных операторов: `a + b`, `x == y`
- Пробелы **внутри** квадратных скобок: `arr[ 0 ]`
- **Без** пробелов внутри атрибутных скобок: `[Attribute]`

```csharp
// ПРАВИЛЬНО
public void SetValue( int value )
{
    if ( value > 0 )
    {
        _items[ 0 ] = value;
    }
}

// НЕПРАВИЛЬНО
public void SetValue(int value)
{
    if(value > 0)
    {
        _items[0] = value;
    }
}
```

## Перенос строк

- Цепочки вызовов методов — **всегда** переносить каждый вызов на новую строку:

```csharp
// ПРАВИЛЬНО
var result = source
    .Where( x => x.IsActive )
    .Select( x => x.Value )
    .ToList();

// НЕПРАВИЛЬНО
var result = source.Where( x => x.IsActive ).Select( x => x.Value ).ToList();
```

- Параметры/аргументы: переносить если не влезают (`chop_if_long`), максимум 2 параметра в строке
- Property patterns: всегда переносить каждый элемент

## Pattern matching

Предпочитать pattern matching:

```csharp
// ПРАВИЛЬНО
if ( obj is MyClass myObj )
{
    myObj.DoSomething();
}

// НЕПРАВИЛЬНО
if ( obj is MyClass )
{
    MyClass myObj = (MyClass)obj;
    myObj.DoSomething();
}
```

## this. — НЕ использовать

```csharp
// ПРАВИЛЬНО
_value = value;
DoSomething();

// НЕПРАВИЛЬНО
this._value = value;
this.DoSomething();
```

## Инициализаторы объектов и коллекций

```csharp
// ПРАВИЛЬНО
MyClass obj = new MyClass
{
    Name = "test",
    Value = 42,
};

List<int> list = new List<int> { 1, 2, 3 };

// НЕПРАВИЛЬНО
MyClass obj = new MyClass();
obj.Name = "test";
obj.Value = 42;
```

## Throw expressions

```csharp
// ПРАВИЛЬНО
string name = value ?? throw new ArgumentNullException( nameof( value ) );

// НЕПРАВИЛЬНО
if ( value == null )
{
    throw new ArgumentNullException( nameof( value ) );
}
string name = value;
```

## Sealed классы

Помечать классы `sealed` если они не предназначены для наследования.

## Enum

Каждый член enum — на отдельной строке.
