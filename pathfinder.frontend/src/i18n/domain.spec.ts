import { afterEach, describe, expect, it } from 'vitest'
import {
  getAbilityLabel,
  getAncestryLabel,
  getBackgroundLabel,
  getCatalogLabel,
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

  it('localizes stable catalog ids and never exposes an unknown raw id', () => {
    setLocale('ru')
    expect(getCatalogLabel('skill.religion', 'Religion')).toBe('Религия')
    expect(getCatalogLabel('domain.cities', 'Cities')).toBe('Города')
    expect(getCatalogLabel('unknown.future_choice', 'unknown.future_choice')).toBe('Future Choice')
  })
})
