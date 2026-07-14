import { describe, expect, it } from 'vitest'
import type { CharacterClass, HuntersEdge } from './api'
import { isHuntersEdgeChoiceComplete } from './huntersEdge'

const ranger = { id: 'class.ranger' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const precision = { id: 'hunters_edge.precision' } as HuntersEdge

describe('Hunter\'s Edge choice', () => {
  it('requires a catalog entry for Ranger', () => {
    expect(isHuntersEdgeChoiceComplete(ranger, precision)).toBe(true)
    expect(isHuntersEdgeChoiceComplete(ranger, null)).toBe(false)
  })

  it('rejects an Edge for a non-Ranger', () => {
    expect(isHuntersEdgeChoiceComplete(fighter, precision)).toBe(false)
    expect(isHuntersEdgeChoiceComplete(fighter, null)).toBe(true)
  })
})
