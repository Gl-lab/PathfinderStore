import type { ArcaneSchool, CharacterClass, SpellDefinition } from './api'

export interface WizardSpellOptions {
  cantrips: SpellDefinition[]
  rankOneSpells: SpellDefinition[]
}

export interface WizardSpellLoadoutState {
  spellbookCantripIds: string[]
  spellbookSpellIds: string[]
  curriculumCantripId: string | null
  curriculumSpellIds: string[]
  preparedCantripIds: string[]
  preparedSpellIds: (string | null)[]
  preparedCurriculumCantripId: string | null
  preparedCurriculumSpellId: string | null
}

export function reconcileWizardSpellLoadoutForSchool(
  school: ArcaneSchool | null,
  state: WizardSpellLoadoutState,
): WizardSpellLoadoutState {
  const baseSpellCount = school?.hasCurriculum ? 5 : 6
  const spellbookSpellIds = state.spellbookSpellIds.slice(0, baseSpellCount)
  const knownSpellIds = new Set(spellbookSpellIds)
  const knownCantripIds = new Set(state.spellbookCantripIds)
  const curriculumCantripIds = new Set(
    school?.curriculumSpells.filter((spell) => spell.rank === 0).map((spell) => spell.id) ?? [],
  )
  const curriculumSpellIds = new Set(
    school?.curriculumSpells.filter((spell) => spell.rank === 1).map((spell) => spell.id) ?? [],
  )

  return {
    spellbookCantripIds: [...state.spellbookCantripIds],
    spellbookSpellIds,
    curriculumCantripId: state.curriculumCantripId && curriculumCantripIds.has(state.curriculumCantripId)
      ? state.curriculumCantripId
      : null,
    curriculumSpellIds: state.curriculumSpellIds
      .filter((id) => curriculumSpellIds.has(id))
      .slice(0, 2),
    preparedCantripIds: state.preparedCantripIds.filter((id) => knownCantripIds.has(id)),
    preparedSpellIds: state.preparedSpellIds.map((id) => id && knownSpellIds.has(id) ? id : null),
    preparedCurriculumCantripId: state.preparedCurriculumCantripId
      && curriculumCantripIds.has(state.preparedCurriculumCantripId)
      ? state.preparedCurriculumCantripId
      : null,
    preparedCurriculumSpellId: state.preparedCurriculumSpellId
      && curriculumSpellIds.has(state.preparedCurriculumSpellId)
      ? state.preparedCurriculumSpellId
      : null,
  }
}

export function isWizardSpellLoadoutComplete(
  characterClass: CharacterClass | null,
  school: ArcaneSchool | null,
  spellbookCantripIds: string[],
  spellbookSpellIds: string[],
  curriculumCantripId: string | null,
  curriculumSpellIds: string[],
  preparedCantripIds: string[],
  preparedSpellIds: (string | null)[],
  preparedCurriculumCantripId: string | null,
  preparedCurriculumSpellId: string | null,
  options: WizardSpellOptions,
): boolean {
  if (characterClass?.id !== 'class.wizard') {
    return spellbookCantripIds.length === 0
      && spellbookSpellIds.length === 0
      && curriculumCantripId === null
      && curriculumSpellIds.length === 0
      && preparedCantripIds.length === 0
      && preparedSpellIds.length === 0
      && preparedCurriculumCantripId === null
      && preparedCurriculumSpellId === null
  }
  if (!school) return false

  const commonCantripIds = new Set(options.cantrips.map((spell) => spell.id))
  const commonSpellIds = new Set(options.rankOneSpells.map((spell) => spell.id))
  const baseSpellCount = school.hasCurriculum ? 5 : 6
  if (!hasUniqueAllowed(spellbookCantripIds, 10, commonCantripIds)
    || !hasUniqueAllowed(spellbookSpellIds, baseSpellCount, commonSpellIds)
    || !hasUniqueAllowed(preparedCantripIds, 5, new Set(spellbookCantripIds))
    || preparedSpellIds.length !== 2
    || preparedSpellIds.some((id) => id === null || !spellbookSpellIds.includes(id))) return false

  if (!school.hasCurriculum) {
    return curriculumCantripId === null
      && curriculumSpellIds.length === 0
      && preparedCurriculumCantripId === null
      && preparedCurriculumSpellId === null
  }

  const curriculumCantripIds = new Set(
    school.curriculumSpells.filter((spell) => spell.rank === 0).map((spell) => spell.id),
  )
  const curriculumRankOneIds = new Set(
    school.curriculumSpells.filter((spell) => spell.rank === 1).map((spell) => spell.id),
  )
  return curriculumCantripId !== null
    && curriculumCantripIds.has(curriculumCantripId)
    && hasUniqueAllowed(curriculumSpellIds, 2, curriculumRankOneIds)
    && preparedCurriculumCantripId !== null
    && curriculumCantripIds.has(preparedCurriculumCantripId)
    && preparedCurriculumSpellId !== null
    && curriculumRankOneIds.has(preparedCurriculumSpellId)
}

function hasUniqueAllowed(ids: string[], count: number, allowedIds: Set<string>): boolean {
  return ids.length === count
    && new Set(ids).size === count
    && ids.every((id) => allowedIds.has(id))
}
