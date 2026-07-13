import { afterEach, describe, expect, it } from 'vitest'
import {
  getAbilityLabel,
  getAncestryLabel,
  getBackgroundLabel,
  getCharacterClassLabel,
} from '@/i18n/domain'
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

  it('localizes a background id and preserves an English fallback', () => {
    setLocale('ru')
    expect(getBackgroundLabel('background.acrobat', 'Acrobat')).toBe('Акробат')

    setLocale('en')
    expect(getBackgroundLabel('background.acrobat', 'Acrobat')).toBe('Acrobat')
  })

  it('localizes a class id and preserves an English fallback', () => {
    setLocale('ru')
    expect(getCharacterClassLabel('class.fighter', 'Fighter')).toBe('Воин')

    setLocale('en')
    expect(getCharacterClassLabel('class.fighter', 'Fighter')).toBe('Fighter')
  })
})
