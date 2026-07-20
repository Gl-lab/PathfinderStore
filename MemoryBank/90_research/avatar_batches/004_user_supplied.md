# Партия 004: предоставленные аватары 000016–000020

## Scope

Пять исходников предоставлены владельцем продукта в каталоге `tempinfo/` как непрозрачные PNG `1024×1024`. Файлы сопоставлены со следующими строками [матрицы ассетов](../avatar_asset_matrix.md), проверены визуально и экспортированы в `512×512 WebP` с качеством 88. Исходные PNG не изменялись и не удалялись.

| AvatarId | Metadata | Исходник | Итоговый файл |
|---|---|---|---|
| `avatar.pc.000016` | Dwarf, `class.fighter`, Female, variant 2 | `tempinfo/16.png` | `pathfinder.frontend/public/avatars/pc/000016.webp` |
| `avatar.pc.000017` | Dwarf, `class.ranger`, Male, variant 1 | `tempinfo/17.png` | `pathfinder.frontend/public/avatars/pc/000017.webp` |
| `avatar.pc.000018` | Dwarf, `class.ranger`, Male, variant 2 | `tempinfo/18.png` | `pathfinder.frontend/public/avatars/pc/000018.webp` |
| `avatar.pc.000019` | Dwarf, `class.ranger`, Female, variant 1 | `tempinfo/19.png` | `pathfinder.frontend/public/avatars/pc/000019.webp` |
| `avatar.pc.000020` | Dwarf, `class.ranger`, Female, variant 2 | `tempinfo/20.png` | `pathfinder.frontend/public/avatars/pc/000020.webp` |

## Визуальный QA

- в каждом кадре один взрослый персонаж с читаемыми Dwarf-признаками;
- Fighter и Ranger различимы через функциональную экипировку и оружие;
- лица и ancestry-признаки сохраняются при круглом crop;
- частичная обрезка оружия границей круга допустима по принятому продуктовому решению, если тип оружия остаётся читаемым;
- силуэты и основные цветовые блоки различимы при `48×48`;
- исходники непрозрачны, итоговые файлы имеют `512×512`, RGB WebP;
- Dwarf Rogue и остальные непокрытые сочетания продолжают использовать `avatar.system.unknown`.
