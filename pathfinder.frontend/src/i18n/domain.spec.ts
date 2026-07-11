import { afterEach, describe, expect, it } from 'vitest'
import { getAbilityLabel, getAncestryLabel } from '@/i18n/domain'
import { setLocale } from '@/i18n'

afterEach(() => {
  setLocale('ru')
})

describe('domain localization helpers', () => {
  it('localizes stable ancestry codes in Russian', () => {
    setLocale('ru')

    expect(getAncestryLabel('Human')).toBe('Человек')
  })

  it('localizes stable ability codes in English', () => {
    setLocale('en')

    expect(getAbilityLabel('Strength')).toBe('Strength')
  })
})
