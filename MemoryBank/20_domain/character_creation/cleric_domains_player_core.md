# Cleric Primary Domains — первый уровень

## Назначение и граница

Документ нормализует primary domains, которые используются текущим [Player Core Deity catalog](deities_player_core.md), для выбора `Domain Initiate` Cloistered Cleric первого уровня.

Stable id domain строится как `domain.<snake_case_name>`, initial spell — как `spell.<snake_case_name>`. Alternate domains, advanced domain spells и spell effects не входят в этот baseline.

## Правило выбора

- Cloistered Cleric выбирает ровно один domain из `PrimaryDomainIds` выбранной Deity.
- Warpriest не получает domain choice на первом уровне.
- Выбор другого class, Doctrine или Deity удаляет несовместимый domain choice.
- Initial domain spell является focus spell rank `1`; focus pool применяется отдельным slice Priority 2.

## Нормализованный каталог

| Domain | Stable id | Initial focus spell | Spell id |
|---|---|---|---|
| Air | `domain.air` | Pushing Gust | `spell.pushing_gust` |
| Ambition | `domain.ambition` | Ignite Ambition | `spell.ignite_ambition` |
| Cities | `domain.cities` | Face in the Crowd | `spell.face_in_the_crowd` |
| Confidence | `domain.confidence` | Veil of Confidence | `spell.veil_of_confidence` |
| Creation | `domain.creation` | Creative Splash | `spell.creative_splash` |
| Darkness | `domain.darkness` | Cloak of Shadow | `spell.cloak_of_shadow` |
| Death | `domain.death` | Death's Call | `spell.death_s_call` |
| Destruction | `domain.destruction` | Cry of Destruction | `spell.cry_of_destruction` |
| Dreams | `domain.dreams` | Sweet Dream | `spell.sweet_dream` |
| Earth | `domain.earth` | Hurtling Stone | `spell.hurtling_stone` |
| Family | `domain.family` | Soothing Words | `spell.soothing_words` |
| Fate | `domain.fate` | Read Fate | `spell.read_fate` |
| Fire | `domain.fire` | Fire Ray | `spell.fire_ray` |
| Freedom | `domain.freedom` | Unimpeded Stride | `spell.unimpeded_stride` |
| Healing | `domain.healing` | Healer's Blessing | `spell.healer_s_blessing` |
| Indulgence | `domain.indulgence` | Overstuff | `spell.overstuff` |
| Knowledge | `domain.knowledge` | Scholarly Recollection | `spell.scholarly_recollection` |
| Luck | `domain.luck` | Bit of Luck | `spell.bit_of_luck` |
| Magic | `domain.magic` | Magic's Vessel | `spell.magic_s_vessel` |
| Metal | `domain.metal` | Serrate | `spell.serrate` |
| Might | `domain.might` | Athletic Rush | `spell.athletic_rush` |
| Moon | `domain.moon` | Moonbeam | `spell.moonbeam` |
| Nature | `domain.nature` | Vibrant Thorns | `spell.vibrant_thorns` |
| Nightmares | `domain.nightmares` | Waking Nightmare | `spell.waking_nightmare` |
| Pain | `domain.pain` | Savor the Sting | `spell.savor_the_sting` |
| Passion | `domain.passion` | Charming Touch | `spell.charming_touch` |
| Perfection | `domain.perfection` | Perfected Mind | `spell.perfected_mind` |
| Protection | `domain.protection` | Protector's Sacrifice | `spell.protector_s_sacrifice` |
| Secrecy | `domain.secrecy` | Whispering Quiet | `spell.whispering_quiet` |
| Sun | `domain.sun` | Dazzling Flash | `spell.dazzling_flash` |
| Travel | `domain.travel` | Agile Feet | `spell.agile_feet` |
| Trickery | `domain.trickery` | Sudden Shift | `spell.sudden_shift` |
| Truth | `domain.truth` | Word of Truth | `spell.word_of_truth` |
| Tyranny | `domain.tyranny` | Touch of Obedience | `spell.touch_of_obedience` |
| Undeath | `domain.undeath` | Touch of Undeath | `spell.touch_of_undeath` |
| Water | `domain.water` | Tidal Surge | `spell.tidal_surge` |
| Wealth | `domain.wealth` | Appearance of Wealth | `spell.appearance_of_wealth` |
| Wood | `domain.wood` | Arms of Nature | `spell.arms_of_nature` |
| Zeal | `domain.zeal` | Weapon Surge | `spell.weapon_surge` |

## Источники и совместимость

- Deity/domain baseline: `Player Core`, страницы 35–39.
- Remastered initial domain spells проверены по Archives of Nethys.
- `Metal` и `Wood` присутствуют у Green Faith в текущем Player Core deity baseline; их актуальные initial focus spells опубликованы в `Divine Mysteries`. Source конкретного spell будет храниться в Cleric Spell Catalog v1, поэтому domain choice не подменяет источник spell definition.

## Referential integrity

- каждый `PrimaryDomainId` Cleric-eligible Deity обязан разрешаться в этом каталоге;
- каждый `InitialFocusSpell.Id` обязан разрешаться в Cleric Spell Catalog v1 после slice 2.2;
- domain id сохраняется в персонаже, остальные metadata вычисляются из каталога.
