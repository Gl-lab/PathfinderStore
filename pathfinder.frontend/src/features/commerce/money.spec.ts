import { describe, expect, it } from 'vitest'
import { combineMoneyParts, splitCopper } from './money'

describe('splitCopper', () => {
  it('omits zero denominations', () => {
    expect(splitCopper(1203)).toEqual([
      { value: 12, unit: 'gold' },
      { value: 3, unit: 'copper' },
    ])
  })

  it('keeps zero copper as an explicit value', () => {
    expect(splitCopper(0)).toEqual([{ value: 0, unit: 'copper' }])
  })

  it('normalizes negative and fractional values', () => {
    expect(splitCopper(-12.5)).toEqual([{ value: 0, unit: 'copper' }])
  })
})

describe('combineMoneyParts', () => {
  it('combines denominations into copper', () => {
    expect(combineMoneyParts(12, 4, 3)).toBe(1243)
  })

  it('normalizes invalid negative parts', () => {
    expect(combineMoneyParts(-1, 2.9, -4)).toBe(20)
  })
})
