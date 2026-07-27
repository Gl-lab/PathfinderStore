import { describe, expect, it } from 'vitest'
import { hasRequiredValue, passwordsMatch } from './validation'

describe('auth form validation', () => {
  it('rejects empty and whitespace-only required values', () => {
    expect(hasRequiredValue('')).toBe(false)
    expect(hasRequiredValue('   ')).toBe(false)
    expect(hasRequiredValue('hero')).toBe(true)
  })

  it('requires the repeated password to match exactly', () => {
    expect(passwordsMatch('secret', 'secret')).toBe(true)
    expect(passwordsMatch('secret', 'Secret')).toBe(false)
  })
})
