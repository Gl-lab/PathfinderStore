import type { LanguageSelectionOptions } from '@/features/character-creation/api'

export function isLanguageSelectionComplete(
  selectedLanguageIds: string[],
  options: LanguageSelectionOptions,
): boolean {
  if (selectedLanguageIds.length !== options.requiredCount) return false
  if (new Set(selectedLanguageIds).size !== selectedLanguageIds.length) return false

  const availableLanguageIds = new Set(options.availableLanguages.map((language) => language.id))
  return selectedLanguageIds.every((languageId) => availableLanguageIds.has(languageId))
}

export function reconcileLanguageSelection(
  selectedLanguageIds: string[],
  options: LanguageSelectionOptions,
): string[] {
  const availableLanguageIds = new Set(options.availableLanguages.map((language) => language.id))
  return Array.from(new Set(selectedLanguageIds))
    .filter((languageId) => availableLanguageIds.has(languageId))
    .slice(0, options.requiredCount)
}
