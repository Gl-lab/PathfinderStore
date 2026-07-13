import { describe, expect, it } from 'vitest'
import type { Proficiency } from '@/features/characters/api'
import {
  formatProficiency,
  groupProficiencies,
} from '@/features/characters/proficiencies'

const proficiencies: Proficiency[] = [
  {
    targetId: 'proficiency.attack.simple_weapons',
    name: 'Simple Weapons',
    category: 'Attack',
    rank: 'Expert',
    sourceGrantId: 'class.fighter.initial_proficiencies',
  },
  {
    targetId: 'proficiency.perception',
    name: 'Perception',
    category: 'Perception',
    rank: 'Expert',
    sourceGrantId: 'class.fighter.initial_proficiencies',
  },
  {
    targetId: 'proficiency.save.fortitude',
    name: 'Fortitude',
    category: 'SavingThrow',
    rank: 'Expert',
    sourceGrantId: 'class.fighter.initial_proficiencies',
  },
]

describe('proficiency presentation', () => {
  it('groups grants in character sheet order', () => {
    expect(groupProficiencies(proficiencies).map((group) => group.category)).toEqual([
      'Perception',
      'SavingThrow',
      'Attack',
    ])
  })

  it('omits empty categories', () => {
    expect(groupProficiencies(proficiencies).some((group) => group.category === 'Defense')).toBe(
      false,
    )
  })

  it('formats target and localized rank', () => {
    expect(formatProficiency(proficiencies[0], (rank) => `rank:${rank}`)).toBe(
      'Simple Weapons — rank:Expert',
    )
  })
})
