import { describe, expect, it } from 'vitest'
import {
  hasCharacterCreationProgress,
  parseCharacterCreationDraft,
  serializeCharacterCreationDraft,
} from './draft'

describe('character creation draft', () => {
  it('round-trips a supported draft', () => {
    const serialized = serializeCharacterCreationDraft(6, { name: 'Valeros', choices: ['fighter'] })

    expect(
      parseCharacterCreationDraft<{ name: string; choices: string[] }>(serialized),
    ).toEqual({
      version: 1,
      step: 6,
      form: { name: 'Valeros', choices: ['fighter'] },
    })
  })

  it('ignores malformed and unsupported drafts', () => {
    expect(parseCharacterCreationDraft('{broken')).toBeNull()
    expect(parseCharacterCreationDraft(JSON.stringify({ version: 2, step: 1, form: {} }))).toBeNull()
    expect(parseCharacterCreationDraft(JSON.stringify({ version: 1, step: 12, form: {} }))).toBeNull()
  })

  it('detects meaningful changes from the initial form', () => {
    const initialForm = { name: '', choices: [] as string[] }

    expect(hasCharacterCreationProgress(initialForm, initialForm)).toBe(false)
    expect(hasCharacterCreationProgress({ name: 'Valeros', choices: [] }, initialForm)).toBe(true)
  })
})
