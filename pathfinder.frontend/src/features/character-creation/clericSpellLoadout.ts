import type { CharacterClass, ClericSpellOptions } from './api'

export function isClericSpellLoadoutComplete(
  characterClass: CharacterClass | null,
  cantripIds: string[],
  preparedSpellIds: (string | null)[],
  options: ClericSpellOptions,
): boolean {
  if (characterClass?.id !== 'class.cleric') {
    return cantripIds.length === 0 && preparedSpellIds.length === 0
  }

  const availableCantripIds = new Set(options.cantrips.map((option) => option.spell.id))
  const availableSpellIds = new Set(options.rankOneSpells.map((option) => option.spell.id))
  return (
    cantripIds.length === 5 &&
    new Set(cantripIds).size === 5 &&
    cantripIds.every((spellId) => availableCantripIds.has(spellId)) &&
    preparedSpellIds.length === 2 &&
    preparedSpellIds.every(
      (spellId) => spellId !== null && availableSpellIds.has(spellId),
    )
  )
}
