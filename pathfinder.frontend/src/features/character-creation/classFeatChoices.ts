import type {
  ArcaneSchool,
  ArcaneThesis,
  CharacterClass,
  FeatChoice,
  FeatDefinition,
} from './api'

export interface ClassFeatChoiceSlot {
  sourceId: string
  name: string
  category: 'Class' | 'Skill'
  requiresSpellshape: boolean
}

export function getRequiredClassFeatChoiceSlots(
  characterClass: CharacterClass | null,
  arcaneSchool: ArcaneSchool | null,
  arcaneThesis: ArcaneThesis | null,
): ClassFeatChoiceSlot[] {
  const slots = (characterClass?.rules ?? [])
    .filter((rule) => rule.kind === 'ClassFeatChoice' || rule.kind === 'SkillFeatChoice')
    .map((rule) => ({
      sourceId: rule.id,
      name: rule.name,
      category: rule.kind === 'SkillFeatChoice' ? 'Skill' as const : 'Class' as const,
      requiresSpellshape: false,
    }))
  const spellshape = arcaneThesis?.effects.find(
    (effect) => effect.kind === 'FirstLevelSpellshapeFeatChoice',
  )
  if (spellshape) {
    slots.push({ sourceId: spellshape.id, name: spellshape.name, category: 'Class', requiresSpellshape: true })
  }
  const extraClassFeat = arcaneSchool?.benefits.find(
    (benefit) => benefit.kind === 'ExtraClassFeat',
  )
  if (extraClassFeat) {
    slots.push({ sourceId: extraClassFeat.id, name: extraClassFeat.name, category: 'Class', requiresSpellshape: false })
  }
  return slots
}

export function getAvailableClassFeatOptions(
  characterClass: CharacterClass | null,
  catalog: FeatDefinition[],
  slot: ClassFeatChoiceSlot,
  choices: FeatChoice[],
): FeatDefinition[] {
  const className = characterClass?.name.toLocaleLowerCase() ?? ''
  const selectedByOtherSlots = new Set(
    choices.filter((choice) => choice.sourceId !== slot.sourceId).map((choice) => choice.featId),
  )
  return catalog.filter(
    (feat) =>
      feat.level === 1 && feat.category === slot.category &&
      (slot.category !== 'Class' || feat.traits.some((trait) => trait.toLocaleLowerCase() === className)) &&
      !feat.deferredDependencies.includes('FeatParameterChoice') &&
      (!slot.requiresSpellshape || feat.id === 'feat.reach_spell' || feat.id === 'feat.widen_spell') &&
      !selectedByOtherSlots.has(feat.id),
  )
}

export function isClassFeatChoiceComplete(
  slots: ClassFeatChoiceSlot[],
  choices: FeatChoice[],
): boolean {
  return slots.length === choices.length &&
    slots.every((slot) => choices.some((choice) => choice.sourceId === slot.sourceId)) &&
    new Set(choices.map((choice) => choice.featId)).size === choices.length
}
