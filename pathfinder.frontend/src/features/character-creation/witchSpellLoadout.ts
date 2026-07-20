import type { CharacterClass, WitchPatron, WitchSpellOptions } from './api'
import { getWitchPatronFamiliarSpellOptions } from './witchPatron'

export function isWitchSpellLoadoutComplete(
  characterClass: CharacterClass | null,
  patron: WitchPatron | null,
  familiarSpellId: string | null,
  familiarCantripIds: string[],
  familiarSpellIds: string[],
  preparedCantripIds: string[],
  preparedSpellIds: (string | null)[],
  focusHexId: string | null,
  options: WitchSpellOptions,
): boolean {
  if (characterClass?.id !== 'class.witch')
    return (
      familiarCantripIds.length === 0 &&
      familiarSpellIds.length === 0 &&
      preparedCantripIds.length === 0 &&
      preparedSpellIds.length === 0 &&
      focusHexId === null
    )
  if (!patron || !focusHexId) return false

  const patronSpellOptions = getWitchPatronFamiliarSpellOptions(patron)
  const patronSpellId = patronSpellOptions.length === 1
    ? patronSpellOptions[0]?.id
    : familiarSpellId
  if (!patronSpellId || !patron.initialFocusHexOptions.some((spell) => spell.id === focusHexId))
    return false

  const availableCantripIds = new Set(options.cantrips.map((spell) => spell.id))
  const availableSpellIds = new Set(options.rankOneSpells.map((spell) => spell.id))
  const knownCantripIds = new Set(familiarCantripIds)
  const knownSpellIds = new Set([...familiarSpellIds, patronSpellId])
  return (
    familiarCantripIds.length === 10 &&
    knownCantripIds.size === 10 &&
    familiarCantripIds.every((id) => availableCantripIds.has(id)) &&
    familiarSpellIds.length === 5 &&
    new Set(familiarSpellIds).size === 5 &&
    !familiarSpellIds.includes(patronSpellId) &&
    familiarSpellIds.every((id) => availableSpellIds.has(id)) &&
    preparedCantripIds.length === 5 &&
    new Set(preparedCantripIds).size === 5 &&
    preparedCantripIds.every((id) => knownCantripIds.has(id)) &&
    preparedSpellIds.length === 2 &&
    preparedSpellIds.every((id) => id !== null && knownSpellIds.has(id))
  )
}
