import type { Proficiency } from '@/features/characters/api'
import type {
  CharacterClass,
  Deity,
  DivineFont,
  DivineSanctification,
  Skill,
} from './api'

export function requiresDeitySkillReplacement(
  deity: Deity | null,
  backgroundSkillIds: string[],
): boolean {
  const divineSkillId = deity?.divineSkillId ?? null
  return divineSkillId !== null && backgroundSkillIds.includes(divineSkillId)
}

export function isDeityChoiceComplete(
  characterClass: CharacterClass | null,
  deity: Deity | null,
  divineFont: DivineFont | null,
  sanctification: DivineSanctification | null,
  replacementSkillId: string | null,
  backgroundSkillIds: string[],
  skills: Skill[],
): boolean {
  if (characterClass?.id !== 'class.cleric') {
    return deity === null && divineFont === null && sanctification === null && replacementSkillId === null
  }
  if (!deity?.canGrantClericPowers || !divineFont || !deity.divineFontOptions.includes(divineFont)) return false
  if (deity.requiredSanctification && sanctification !== deity.requiredSanctification) return false
  if (sanctification && !deity.sanctificationOptions.includes(sanctification)) return false

  const requiresReplacement = requiresDeitySkillReplacement(deity, backgroundSkillIds)
  if (!requiresReplacement) return replacementSkillId === null
  return Boolean(
    replacementSkillId &&
      !backgroundSkillIds.includes(replacementSkillId) &&
      skills.some((skill) => skill.id === replacementSkillId),
  )
}

export function getDeityProficiencies(deity: Deity | null): Proficiency[] {
  return deity?.favoredWeapons.map((weapon) => ({
    targetId: `proficiency.attack.${weapon.id}`,
    name: weapon.name,
    category: 'Attack',
    rank: 'Trained',
    sourceGrantId: `${deity.id}.proficiency.${weapon.id}`,
  })) ?? []
}
