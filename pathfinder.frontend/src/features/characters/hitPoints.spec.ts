import { describe, expect, it } from 'vitest'
import { formatSignedModifier } from '@/features/characters/hitPoints'

describe('hit points presentation', () => {
  it('formats positive and zero modifiers with an explicit plus sign', () => {
    expect(formatSignedModifier(3)).toBe('+3')
    expect(formatSignedModifier(0)).toBe('+0')
  })

  it('preserves the minus sign for negative modifiers', () => {
    expect(formatSignedModifier(-1)).toBe('-1')
  })
})
