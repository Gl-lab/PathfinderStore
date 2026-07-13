import { describe, expect, it } from 'vitest'
import type { CharacterClass, Deity, Skill } from './api'
import { getDeityProficiencies, isDeityChoiceComplete } from './deity'

const cleric = { id: 'class.cleric' } as CharacterClass
const iomedae: Deity = {
  id: 'deity.iomedae',
  name: 'Iomedae',
  canGrantClericPowers: true,
  divineSkillId: 'skill.intimidation',
  favoredWeapons: [{ id: 'weapon.longsword', name: 'Longsword', category: 'Martial' }],
  divineFontOptions: ['Heal'],
  sanctificationOptions: ['Holy'],
  requiredSanctification: 'Holy',
  primaryDomainIds: ['domain.might'],
  grantedSpells: [],
}
const skills = [
  { id: 'skill.intimidation', name: 'Intimidation', keyAbility: 'Charisma' },
  { id: 'skill.religion', name: 'Religion', keyAbility: 'Wisdom' },
] as Skill[]

describe('deity flow', () => {
  it('requires an eligible deity, font, and required sanctification for Cleric', () => {
    expect(isDeityChoiceComplete(cleric, iomedae, 'Heal', 'Holy', null, [], skills)).toBe(true)
    expect(isDeityChoiceComplete(cleric, iomedae, 'Heal', null, null, [], skills)).toBe(false)
    expect(isDeityChoiceComplete(cleric, { ...iomedae, canGrantClericPowers: false }, 'Heal', 'Holy', null, [], skills)).toBe(false)
  })

  it('requires an unused replacement when the background already trains the divine skill', () => {
    expect(isDeityChoiceComplete(cleric, iomedae, 'Heal', 'Holy', null, ['skill.intimidation'], skills)).toBe(false)
    expect(isDeityChoiceComplete(cleric, iomedae, 'Heal', 'Holy', 'skill.religion', ['skill.intimidation'], skills)).toBe(true)
  })

  it('maps favored weapons to individual trained attack proficiencies', () => {
    expect(getDeityProficiencies(iomedae)[0]).toMatchObject({
      targetId: 'proficiency.attack.weapon.longsword',
      rank: 'Trained',
    })
  })
})
