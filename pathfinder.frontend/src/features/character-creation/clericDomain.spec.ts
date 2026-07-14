import { describe, expect, it } from 'vitest'
import type { CharacterClass, ClericDoctrine, ClericDomain, Deity } from './api'
import { getAvailableClericDomains, isClericDomainChoiceComplete } from './clericDomain'

const cleric = { id: 'class.cleric' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const cloistered = { id: 'cleric_doctrine.cloistered' } as ClericDoctrine
const warpriest = { id: 'cleric_doctrine.warpriest' } as ClericDoctrine
const might = { id: 'domain.might' } as ClericDomain
const truth = { id: 'domain.truth' } as ClericDomain
const iomedae = { primaryDomainIds: ['domain.might'] } as Deity

describe('Cleric domain choice', () => {
  it('offers only primary domains of the selected deity', () => {
    expect(getAvailableClericDomains(iomedae, [might, truth])).toEqual([might])
  })

  it('requires a primary domain for Cloistered Cleric', () => {
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, might)).toBe(true)
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, truth)).toBe(false)
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, null)).toBe(false)
  })

  it('forbids a domain for Warpriest and non-Cleric classes', () => {
    expect(isClericDomainChoiceComplete(cleric, warpriest, iomedae, null)).toBe(true)
    expect(isClericDomainChoiceComplete(cleric, warpriest, iomedae, might)).toBe(false)
    expect(isClericDomainChoiceComplete(fighter, null, null, might)).toBe(false)
  })
})
