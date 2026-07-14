import type { CharacterClass, WitchPatron } from './api'

export function getWitchPatronFamiliarSpellOptions(patron: WitchPatron | null) {
  return patron?.benefits.filter((benefit) => benefit.kind === 'FamiliarSpell') ?? []
}

export function isWitchPatronChoiceComplete(
  characterClass: CharacterClass | null,
  patron: WitchPatron | null,
  familiarSpellId: string | null,
): boolean {
  if (characterClass?.id !== 'class.witch') return patron === null && familiarSpellId === null
  if (!patron) return false
  const options = getWitchPatronFamiliarSpellOptions(patron)
  return options.length === 1
    ? familiarSpellId === null
    : options.some((option) => option.id === familiarSpellId)
}

export function withWitchPatron(
  characterClass: CharacterClass | null,
  patron: WitchPatron | null,
): CharacterClass | null {
  if (!characterClass || characterClass.id !== 'class.witch' || !patron) return characterClass
  return {
    ...characterClass,
    spellTradition: patron.spellTradition,
    initialSkillGrants: [...characterClass.initialSkillGrants, patron.skillGrant],
  }
}
