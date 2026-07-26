import { describe, expect, it } from 'vitest'
import { bulkProgress, formatBulk } from './bulk'

describe('formatBulk', () => {
  it('formats whole and light bulk values', () => {
    expect(formatBulk(20)).toBe('2')
    expect(formatBulk(1)).toBe('0.1')
  })
})

describe('bulkProgress', () => {
  it('clamps progress to the meter range', () => {
    expect(bulkProgress(15, 30)).toBe(50)
    expect(bulkProgress(40, 30)).toBe(100)
    expect(bulkProgress(10, 0)).toBe(0)
  })
})
