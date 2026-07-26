import { describe, expect, it } from 'vitest'
import { formatCountdown, secondsUntil } from './countdown'

describe('secondsUntil', () => {
  it('calculates remaining whole seconds and clamps expired values', () => {
    expect(secondsUntil('2026-01-01T00:00:03.500Z', Date.parse('2026-01-01T00:00:00Z'))).toBe(4)
    expect(secondsUntil('2025-01-01T00:00:00Z', Date.parse('2026-01-01T00:00:00Z'))).toBe(0)
  })
})

describe('formatCountdown', () => {
  it('formats minutes and padded seconds', () => {
    expect(formatCountdown(125)).toBe('2:05')
  })
})
