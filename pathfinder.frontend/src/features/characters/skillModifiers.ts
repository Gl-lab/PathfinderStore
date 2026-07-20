import type {
  CharacterProficiencyStatistic,
  CharacterSkillModifiers,
} from '@/features/characters/api'

export interface SkillModifierSection {
  key: 'general' | 'lore'
  items: CharacterProficiencyStatistic[]
}

export function getSkillModifierSections(
  modifiers: CharacterSkillModifiers,
): SkillModifierSection[] {
  const sections: SkillModifierSection[] = [
    { key: 'general', items: modifiers.general },
  ]

  if (modifiers.lore.length > 0) {
    sections.push({ key: 'lore', items: modifiers.lore })
  }

  return sections
}
