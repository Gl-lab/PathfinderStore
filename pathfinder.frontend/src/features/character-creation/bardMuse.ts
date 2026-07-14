import type { BardMuse, CharacterClass } from './api'

export function isBardMuseChoiceComplete(
  characterClass: CharacterClass | null,
  bardMuse: BardMuse | null,
): boolean {
  return characterClass?.id === 'class.bard' ? bardMuse !== null : bardMuse === null
}
