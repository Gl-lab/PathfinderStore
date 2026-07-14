import type { CharacterClass, DruidicOrder } from './api'

export function isDruidicOrderChoiceComplete(
  characterClass: CharacterClass | null,
  druidicOrder: DruidicOrder | null,
): boolean {
  return characterClass?.id === 'class.druid' ? druidicOrder !== null : druidicOrder === null
}

export function withDruidicOrderSkillGrant(
  characterClass: CharacterClass | null,
  druidicOrder: DruidicOrder | null,
): CharacterClass | null {
  if (!characterClass || characterClass.id !== 'class.druid' || !druidicOrder) return characterClass
  return {
    ...characterClass,
    initialSkillGrants: [...characterClass.initialSkillGrants, druidicOrder.skillGrant],
  }
}
