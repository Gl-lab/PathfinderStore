import type { BardMuse, BardSpellOptions, CharacterClass } from './api'

export function getMuseGrantedSpellId(muse: BardMuse | null): string | null {
  return muse?.benefits.find((benefit) => benefit.kind === 'RepertoireSpell')?.id ?? null
}

export function isBardSpellLoadoutComplete(
  characterClass: CharacterClass | null,
  muse: BardMuse | null,
  cantripIds: string[],
  spellIds: string[],
  options: BardSpellOptions,
): boolean {
  if (characterClass?.id !== 'class.bard') {
    return cantripIds.length === 0 && spellIds.length === 0
  }

  const availableCantripIds = new Set(options.cantrips.map((spell) => spell.id))
  const availableSpellIds = new Set(options.rankOneSpells.map((spell) => spell.id))
  const museGrantedSpellId = getMuseGrantedSpellId(muse)
  return cantripIds.length === 5
    && new Set(cantripIds).size === 5
    && cantripIds.every((spellId) => availableCantripIds.has(spellId))
    && spellIds.length === 2
    && new Set(spellIds).size === 2
    && spellIds.every((spellId) => availableSpellIds.has(spellId))
    && museGrantedSpellId !== null
    && !spellIds.includes(museGrantedSpellId)
}
