import type { SpellDefinition, SpellKind } from './api'

export type ClericSpellCatalogGroups = Record<SpellKind, SpellDefinition[]>

export function groupClericSpellCatalog(
  spells: SpellDefinition[],
): ClericSpellCatalogGroups {
  return {
    Cantrip: spells.filter((spell) => spell.kind === 'Cantrip'),
    Spell: spells.filter((spell) => spell.kind === 'Spell'),
    Focus: spells.filter((spell) => spell.kind === 'Focus'),
  }
}
