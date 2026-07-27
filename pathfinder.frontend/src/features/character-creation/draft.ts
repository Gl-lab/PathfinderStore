export const CharacterCreationDraftStorageKey = 'pathfinder.character-creation-draft'

interface CharacterCreationDraft<TForm> {
  version: 1
  step: number
  form: TForm
}

export function serializeCharacterCreationDraft<TForm>(step: number, form: TForm): string {
  return JSON.stringify({ version: 1, step, form } satisfies CharacterCreationDraft<TForm>)
}

export function parseCharacterCreationDraft<TForm>(
  serializedDraft: string | null,
): CharacterCreationDraft<TForm> | null {
  if (!serializedDraft) {
    return null
  }

  try {
    const draft = JSON.parse(serializedDraft) as Partial<CharacterCreationDraft<TForm>>
    if (
      draft.version !== 1 ||
      !Number.isInteger(draft.step) ||
      (draft.step ?? 0) < 1 ||
      (draft.step ?? 0) > 11 ||
      typeof draft.form !== 'object' ||
      draft.form === null ||
      Array.isArray(draft.form)
    ) {
      return null
    }

    return draft as CharacterCreationDraft<TForm>
  } catch {
    return null
  }
}

export function hasCharacterCreationProgress<TForm>(form: TForm, initialForm: TForm): boolean {
  return JSON.stringify(form) !== JSON.stringify(initialForm)
}
