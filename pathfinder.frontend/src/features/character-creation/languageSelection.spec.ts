import { describe, expect, it } from 'vitest'
import type { LanguageSelectionOptions } from '@/features/character-creation/api'
import {
  isLanguageSelectionComplete,
  reconcileLanguageSelection,
} from '@/features/character-creation/languageSelection'

const options: LanguageSelectionOptions = {
  requiredCount: 2,
  availableLanguages: [
    { id: 'draconic', name: 'Draconic', speakers: '', rarity: 'Common', category: 'Standard', source: { book: 'Player Core', page: 89 } },
    { id: 'elven', name: 'Elven', speakers: '', rarity: 'Common', category: 'Standard', source: { book: 'Player Core', page: 89 } },
  ],
}

describe('language selection', () => {
  it('requires the server-provided count of unique available languages', () => {
    expect(isLanguageSelectionComplete(['draconic', 'elven'], options)).toBe(true)
    expect(isLanguageSelectionComplete(['draconic'], options)).toBe(false)
    expect(isLanguageSelectionComplete(['draconic', 'draconic'], options)).toBe(false)
    expect(isLanguageSelectionComplete(['draconic', 'dwarven'], options)).toBe(false)
  })

  it('removes stale, duplicate, and excess choices when options change', () => {
    expect(reconcileLanguageSelection(['draconic', 'dwarven', 'draconic', 'elven'], options))
      .toEqual(['draconic', 'elven'])
  })
})
