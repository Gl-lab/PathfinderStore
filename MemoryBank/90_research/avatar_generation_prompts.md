# Аватары Player Core: источники и пакет промптов

## Назначение

Документ фиксирует проверенную основу для генерации тематических аватаров текущих персонажей. Archives of Nethys используется только как источник текстовых правил и описаний. Официальные иллюстрации не являются визуальными референсами и не должны копироваться.

Целевой каталог:

```text
6 ancestry × 8 classes × 2 genders × 2 variants = 192 avatars
```

Полная матрица ID и metadata: [avatar_asset_matrix.md](avatar_asset_matrix.md).

Пошаговый порядок выбора строк, формирования конкретного промта, визуальной приёмки, конвертации и подключения ассета: [avatar_assets.md](../10_workflow/avatar_assets.md).

## Проверенные источники ancestry

Все записи относятся к `Player Core`. Формулировки ниже являются кратким пересказом разделов Summary и Physical Description.

| Ancestry | AoN | Проверенные визуальные признаки |
|---|---|---|
| Dwarf | [Player Core, стр. 42](https://2e.aonprd.com/Ancestries.aspx?ID=59) | Низкий коренастый народ; широкое компактное телосложение и мощная фигура. Длинные волосы и бороды ценятся у всех полов, часто заплетены в сложные клановые узоры. Борода допустима, но не обязательна и не должна использоваться как единственный маркер пола. |
| Elf | [Player Core, стр. 46](https://2e.aonprd.com/Ancestries.aspx?ID=60) | Высокие и изящные; удлинённые черты лица, резко заострённые уши. Широкие округлые глаза с крупными яркими зрачками. Допустимы едва заметные признаки длительной адаптации к окружению. |
| Gnome | [Player Core, стр. 50](https://2e.aonprd.com/Ancestries.aspx?ID=61) | Низкие, но крепкие взрослые. Естественные цвета кожи разнообразны и чаще землистые или розоватые; волосы и глаза часто насыщенных необычных цветов. Нельзя изображать ребёнком. |
| Goblin | [Player Core, стр. 54](https://2e.aonprd.com/Ancestries.aspx?ID=62) | Приземистый гуманоид с непропорционально большой головой, крупными ушами и маленькими красными глазами. Кожа зелёная, серая или синяя; чаще лысый; неровные постоянно обновляющиеся зубы. Сохранять выразительность, не превращая портрет в хоррор-карикатуру. |
| Halfling | [Player Core, стр. 58](https://2e.aonprd.com/Ancestries.aspx?ID=63) | Низкий взрослый гуманоид, похожий на человека; возможна чуть более крупная голова. Кожа чаще насыщенных янтарных или древесно-коричневых оттенков, волосы от светло-золотистых до чёрных. Нельзя изображать ребёнком. Признаки стоп не используются в погрудном портрете. |
| Human | [Player Core, стр. 62](https://2e.aonprd.com/Ancestries.aspx?ID=64) | Максимально разнообразные взрослые люди: широкий диапазон оттенков кожи, волос, черт лица и телосложения. В партиях нужно осознанно менять фенотипы, а не закреплять один внешний вид как стандартный. |

## Проверенные источники classes

Внешние элементы класса ниже являются художественным переводом вводного описания, боевой роли и начальных proficiencies. Они не объявляются обязательной униформой класса.

| ClassId | AoN | Визуальный профиль для аватара |
|---|---|---|
| `class.bard` | [Bard, Player Core, стр. 94](https://2e.aonprd.com/Classes.aspx?ID=32) | Артист и знаток тайных знаний, использующий магические выступления. Выразительная одежда путешествующего исполнителя; один компактный инструмент или иной предмет выступления; мягкий оккультный свет как вторичный акцент. Лёгкая броня допустима. |
| `class.cleric` | [Cleric, Player Core, стр. 108](https://2e.aonprd.com/Classes.aspx?ID=33) | Служитель божества, носящий символ своей веры и владеющий дарованной божественной магией. Нейтральный оригинальный священный знак без копирования символов конкретного божества; мягкое защитное или исцеляющее свечение; простой любимый вид оружия допустим как фоновой элемент. |
| `class.druid` | [Druid, Player Core, стр. 122](https://2e.aonprd.com/Classes.aspx?ID=34) | Спокойный проводник первобытной силы природы. Практичная одежда или лёгкая/средняя броня с природными материалами; листья, ветви, камень или вода как сдержанный primal-акцент. Не закреплять один биом или один druidic order. |
| `class.fighter` | [Fighter, Player Core, стр. 136](https://2e.aonprd.com/Classes.aspx?ID=35) | Мастер оружия и боевых техник, способный быть рыцарем, наёмником, стрелком или мастером клинка. Ухоженная функциональная броня и одно ясно читаемое оружие; собранная защитная стойка. Не закреплять только тяжёлую броню или только меч. |
| `class.ranger` | [Ranger, Player Core, стр. 152](https://2e.aonprd.com/Classes.aspx?ID=36) | Охотник, следопыт и воин дикой местности. Выветренная практичная лёгкая или средняя броня, плащ или ремни походного снаряжения; лук либо одно ближнее оружие; едва заметный природный фон. Не добавлять animal companion без отдельного scope. |
| `class.rogue` | [Rogue, Player Core, стр. 164](https://2e.aonprd.com/Classes.aspx?ID=37) | Быстрый специалист по скрытности, точным атакам и множеству навыков. Ненавязчивая лёгкая броня, компактный клинок или инструменты, асимметричная тень. Не изображать обязательно преступником или в полностью чёрной одежде. |
| `class.witch` | [Witch, Player Core, стр. 178](https://2e.aonprd.com/Classes.aspx?ID=38) | Заклинатель, получающий знания от таинственного покровителя через необычного familiar. Familiar рядом с плечом является главным классовым маркером; второй маркер — сдержанная hex-магия, маленькая склянка или талисман. Familiar должен оставаться вторичным и не закрывать лицо. |
| `class.wizard` | [Wizard, Player Core, стр. 192](https://2e.aonprd.com/Classes.aspx?ID=39) | Учёный арканист, рассматривающий магию как дисциплину формул и текстов. Книга заклинаний, лист с формулами или письменные принадлежности; точный геометрический arcane-эффект. Без брони и без обязательной остроконечной шляпы. |

## Общий art direction

- Оригинальная стилизованно-реалистичная fantasy-иллюстрация, не имитирующая конкретного художника или официальную иллюстрацию Pathfinder.
- Один взрослый персонаж, портрет по грудь или плечи, допустим лёгкий ракурс 3/4.
- Квадратный исходник `1024×1024`; финальный экспорт `512×512 WebP`, качество около 85.
- Голова, лицо, уши и основной ancestry-признак полностью находятся в центральных 70% кадра.
- Круглый crop не должен срезать макушку, подбородок, уши или главный классовый предмет.
- Лицо и ancestry — первый акцент; одежда и ровно один хорошо читаемый классовый маркер — второй.
- Простой размытый фон без сюжетной сцены. Контраст должен сохраняться при `48×48`.
- Реалистичная фактура кожи, волос, ткани и материалов без гипердетализации и без фотореалистичного сходства с реальным человеком.
- Пол влияет на взрослую презентацию персонажа, но не ограничивает профессию, телосложение, цвет, причёску или экипировку.
- Варианты отличаются композицией и визуальной интерпретацией, а не только случайным seed.

## Варианты

### Variant 1

- лёгкий разворот влево;
- спокойное уверенное выражение;
- мягкий тёплый ключевой свет;
- наиболее узнаваемый классовый предмет хорошо читается у плеча;
- аккуратная, более собранная экипировка.

### Variant 2

- лёгкий разворот вправо;
- настороженное или сосредоточенное выражение;
- нейтрально-холодный ключевой свет;
- альтернативный допустимый классовый предмет или магический акцент;
- более походная, использованная экипировка и другой допустимый ancestry-фенотип.

## Общий negative prompt

```text
no text, letters, numbers, captions, logos, watermark, signature, border, frame, UI,
no copied Pathfinder character, no iconic named character, no imitation of an official illustration or living artist,
no extra person, crowd, duplicate face, extra head, extra arms, malformed hands,
no cropped crown, cropped chin, cropped ears, face outside the central safe area,
no child or childlike adult, no sexualized pose, no revealing costume, no gender stereotype costume,
no modern clothing, firearm, modern technology, sci-fi elements,
no busy narrative scene, detailed landscape, harsh background pattern,
no photorealistic real-person likeness, no plastic skin, no extreme gore, no horror distortion,
no ancestry features from a different profile, no class uniform treated as ancestry anatomy
```

## Шаблон production prompt

Подставляемые поля берутся из матрицы и таблиц выше. Текст промпта формируется на английском, чтобы все партии использовали одинаковые термины.

```text
Use case: stylized-concept
Asset type: character avatar for a fantasy RPG web UI
Primary request: create an original {gender_presentation} {ancestry} {class_name}, variant {variant}
Subject ancestry: {ancestry_visual_profile}
Class expression: {class_visual_profile}
Style/medium: original stylized-realistic fantasy digital painting; natural materials and believable anatomy; not based on any existing character or official artwork
Composition/framing: square 1024x1024 head-and-shoulders or chest-up portrait; {variant_composition}; face, crown, chin, ears, ancestry traits, and one class marker fully inside the central 70%; safe for a circular crop and readable at 48px
Lighting/mood: {variant_lighting}; focused, capable adventurer
Background: simple softly blurred fantasy-neutral backdrop with clear silhouette separation
Diversity: adult presentation; use a plausible phenotype and coloring within the ancestry profile; do not use profession, body type, hairstyle, or equipment as a gender stereotype
Constraints: one character only; ancestry is the primary visual identity; exactly one clear class marker is secondary; original design; no copied franchise character; no text, logo, border, UI, or watermark
Avoid: {common_negative_prompt}
```

`gender_presentation` принимает только `adult male-presenting` или `adult female-presenting`. `variant_composition` и `variant_lighting` берутся из раздела вариантов.

## Правила трассировки и QA

Для каждого результата проверяются:

1. `AvatarId`, путь, ancestry, class, gender и variant совпадают с одной строкой матрицы.
2. Ancestry-признаки соответствуют указанной странице AoN и не смешаны с другой ancestry.
3. Классовый маркер выводится из описания или proficiencies указанной class-страницы.
4. В кадре один взрослый персонаж; пол не выражен через карикатурные стереотипы.
5. Лицо и важные признаки сохраняются при квадратном ресайзе и круглом crop.
6. Изображение является оригинальной интерпретацией и не повторяет официальную композицию AoN.
7. После принятия исходник экспортируется в `512×512 WebP`; запись каталога добавляется только вместе с принятым файлом.

## Размер партий

Минимальная независимая партия — одна комбинация ancestry + class:

```text
2 genders × 2 variants = 4 assets
```

Более крупные безопасные срезы:

- ancestry + один class: 4 ассета;
- ancestry + четыре classes: 16 ассетов;
- одна ancestry целиком: 32 ассета.

Отсутствующие строки продолжают использовать `avatar.system.unknown`; ранее созданные персонажи не переназначаются.
