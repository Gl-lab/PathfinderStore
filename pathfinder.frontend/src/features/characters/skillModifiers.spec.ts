import { describe, expect, it } from 'vitest'
import type { CharacterProficiencyStatistic } from '@/features/characters/api'
import { getSkillModifierSections } from '@/features/characters/skillModifiers'

const skill: CharacterProficiencyStatistic = {
  targetId: 'skill.acrobatics',
  name: 'Acrobatics',
  ability: 'Dexterity',
  abilityModifier: 2,
  proficiencyRank: 'Trained',
  proficiencyBonus: 3,
  total: 5,
  sourceGrantIds: ['class.rogue.skill.additional'],
}

describe('skill modifier presentation', () => {
  it('keeps the full general catalog and includes Lore when present', () => {
    const sections = getSkillModifierSections({
      general: [skill],
      lore: [{ ...skill, targetId: 'lore.warfare', name: 'Warfare Lore' }],
    })

    expect(sections.map((section) => section.key)).toEqual(['general', 'lore'])
    expect(sections[0]?.items).toEqual([skill])
  })

  it('omits an empty Lore section without hiding general skills', () => {
    expect(getSkillModifierSections({ general: [skill], lore: [] })).toEqual([
      { key: 'general', items: [skill] },
    ])
  })
})
