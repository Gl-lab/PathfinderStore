import type { CharacterClass, DruidSpellOptions } from './api'

export function isDruidSpellLoadoutComplete(
  characterClass: CharacterClass | null,
  cantripIds: string[],
  preparedSpellIds: (string | null)[],
  options: DruidSpellOptions,
): boolean {
  if (characterClass?.id !== 'class.druid')
    return cantripIds.length === 0 && preparedSpellIds.length === 0

  const availableCantripIds = new Set(options.cantrips.map((spell) => spell.id))
  const availableSpellIds = new Set(options.rankOneSpells.map((spell) => spell.id))
  return (
    cantripIds.length === 5 &&
    new Set(cantripIds).size === 5 &&
    cantripIds.every((spellId) => availableCantripIds.has(spellId)) &&
    preparedSpellIds.length === 2 &&
    preparedSpellIds.every(
      (spellId): spellId is string => spellId !== null && availableSpellIds.has(spellId),
    )
  )
}
