import { describe, expect, it } from 'vitest'
import type { ArcaneSchool, CharacterClass } from './api'
import { groupArcaneSchoolCurriculum, isArcaneSchoolChoiceComplete } from './arcaneSchool'

const wizard = { id: 'class.wizard' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const school = {
  id: 'arcane_school.battle_magic',
  curriculumSpells: [
    { id: 'spell.shield', name: 'Shield', rank: 0 },
    { id: 'spell.force_barrage', name: 'Force Barrage', rank: 1 },
  ],
} as ArcaneSchool

describe('Arcane School choice', () => {
  it('requires a School only for Wizard', () => {
    expect(isArcaneSchoolChoiceComplete(wizard, null)).toBe(false)
    expect(isArcaneSchoolChoiceComplete(wizard, school)).toBe(true)
    expect(isArcaneSchoolChoiceComplete(fighter, null)).toBe(true)
    expect(isArcaneSchoolChoiceComplete(fighter, school)).toBe(false)
  })

  it('groups curriculum by rank', () => {
    expect(groupArcaneSchoolCurriculum(school)).toEqual([
      { rank: 0, spells: [school.curriculumSpells[0]] },
      { rank: 1, spells: [school.curriculumSpells[1]] },
    ])
  })
})
