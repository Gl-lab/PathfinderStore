# Матрица тематических аватаров Player Core

## Контракт

Матрица резервирует 192 непрозрачных стабильных ID. Номер и ID не используются для восстановления metadata: источником условий выбора остаётся запись каталога.

Профили и production prompt: [avatar_generation_prompts.md](avatar_generation_prompts.md).

- Порядок резервирования: ancestry → class → gender → variant.
- Каждая строка независима и может быть реализована в любой партии.
- `BatchKey` объединяет четыре ассета одной комбинации ancestry + class.
- `AncestryProfile`, `ClassProfile` и `VariantProfile` являются ключами шаблона промпта, а не частью доменного ID.
- До принятия конкретного изображения строка остаётся планом и не добавляется в runtime-каталог.

## Проверка количества

```text
6 ancestries × 8 classes × 2 genders × 2 variants = 192 rows
```

## Полная матрица

| # | AvatarId | AssetPath | Ancestry | ClassId | Gender | Variant | AncestryProfile | ClassProfile | VariantProfile | BatchKey |
|---:|---|---|---|---|---|---:|---|---|---|---|
| 1 | `avatar.pc.000001` | `/avatars/pc/000001.webp` | Dwarf | `class.bard` | Male | 1 | `dwarf` | `bard` | `male-v1` | `pc-dwarf-bard` |
| 2 | `avatar.pc.000002` | `/avatars/pc/000002.webp` | Dwarf | `class.bard` | Male | 2 | `dwarf` | `bard` | `male-v2` | `pc-dwarf-bard` |
| 3 | `avatar.pc.000003` | `/avatars/pc/000003.webp` | Dwarf | `class.bard` | Female | 1 | `dwarf` | `bard` | `female-v1` | `pc-dwarf-bard` |
| 4 | `avatar.pc.000004` | `/avatars/pc/000004.webp` | Dwarf | `class.bard` | Female | 2 | `dwarf` | `bard` | `female-v2` | `pc-dwarf-bard` |
| 5 | `avatar.pc.000005` | `/avatars/pc/000005.webp` | Dwarf | `class.cleric` | Male | 1 | `dwarf` | `cleric` | `male-v1` | `pc-dwarf-cleric` |
| 6 | `avatar.pc.000006` | `/avatars/pc/000006.webp` | Dwarf | `class.cleric` | Male | 2 | `dwarf` | `cleric` | `male-v2` | `pc-dwarf-cleric` |
| 7 | `avatar.pc.000007` | `/avatars/pc/000007.webp` | Dwarf | `class.cleric` | Female | 1 | `dwarf` | `cleric` | `female-v1` | `pc-dwarf-cleric` |
| 8 | `avatar.pc.000008` | `/avatars/pc/000008.webp` | Dwarf | `class.cleric` | Female | 2 | `dwarf` | `cleric` | `female-v2` | `pc-dwarf-cleric` |
| 9 | `avatar.pc.000009` | `/avatars/pc/000009.webp` | Dwarf | `class.druid` | Male | 1 | `dwarf` | `druid` | `male-v1` | `pc-dwarf-druid` |
| 10 | `avatar.pc.000010` | `/avatars/pc/000010.webp` | Dwarf | `class.druid` | Male | 2 | `dwarf` | `druid` | `male-v2` | `pc-dwarf-druid` |
| 11 | `avatar.pc.000011` | `/avatars/pc/000011.webp` | Dwarf | `class.druid` | Female | 1 | `dwarf` | `druid` | `female-v1` | `pc-dwarf-druid` |
| 12 | `avatar.pc.000012` | `/avatars/pc/000012.webp` | Dwarf | `class.druid` | Female | 2 | `dwarf` | `druid` | `female-v2` | `pc-dwarf-druid` |
| 13 | `avatar.pc.000013` | `/avatars/pc/000013.webp` | Dwarf | `class.fighter` | Male | 1 | `dwarf` | `fighter` | `male-v1` | `pc-dwarf-fighter` |
| 14 | `avatar.pc.000014` | `/avatars/pc/000014.webp` | Dwarf | `class.fighter` | Male | 2 | `dwarf` | `fighter` | `male-v2` | `pc-dwarf-fighter` |
| 15 | `avatar.pc.000015` | `/avatars/pc/000015.webp` | Dwarf | `class.fighter` | Female | 1 | `dwarf` | `fighter` | `female-v1` | `pc-dwarf-fighter` |
| 16 | `avatar.pc.000016` | `/avatars/pc/000016.webp` | Dwarf | `class.fighter` | Female | 2 | `dwarf` | `fighter` | `female-v2` | `pc-dwarf-fighter` |
| 17 | `avatar.pc.000017` | `/avatars/pc/000017.webp` | Dwarf | `class.ranger` | Male | 1 | `dwarf` | `ranger` | `male-v1` | `pc-dwarf-ranger` |
| 18 | `avatar.pc.000018` | `/avatars/pc/000018.webp` | Dwarf | `class.ranger` | Male | 2 | `dwarf` | `ranger` | `male-v2` | `pc-dwarf-ranger` |
| 19 | `avatar.pc.000019` | `/avatars/pc/000019.webp` | Dwarf | `class.ranger` | Female | 1 | `dwarf` | `ranger` | `female-v1` | `pc-dwarf-ranger` |
| 20 | `avatar.pc.000020` | `/avatars/pc/000020.webp` | Dwarf | `class.ranger` | Female | 2 | `dwarf` | `ranger` | `female-v2` | `pc-dwarf-ranger` |
| 21 | `avatar.pc.000021` | `/avatars/pc/000021.webp` | Dwarf | `class.rogue` | Male | 1 | `dwarf` | `rogue` | `male-v1` | `pc-dwarf-rogue` |
| 22 | `avatar.pc.000022` | `/avatars/pc/000022.webp` | Dwarf | `class.rogue` | Male | 2 | `dwarf` | `rogue` | `male-v2` | `pc-dwarf-rogue` |
| 23 | `avatar.pc.000023` | `/avatars/pc/000023.webp` | Dwarf | `class.rogue` | Female | 1 | `dwarf` | `rogue` | `female-v1` | `pc-dwarf-rogue` |
| 24 | `avatar.pc.000024` | `/avatars/pc/000024.webp` | Dwarf | `class.rogue` | Female | 2 | `dwarf` | `rogue` | `female-v2` | `pc-dwarf-rogue` |
| 25 | `avatar.pc.000025` | `/avatars/pc/000025.webp` | Dwarf | `class.witch` | Male | 1 | `dwarf` | `witch` | `male-v1` | `pc-dwarf-witch` |
| 26 | `avatar.pc.000026` | `/avatars/pc/000026.webp` | Dwarf | `class.witch` | Male | 2 | `dwarf` | `witch` | `male-v2` | `pc-dwarf-witch` |
| 27 | `avatar.pc.000027` | `/avatars/pc/000027.webp` | Dwarf | `class.witch` | Female | 1 | `dwarf` | `witch` | `female-v1` | `pc-dwarf-witch` |
| 28 | `avatar.pc.000028` | `/avatars/pc/000028.webp` | Dwarf | `class.witch` | Female | 2 | `dwarf` | `witch` | `female-v2` | `pc-dwarf-witch` |
| 29 | `avatar.pc.000029` | `/avatars/pc/000029.webp` | Dwarf | `class.wizard` | Male | 1 | `dwarf` | `wizard` | `male-v1` | `pc-dwarf-wizard` |
| 30 | `avatar.pc.000030` | `/avatars/pc/000030.webp` | Dwarf | `class.wizard` | Male | 2 | `dwarf` | `wizard` | `male-v2` | `pc-dwarf-wizard` |
| 31 | `avatar.pc.000031` | `/avatars/pc/000031.webp` | Dwarf | `class.wizard` | Female | 1 | `dwarf` | `wizard` | `female-v1` | `pc-dwarf-wizard` |
| 32 | `avatar.pc.000032` | `/avatars/pc/000032.webp` | Dwarf | `class.wizard` | Female | 2 | `dwarf` | `wizard` | `female-v2` | `pc-dwarf-wizard` |
| 33 | `avatar.pc.000033` | `/avatars/pc/000033.webp` | Elf | `class.bard` | Male | 1 | `elf` | `bard` | `male-v1` | `pc-elf-bard` |
| 34 | `avatar.pc.000034` | `/avatars/pc/000034.webp` | Elf | `class.bard` | Male | 2 | `elf` | `bard` | `male-v2` | `pc-elf-bard` |
| 35 | `avatar.pc.000035` | `/avatars/pc/000035.webp` | Elf | `class.bard` | Female | 1 | `elf` | `bard` | `female-v1` | `pc-elf-bard` |
| 36 | `avatar.pc.000036` | `/avatars/pc/000036.webp` | Elf | `class.bard` | Female | 2 | `elf` | `bard` | `female-v2` | `pc-elf-bard` |
| 37 | `avatar.pc.000037` | `/avatars/pc/000037.webp` | Elf | `class.cleric` | Male | 1 | `elf` | `cleric` | `male-v1` | `pc-elf-cleric` |
| 38 | `avatar.pc.000038` | `/avatars/pc/000038.webp` | Elf | `class.cleric` | Male | 2 | `elf` | `cleric` | `male-v2` | `pc-elf-cleric` |
| 39 | `avatar.pc.000039` | `/avatars/pc/000039.webp` | Elf | `class.cleric` | Female | 1 | `elf` | `cleric` | `female-v1` | `pc-elf-cleric` |
| 40 | `avatar.pc.000040` | `/avatars/pc/000040.webp` | Elf | `class.cleric` | Female | 2 | `elf` | `cleric` | `female-v2` | `pc-elf-cleric` |
| 41 | `avatar.pc.000041` | `/avatars/pc/000041.webp` | Elf | `class.druid` | Male | 1 | `elf` | `druid` | `male-v1` | `pc-elf-druid` |
| 42 | `avatar.pc.000042` | `/avatars/pc/000042.webp` | Elf | `class.druid` | Male | 2 | `elf` | `druid` | `male-v2` | `pc-elf-druid` |
| 43 | `avatar.pc.000043` | `/avatars/pc/000043.webp` | Elf | `class.druid` | Female | 1 | `elf` | `druid` | `female-v1` | `pc-elf-druid` |
| 44 | `avatar.pc.000044` | `/avatars/pc/000044.webp` | Elf | `class.druid` | Female | 2 | `elf` | `druid` | `female-v2` | `pc-elf-druid` |
| 45 | `avatar.pc.000045` | `/avatars/pc/000045.webp` | Elf | `class.fighter` | Male | 1 | `elf` | `fighter` | `male-v1` | `pc-elf-fighter` |
| 46 | `avatar.pc.000046` | `/avatars/pc/000046.webp` | Elf | `class.fighter` | Male | 2 | `elf` | `fighter` | `male-v2` | `pc-elf-fighter` |
| 47 | `avatar.pc.000047` | `/avatars/pc/000047.webp` | Elf | `class.fighter` | Female | 1 | `elf` | `fighter` | `female-v1` | `pc-elf-fighter` |
| 48 | `avatar.pc.000048` | `/avatars/pc/000048.webp` | Elf | `class.fighter` | Female | 2 | `elf` | `fighter` | `female-v2` | `pc-elf-fighter` |
| 49 | `avatar.pc.000049` | `/avatars/pc/000049.webp` | Elf | `class.ranger` | Male | 1 | `elf` | `ranger` | `male-v1` | `pc-elf-ranger` |
| 50 | `avatar.pc.000050` | `/avatars/pc/000050.webp` | Elf | `class.ranger` | Male | 2 | `elf` | `ranger` | `male-v2` | `pc-elf-ranger` |
| 51 | `avatar.pc.000051` | `/avatars/pc/000051.webp` | Elf | `class.ranger` | Female | 1 | `elf` | `ranger` | `female-v1` | `pc-elf-ranger` |
| 52 | `avatar.pc.000052` | `/avatars/pc/000052.webp` | Elf | `class.ranger` | Female | 2 | `elf` | `ranger` | `female-v2` | `pc-elf-ranger` |
| 53 | `avatar.pc.000053` | `/avatars/pc/000053.webp` | Elf | `class.rogue` | Male | 1 | `elf` | `rogue` | `male-v1` | `pc-elf-rogue` |
| 54 | `avatar.pc.000054` | `/avatars/pc/000054.webp` | Elf | `class.rogue` | Male | 2 | `elf` | `rogue` | `male-v2` | `pc-elf-rogue` |
| 55 | `avatar.pc.000055` | `/avatars/pc/000055.webp` | Elf | `class.rogue` | Female | 1 | `elf` | `rogue` | `female-v1` | `pc-elf-rogue` |
| 56 | `avatar.pc.000056` | `/avatars/pc/000056.webp` | Elf | `class.rogue` | Female | 2 | `elf` | `rogue` | `female-v2` | `pc-elf-rogue` |
| 57 | `avatar.pc.000057` | `/avatars/pc/000057.webp` | Elf | `class.witch` | Male | 1 | `elf` | `witch` | `male-v1` | `pc-elf-witch` |
| 58 | `avatar.pc.000058` | `/avatars/pc/000058.webp` | Elf | `class.witch` | Male | 2 | `elf` | `witch` | `male-v2` | `pc-elf-witch` |
| 59 | `avatar.pc.000059` | `/avatars/pc/000059.webp` | Elf | `class.witch` | Female | 1 | `elf` | `witch` | `female-v1` | `pc-elf-witch` |
| 60 | `avatar.pc.000060` | `/avatars/pc/000060.webp` | Elf | `class.witch` | Female | 2 | `elf` | `witch` | `female-v2` | `pc-elf-witch` |
| 61 | `avatar.pc.000061` | `/avatars/pc/000061.webp` | Elf | `class.wizard` | Male | 1 | `elf` | `wizard` | `male-v1` | `pc-elf-wizard` |
| 62 | `avatar.pc.000062` | `/avatars/pc/000062.webp` | Elf | `class.wizard` | Male | 2 | `elf` | `wizard` | `male-v2` | `pc-elf-wizard` |
| 63 | `avatar.pc.000063` | `/avatars/pc/000063.webp` | Elf | `class.wizard` | Female | 1 | `elf` | `wizard` | `female-v1` | `pc-elf-wizard` |
| 64 | `avatar.pc.000064` | `/avatars/pc/000064.webp` | Elf | `class.wizard` | Female | 2 | `elf` | `wizard` | `female-v2` | `pc-elf-wizard` |
| 65 | `avatar.pc.000065` | `/avatars/pc/000065.webp` | Gnome | `class.bard` | Male | 1 | `gnome` | `bard` | `male-v1` | `pc-gnome-bard` |
| 66 | `avatar.pc.000066` | `/avatars/pc/000066.webp` | Gnome | `class.bard` | Male | 2 | `gnome` | `bard` | `male-v2` | `pc-gnome-bard` |
| 67 | `avatar.pc.000067` | `/avatars/pc/000067.webp` | Gnome | `class.bard` | Female | 1 | `gnome` | `bard` | `female-v1` | `pc-gnome-bard` |
| 68 | `avatar.pc.000068` | `/avatars/pc/000068.webp` | Gnome | `class.bard` | Female | 2 | `gnome` | `bard` | `female-v2` | `pc-gnome-bard` |
| 69 | `avatar.pc.000069` | `/avatars/pc/000069.webp` | Gnome | `class.cleric` | Male | 1 | `gnome` | `cleric` | `male-v1` | `pc-gnome-cleric` |
| 70 | `avatar.pc.000070` | `/avatars/pc/000070.webp` | Gnome | `class.cleric` | Male | 2 | `gnome` | `cleric` | `male-v2` | `pc-gnome-cleric` |
| 71 | `avatar.pc.000071` | `/avatars/pc/000071.webp` | Gnome | `class.cleric` | Female | 1 | `gnome` | `cleric` | `female-v1` | `pc-gnome-cleric` |
| 72 | `avatar.pc.000072` | `/avatars/pc/000072.webp` | Gnome | `class.cleric` | Female | 2 | `gnome` | `cleric` | `female-v2` | `pc-gnome-cleric` |
| 73 | `avatar.pc.000073` | `/avatars/pc/000073.webp` | Gnome | `class.druid` | Male | 1 | `gnome` | `druid` | `male-v1` | `pc-gnome-druid` |
| 74 | `avatar.pc.000074` | `/avatars/pc/000074.webp` | Gnome | `class.druid` | Male | 2 | `gnome` | `druid` | `male-v2` | `pc-gnome-druid` |
| 75 | `avatar.pc.000075` | `/avatars/pc/000075.webp` | Gnome | `class.druid` | Female | 1 | `gnome` | `druid` | `female-v1` | `pc-gnome-druid` |
| 76 | `avatar.pc.000076` | `/avatars/pc/000076.webp` | Gnome | `class.druid` | Female | 2 | `gnome` | `druid` | `female-v2` | `pc-gnome-druid` |
| 77 | `avatar.pc.000077` | `/avatars/pc/000077.webp` | Gnome | `class.fighter` | Male | 1 | `gnome` | `fighter` | `male-v1` | `pc-gnome-fighter` |
| 78 | `avatar.pc.000078` | `/avatars/pc/000078.webp` | Gnome | `class.fighter` | Male | 2 | `gnome` | `fighter` | `male-v2` | `pc-gnome-fighter` |
| 79 | `avatar.pc.000079` | `/avatars/pc/000079.webp` | Gnome | `class.fighter` | Female | 1 | `gnome` | `fighter` | `female-v1` | `pc-gnome-fighter` |
| 80 | `avatar.pc.000080` | `/avatars/pc/000080.webp` | Gnome | `class.fighter` | Female | 2 | `gnome` | `fighter` | `female-v2` | `pc-gnome-fighter` |
| 81 | `avatar.pc.000081` | `/avatars/pc/000081.webp` | Gnome | `class.ranger` | Male | 1 | `gnome` | `ranger` | `male-v1` | `pc-gnome-ranger` |
| 82 | `avatar.pc.000082` | `/avatars/pc/000082.webp` | Gnome | `class.ranger` | Male | 2 | `gnome` | `ranger` | `male-v2` | `pc-gnome-ranger` |
| 83 | `avatar.pc.000083` | `/avatars/pc/000083.webp` | Gnome | `class.ranger` | Female | 1 | `gnome` | `ranger` | `female-v1` | `pc-gnome-ranger` |
| 84 | `avatar.pc.000084` | `/avatars/pc/000084.webp` | Gnome | `class.ranger` | Female | 2 | `gnome` | `ranger` | `female-v2` | `pc-gnome-ranger` |
| 85 | `avatar.pc.000085` | `/avatars/pc/000085.webp` | Gnome | `class.rogue` | Male | 1 | `gnome` | `rogue` | `male-v1` | `pc-gnome-rogue` |
| 86 | `avatar.pc.000086` | `/avatars/pc/000086.webp` | Gnome | `class.rogue` | Male | 2 | `gnome` | `rogue` | `male-v2` | `pc-gnome-rogue` |
| 87 | `avatar.pc.000087` | `/avatars/pc/000087.webp` | Gnome | `class.rogue` | Female | 1 | `gnome` | `rogue` | `female-v1` | `pc-gnome-rogue` |
| 88 | `avatar.pc.000088` | `/avatars/pc/000088.webp` | Gnome | `class.rogue` | Female | 2 | `gnome` | `rogue` | `female-v2` | `pc-gnome-rogue` |
| 89 | `avatar.pc.000089` | `/avatars/pc/000089.webp` | Gnome | `class.witch` | Male | 1 | `gnome` | `witch` | `male-v1` | `pc-gnome-witch` |
| 90 | `avatar.pc.000090` | `/avatars/pc/000090.webp` | Gnome | `class.witch` | Male | 2 | `gnome` | `witch` | `male-v2` | `pc-gnome-witch` |
| 91 | `avatar.pc.000091` | `/avatars/pc/000091.webp` | Gnome | `class.witch` | Female | 1 | `gnome` | `witch` | `female-v1` | `pc-gnome-witch` |
| 92 | `avatar.pc.000092` | `/avatars/pc/000092.webp` | Gnome | `class.witch` | Female | 2 | `gnome` | `witch` | `female-v2` | `pc-gnome-witch` |
| 93 | `avatar.pc.000093` | `/avatars/pc/000093.webp` | Gnome | `class.wizard` | Male | 1 | `gnome` | `wizard` | `male-v1` | `pc-gnome-wizard` |
| 94 | `avatar.pc.000094` | `/avatars/pc/000094.webp` | Gnome | `class.wizard` | Male | 2 | `gnome` | `wizard` | `male-v2` | `pc-gnome-wizard` |
| 95 | `avatar.pc.000095` | `/avatars/pc/000095.webp` | Gnome | `class.wizard` | Female | 1 | `gnome` | `wizard` | `female-v1` | `pc-gnome-wizard` |
| 96 | `avatar.pc.000096` | `/avatars/pc/000096.webp` | Gnome | `class.wizard` | Female | 2 | `gnome` | `wizard` | `female-v2` | `pc-gnome-wizard` |
| 97 | `avatar.pc.000097` | `/avatars/pc/000097.webp` | Goblin | `class.bard` | Male | 1 | `goblin` | `bard` | `male-v1` | `pc-goblin-bard` |
| 98 | `avatar.pc.000098` | `/avatars/pc/000098.webp` | Goblin | `class.bard` | Male | 2 | `goblin` | `bard` | `male-v2` | `pc-goblin-bard` |
| 99 | `avatar.pc.000099` | `/avatars/pc/000099.webp` | Goblin | `class.bard` | Female | 1 | `goblin` | `bard` | `female-v1` | `pc-goblin-bard` |
| 100 | `avatar.pc.000100` | `/avatars/pc/000100.webp` | Goblin | `class.bard` | Female | 2 | `goblin` | `bard` | `female-v2` | `pc-goblin-bard` |
| 101 | `avatar.pc.000101` | `/avatars/pc/000101.webp` | Goblin | `class.cleric` | Male | 1 | `goblin` | `cleric` | `male-v1` | `pc-goblin-cleric` |
| 102 | `avatar.pc.000102` | `/avatars/pc/000102.webp` | Goblin | `class.cleric` | Male | 2 | `goblin` | `cleric` | `male-v2` | `pc-goblin-cleric` |
| 103 | `avatar.pc.000103` | `/avatars/pc/000103.webp` | Goblin | `class.cleric` | Female | 1 | `goblin` | `cleric` | `female-v1` | `pc-goblin-cleric` |
| 104 | `avatar.pc.000104` | `/avatars/pc/000104.webp` | Goblin | `class.cleric` | Female | 2 | `goblin` | `cleric` | `female-v2` | `pc-goblin-cleric` |
| 105 | `avatar.pc.000105` | `/avatars/pc/000105.webp` | Goblin | `class.druid` | Male | 1 | `goblin` | `druid` | `male-v1` | `pc-goblin-druid` |
| 106 | `avatar.pc.000106` | `/avatars/pc/000106.webp` | Goblin | `class.druid` | Male | 2 | `goblin` | `druid` | `male-v2` | `pc-goblin-druid` |
| 107 | `avatar.pc.000107` | `/avatars/pc/000107.webp` | Goblin | `class.druid` | Female | 1 | `goblin` | `druid` | `female-v1` | `pc-goblin-druid` |
| 108 | `avatar.pc.000108` | `/avatars/pc/000108.webp` | Goblin | `class.druid` | Female | 2 | `goblin` | `druid` | `female-v2` | `pc-goblin-druid` |
| 109 | `avatar.pc.000109` | `/avatars/pc/000109.webp` | Goblin | `class.fighter` | Male | 1 | `goblin` | `fighter` | `male-v1` | `pc-goblin-fighter` |
| 110 | `avatar.pc.000110` | `/avatars/pc/000110.webp` | Goblin | `class.fighter` | Male | 2 | `goblin` | `fighter` | `male-v2` | `pc-goblin-fighter` |
| 111 | `avatar.pc.000111` | `/avatars/pc/000111.webp` | Goblin | `class.fighter` | Female | 1 | `goblin` | `fighter` | `female-v1` | `pc-goblin-fighter` |
| 112 | `avatar.pc.000112` | `/avatars/pc/000112.webp` | Goblin | `class.fighter` | Female | 2 | `goblin` | `fighter` | `female-v2` | `pc-goblin-fighter` |
| 113 | `avatar.pc.000113` | `/avatars/pc/000113.webp` | Goblin | `class.ranger` | Male | 1 | `goblin` | `ranger` | `male-v1` | `pc-goblin-ranger` |
| 114 | `avatar.pc.000114` | `/avatars/pc/000114.webp` | Goblin | `class.ranger` | Male | 2 | `goblin` | `ranger` | `male-v2` | `pc-goblin-ranger` |
| 115 | `avatar.pc.000115` | `/avatars/pc/000115.webp` | Goblin | `class.ranger` | Female | 1 | `goblin` | `ranger` | `female-v1` | `pc-goblin-ranger` |
| 116 | `avatar.pc.000116` | `/avatars/pc/000116.webp` | Goblin | `class.ranger` | Female | 2 | `goblin` | `ranger` | `female-v2` | `pc-goblin-ranger` |
| 117 | `avatar.pc.000117` | `/avatars/pc/000117.webp` | Goblin | `class.rogue` | Male | 1 | `goblin` | `rogue` | `male-v1` | `pc-goblin-rogue` |
| 118 | `avatar.pc.000118` | `/avatars/pc/000118.webp` | Goblin | `class.rogue` | Male | 2 | `goblin` | `rogue` | `male-v2` | `pc-goblin-rogue` |
| 119 | `avatar.pc.000119` | `/avatars/pc/000119.webp` | Goblin | `class.rogue` | Female | 1 | `goblin` | `rogue` | `female-v1` | `pc-goblin-rogue` |
| 120 | `avatar.pc.000120` | `/avatars/pc/000120.webp` | Goblin | `class.rogue` | Female | 2 | `goblin` | `rogue` | `female-v2` | `pc-goblin-rogue` |
| 121 | `avatar.pc.000121` | `/avatars/pc/000121.webp` | Goblin | `class.witch` | Male | 1 | `goblin` | `witch` | `male-v1` | `pc-goblin-witch` |
| 122 | `avatar.pc.000122` | `/avatars/pc/000122.webp` | Goblin | `class.witch` | Male | 2 | `goblin` | `witch` | `male-v2` | `pc-goblin-witch` |
| 123 | `avatar.pc.000123` | `/avatars/pc/000123.webp` | Goblin | `class.witch` | Female | 1 | `goblin` | `witch` | `female-v1` | `pc-goblin-witch` |
| 124 | `avatar.pc.000124` | `/avatars/pc/000124.webp` | Goblin | `class.witch` | Female | 2 | `goblin` | `witch` | `female-v2` | `pc-goblin-witch` |
| 125 | `avatar.pc.000125` | `/avatars/pc/000125.webp` | Goblin | `class.wizard` | Male | 1 | `goblin` | `wizard` | `male-v1` | `pc-goblin-wizard` |
| 126 | `avatar.pc.000126` | `/avatars/pc/000126.webp` | Goblin | `class.wizard` | Male | 2 | `goblin` | `wizard` | `male-v2` | `pc-goblin-wizard` |
| 127 | `avatar.pc.000127` | `/avatars/pc/000127.webp` | Goblin | `class.wizard` | Female | 1 | `goblin` | `wizard` | `female-v1` | `pc-goblin-wizard` |
| 128 | `avatar.pc.000128` | `/avatars/pc/000128.webp` | Goblin | `class.wizard` | Female | 2 | `goblin` | `wizard` | `female-v2` | `pc-goblin-wizard` |
| 129 | `avatar.pc.000129` | `/avatars/pc/000129.webp` | Halfling | `class.bard` | Male | 1 | `halfling` | `bard` | `male-v1` | `pc-halfling-bard` |
| 130 | `avatar.pc.000130` | `/avatars/pc/000130.webp` | Halfling | `class.bard` | Male | 2 | `halfling` | `bard` | `male-v2` | `pc-halfling-bard` |
| 131 | `avatar.pc.000131` | `/avatars/pc/000131.webp` | Halfling | `class.bard` | Female | 1 | `halfling` | `bard` | `female-v1` | `pc-halfling-bard` |
| 132 | `avatar.pc.000132` | `/avatars/pc/000132.webp` | Halfling | `class.bard` | Female | 2 | `halfling` | `bard` | `female-v2` | `pc-halfling-bard` |
| 133 | `avatar.pc.000133` | `/avatars/pc/000133.webp` | Halfling | `class.cleric` | Male | 1 | `halfling` | `cleric` | `male-v1` | `pc-halfling-cleric` |
| 134 | `avatar.pc.000134` | `/avatars/pc/000134.webp` | Halfling | `class.cleric` | Male | 2 | `halfling` | `cleric` | `male-v2` | `pc-halfling-cleric` |
| 135 | `avatar.pc.000135` | `/avatars/pc/000135.webp` | Halfling | `class.cleric` | Female | 1 | `halfling` | `cleric` | `female-v1` | `pc-halfling-cleric` |
| 136 | `avatar.pc.000136` | `/avatars/pc/000136.webp` | Halfling | `class.cleric` | Female | 2 | `halfling` | `cleric` | `female-v2` | `pc-halfling-cleric` |
| 137 | `avatar.pc.000137` | `/avatars/pc/000137.webp` | Halfling | `class.druid` | Male | 1 | `halfling` | `druid` | `male-v1` | `pc-halfling-druid` |
| 138 | `avatar.pc.000138` | `/avatars/pc/000138.webp` | Halfling | `class.druid` | Male | 2 | `halfling` | `druid` | `male-v2` | `pc-halfling-druid` |
| 139 | `avatar.pc.000139` | `/avatars/pc/000139.webp` | Halfling | `class.druid` | Female | 1 | `halfling` | `druid` | `female-v1` | `pc-halfling-druid` |
| 140 | `avatar.pc.000140` | `/avatars/pc/000140.webp` | Halfling | `class.druid` | Female | 2 | `halfling` | `druid` | `female-v2` | `pc-halfling-druid` |
| 141 | `avatar.pc.000141` | `/avatars/pc/000141.webp` | Halfling | `class.fighter` | Male | 1 | `halfling` | `fighter` | `male-v1` | `pc-halfling-fighter` |
| 142 | `avatar.pc.000142` | `/avatars/pc/000142.webp` | Halfling | `class.fighter` | Male | 2 | `halfling` | `fighter` | `male-v2` | `pc-halfling-fighter` |
| 143 | `avatar.pc.000143` | `/avatars/pc/000143.webp` | Halfling | `class.fighter` | Female | 1 | `halfling` | `fighter` | `female-v1` | `pc-halfling-fighter` |
| 144 | `avatar.pc.000144` | `/avatars/pc/000144.webp` | Halfling | `class.fighter` | Female | 2 | `halfling` | `fighter` | `female-v2` | `pc-halfling-fighter` |
| 145 | `avatar.pc.000145` | `/avatars/pc/000145.webp` | Halfling | `class.ranger` | Male | 1 | `halfling` | `ranger` | `male-v1` | `pc-halfling-ranger` |
| 146 | `avatar.pc.000146` | `/avatars/pc/000146.webp` | Halfling | `class.ranger` | Male | 2 | `halfling` | `ranger` | `male-v2` | `pc-halfling-ranger` |
| 147 | `avatar.pc.000147` | `/avatars/pc/000147.webp` | Halfling | `class.ranger` | Female | 1 | `halfling` | `ranger` | `female-v1` | `pc-halfling-ranger` |
| 148 | `avatar.pc.000148` | `/avatars/pc/000148.webp` | Halfling | `class.ranger` | Female | 2 | `halfling` | `ranger` | `female-v2` | `pc-halfling-ranger` |
| 149 | `avatar.pc.000149` | `/avatars/pc/000149.webp` | Halfling | `class.rogue` | Male | 1 | `halfling` | `rogue` | `male-v1` | `pc-halfling-rogue` |
| 150 | `avatar.pc.000150` | `/avatars/pc/000150.webp` | Halfling | `class.rogue` | Male | 2 | `halfling` | `rogue` | `male-v2` | `pc-halfling-rogue` |
| 151 | `avatar.pc.000151` | `/avatars/pc/000151.webp` | Halfling | `class.rogue` | Female | 1 | `halfling` | `rogue` | `female-v1` | `pc-halfling-rogue` |
| 152 | `avatar.pc.000152` | `/avatars/pc/000152.webp` | Halfling | `class.rogue` | Female | 2 | `halfling` | `rogue` | `female-v2` | `pc-halfling-rogue` |
| 153 | `avatar.pc.000153` | `/avatars/pc/000153.webp` | Halfling | `class.witch` | Male | 1 | `halfling` | `witch` | `male-v1` | `pc-halfling-witch` |
| 154 | `avatar.pc.000154` | `/avatars/pc/000154.webp` | Halfling | `class.witch` | Male | 2 | `halfling` | `witch` | `male-v2` | `pc-halfling-witch` |
| 155 | `avatar.pc.000155` | `/avatars/pc/000155.webp` | Halfling | `class.witch` | Female | 1 | `halfling` | `witch` | `female-v1` | `pc-halfling-witch` |
| 156 | `avatar.pc.000156` | `/avatars/pc/000156.webp` | Halfling | `class.witch` | Female | 2 | `halfling` | `witch` | `female-v2` | `pc-halfling-witch` |
| 157 | `avatar.pc.000157` | `/avatars/pc/000157.webp` | Halfling | `class.wizard` | Male | 1 | `halfling` | `wizard` | `male-v1` | `pc-halfling-wizard` |
| 158 | `avatar.pc.000158` | `/avatars/pc/000158.webp` | Halfling | `class.wizard` | Male | 2 | `halfling` | `wizard` | `male-v2` | `pc-halfling-wizard` |
| 159 | `avatar.pc.000159` | `/avatars/pc/000159.webp` | Halfling | `class.wizard` | Female | 1 | `halfling` | `wizard` | `female-v1` | `pc-halfling-wizard` |
| 160 | `avatar.pc.000160` | `/avatars/pc/000160.webp` | Halfling | `class.wizard` | Female | 2 | `halfling` | `wizard` | `female-v2` | `pc-halfling-wizard` |
| 161 | `avatar.pc.000161` | `/avatars/pc/000161.webp` | Human | `class.bard` | Male | 1 | `human` | `bard` | `male-v1` | `pc-human-bard` |
| 162 | `avatar.pc.000162` | `/avatars/pc/000162.webp` | Human | `class.bard` | Male | 2 | `human` | `bard` | `male-v2` | `pc-human-bard` |
| 163 | `avatar.pc.000163` | `/avatars/pc/000163.webp` | Human | `class.bard` | Female | 1 | `human` | `bard` | `female-v1` | `pc-human-bard` |
| 164 | `avatar.pc.000164` | `/avatars/pc/000164.webp` | Human | `class.bard` | Female | 2 | `human` | `bard` | `female-v2` | `pc-human-bard` |
| 165 | `avatar.pc.000165` | `/avatars/pc/000165.webp` | Human | `class.cleric` | Male | 1 | `human` | `cleric` | `male-v1` | `pc-human-cleric` |
| 166 | `avatar.pc.000166` | `/avatars/pc/000166.webp` | Human | `class.cleric` | Male | 2 | `human` | `cleric` | `male-v2` | `pc-human-cleric` |
| 167 | `avatar.pc.000167` | `/avatars/pc/000167.webp` | Human | `class.cleric` | Female | 1 | `human` | `cleric` | `female-v1` | `pc-human-cleric` |
| 168 | `avatar.pc.000168` | `/avatars/pc/000168.webp` | Human | `class.cleric` | Female | 2 | `human` | `cleric` | `female-v2` | `pc-human-cleric` |
| 169 | `avatar.pc.000169` | `/avatars/pc/000169.webp` | Human | `class.druid` | Male | 1 | `human` | `druid` | `male-v1` | `pc-human-druid` |
| 170 | `avatar.pc.000170` | `/avatars/pc/000170.webp` | Human | `class.druid` | Male | 2 | `human` | `druid` | `male-v2` | `pc-human-druid` |
| 171 | `avatar.pc.000171` | `/avatars/pc/000171.webp` | Human | `class.druid` | Female | 1 | `human` | `druid` | `female-v1` | `pc-human-druid` |
| 172 | `avatar.pc.000172` | `/avatars/pc/000172.webp` | Human | `class.druid` | Female | 2 | `human` | `druid` | `female-v2` | `pc-human-druid` |
| 173 | `avatar.pc.000173` | `/avatars/pc/000173.webp` | Human | `class.fighter` | Male | 1 | `human` | `fighter` | `male-v1` | `pc-human-fighter` |
| 174 | `avatar.pc.000174` | `/avatars/pc/000174.webp` | Human | `class.fighter` | Male | 2 | `human` | `fighter` | `male-v2` | `pc-human-fighter` |
| 175 | `avatar.pc.000175` | `/avatars/pc/000175.webp` | Human | `class.fighter` | Female | 1 | `human` | `fighter` | `female-v1` | `pc-human-fighter` |
| 176 | `avatar.pc.000176` | `/avatars/pc/000176.webp` | Human | `class.fighter` | Female | 2 | `human` | `fighter` | `female-v2` | `pc-human-fighter` |
| 177 | `avatar.pc.000177` | `/avatars/pc/000177.webp` | Human | `class.ranger` | Male | 1 | `human` | `ranger` | `male-v1` | `pc-human-ranger` |
| 178 | `avatar.pc.000178` | `/avatars/pc/000178.webp` | Human | `class.ranger` | Male | 2 | `human` | `ranger` | `male-v2` | `pc-human-ranger` |
| 179 | `avatar.pc.000179` | `/avatars/pc/000179.webp` | Human | `class.ranger` | Female | 1 | `human` | `ranger` | `female-v1` | `pc-human-ranger` |
| 180 | `avatar.pc.000180` | `/avatars/pc/000180.webp` | Human | `class.ranger` | Female | 2 | `human` | `ranger` | `female-v2` | `pc-human-ranger` |
| 181 | `avatar.pc.000181` | `/avatars/pc/000181.webp` | Human | `class.rogue` | Male | 1 | `human` | `rogue` | `male-v1` | `pc-human-rogue` |
| 182 | `avatar.pc.000182` | `/avatars/pc/000182.webp` | Human | `class.rogue` | Male | 2 | `human` | `rogue` | `male-v2` | `pc-human-rogue` |
| 183 | `avatar.pc.000183` | `/avatars/pc/000183.webp` | Human | `class.rogue` | Female | 1 | `human` | `rogue` | `female-v1` | `pc-human-rogue` |
| 184 | `avatar.pc.000184` | `/avatars/pc/000184.webp` | Human | `class.rogue` | Female | 2 | `human` | `rogue` | `female-v2` | `pc-human-rogue` |
| 185 | `avatar.pc.000185` | `/avatars/pc/000185.webp` | Human | `class.witch` | Male | 1 | `human` | `witch` | `male-v1` | `pc-human-witch` |
| 186 | `avatar.pc.000186` | `/avatars/pc/000186.webp` | Human | `class.witch` | Male | 2 | `human` | `witch` | `male-v2` | `pc-human-witch` |
| 187 | `avatar.pc.000187` | `/avatars/pc/000187.webp` | Human | `class.witch` | Female | 1 | `human` | `witch` | `female-v1` | `pc-human-witch` |
| 188 | `avatar.pc.000188` | `/avatars/pc/000188.webp` | Human | `class.witch` | Female | 2 | `human` | `witch` | `female-v2` | `pc-human-witch` |
| 189 | `avatar.pc.000189` | `/avatars/pc/000189.webp` | Human | `class.wizard` | Male | 1 | `human` | `wizard` | `male-v1` | `pc-human-wizard` |
| 190 | `avatar.pc.000190` | `/avatars/pc/000190.webp` | Human | `class.wizard` | Male | 2 | `human` | `wizard` | `male-v2` | `pc-human-wizard` |
| 191 | `avatar.pc.000191` | `/avatars/pc/000191.webp` | Human | `class.wizard` | Female | 1 | `human` | `wizard` | `female-v1` | `pc-human-wizard` |
| 192 | `avatar.pc.000192` | `/avatars/pc/000192.webp` | Human | `class.wizard` | Female | 2 | `human` | `wizard` | `female-v2` | `pc-human-wizard` |

