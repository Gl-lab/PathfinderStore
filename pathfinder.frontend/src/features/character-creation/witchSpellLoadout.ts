import type { CharacterClass, WitchPatron, WitchSpellOptions } from './api'
import { getWitchPatronFamiliarSpellOptions } from './witchPatron'

export interface WitchSpellSelectionProgress {
  selected: number
  required: number
  isComplete: boolean
}

export interface WitchSpellLoadoutProgress {
  patronReady: boolean
  familiarCantrips: WitchSpellSelectionProgress
  familiarSpells: WitchSpellSelectionProgress
  preparedCantrips: WitchSpellSelectionProgress
  preparedSpellSlots: WitchSpellSelectionProgress
  focusHexSelected: boolean
  isComplete: boolean
}

export function getWitchSpellLoadoutProgress(
  characterClass: CharacterClass | null,
  patron: WitchPatron | null,
  familiarSpellId: string | null,
  familiarCantripIds: string[],
  familiarSpellIds: string[],
  preparedCantripIds: string[],
  preparedSpellIds: (string | null)[],
  focusHexId: string | null,
  options: WitchSpellOptions,
): WitchSpellLoadoutProgress {
  if (characterClass?.id !== 'class.witch') {
    const isComplete =
      familiarCantripIds.length === 0 &&
      familiarSpellIds.length === 0 &&
      preparedCantripIds.length === 0 &&
      preparedSpellIds.length === 0 &&
      focusHexId === null
    return {
      patronReady: true,
      familiarCantrips: { selected: familiarCantripIds.length, required: 0, isComplete },
      familiarSpells: { selected: familiarSpellIds.length, required: 0, isComplete },
      preparedCantrips: { selected: preparedCantripIds.length, required: 0, isComplete },
      preparedSpellSlots: { selected: preparedSpellIds.length, required: 0, isComplete },
      focusHexSelected: focusHexId === null,
      isComplete,
    }
  }

  const patronSpellOptions = getWitchPatronFamiliarSpellOptions(patron)
  const patronSpellId =
    patronSpellOptions.length === 1 ? patronSpellOptions[0]?.id : familiarSpellId
  const patronReady = Boolean(patron && patronSpellId)
  const focusHexSelected = Boolean(
    patron && focusHexId && patron.initialFocusHexOptions.some((spell) => spell.id === focusHexId),
  )
  const availableCantripIds = new Set(options.cantrips.map((spell) => spell.id))
  const availableSpellIds = new Set(options.rankOneSpells.map((spell) => spell.id))
  const knownCantripIds = new Set(familiarCantripIds)
  const knownSpellIds = new Set([...familiarSpellIds, ...(patronSpellId ? [patronSpellId] : [])])
  const familiarCantripsComplete =
    familiarCantripIds.length === 10 &&
    knownCantripIds.size === 10 &&
    familiarCantripIds.every((id) => availableCantripIds.has(id))
  const familiarSpellsComplete =
    familiarSpellIds.length === 5 &&
    new Set(familiarSpellIds).size === 5 &&
    !familiarSpellIds.includes(patronSpellId ?? '') &&
    familiarSpellIds.every((id) => availableSpellIds.has(id))
  const preparedCantripsComplete =
    preparedCantripIds.length === 5 &&
    new Set(preparedCantripIds).size === 5 &&
    preparedCantripIds.every((id) => knownCantripIds.has(id))
  const validPreparedSpellCount = preparedSpellIds.filter(
    (id) => id !== null && knownSpellIds.has(id),
  ).length
  const preparedSpellSlotsComplete = preparedSpellIds.length === 2 && validPreparedSpellCount === 2
  const isComplete =
    patronReady &&
    focusHexSelected &&
    familiarCantripsComplete &&
    familiarSpellsComplete &&
    preparedCantripsComplete &&
    preparedSpellSlotsComplete

  return {
    patronReady,
    familiarCantrips: {
      selected: familiarCantripIds.length,
      required: 10,
      isComplete: familiarCantripsComplete,
    },
    familiarSpells: {
      selected: familiarSpellIds.length,
      required: 5,
      isComplete: familiarSpellsComplete,
    },
    preparedCantrips: {
      selected: preparedCantripIds.length,
      required: 5,
      isComplete: preparedCantripsComplete,
    },
    preparedSpellSlots: {
      selected: validPreparedSpellCount,
      required: 2,
      isComplete: preparedSpellSlotsComplete,
    },
    focusHexSelected,
    isComplete,
  }
}

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
  return getWitchSpellLoadoutProgress(
    characterClass,
    patron,
    familiarSpellId,
    familiarCantripIds,
    familiarSpellIds,
    preparedCantripIds,
    preparedSpellIds,
    focusHexId,
    options,
  ).isComplete
}
