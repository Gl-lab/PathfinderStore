import { afterEach, describe, expect, it } from 'vitest'
import { i18n, setLocale } from '@/i18n'
import { localizeBusinessError } from './errors'

describe('business error localization', () => {
  afterEach(() => {
    setLocale('ru')
  })

  it('maps representative backend commerce errors in Russian', () => {
    setLocale('ru')
    expect(localizeBusinessError('Wallet has insufficient available balance.')).toBe(
      'Недостаточно доступных средств.',
    )
    expect(localizeBusinessError('Exchange item changed after reservation.')).toBe(
      'Предмет изменился. Данные обновлены.',
    )
  })

  it('uses the active English locale and preserves unknown messages', () => {
    setLocale('en')
    expect(localizeBusinessError('Stock item is unavailable.')).toBe(
      'The item or offer is no longer available.',
    )
    expect(localizeBusinessError('Custom diagnostic')).toBe('Custom diagnostic')
    expect(i18n.global.locale.value).toBe('en')
  })
})
