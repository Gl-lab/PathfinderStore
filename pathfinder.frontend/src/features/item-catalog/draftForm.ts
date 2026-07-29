import type {
  AdminItemDefinition,
  ArmorCategory,
  ChargeRecoveryRule,
  ConsumptionMode,
  DamageDieSize,
  EquipmentUsage,
  ItemCategory,
  ItemDamageType,
  ItemRarity,
  ItemRulesRequest,
} from './api'

export interface AttackFormModel {
  name: string
  damageDieCount: number
  damageDieSize: DamageDieSize
  damageType: ItemDamageType
  hands: number
  rangeIncrementFeet: number | null
}

export interface DraftFormModel {
  key: string
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  category: ItemCategory
  rarity: ItemRarity
  attacks: AttackFormModel[]
  armor: {
    category: ArmorCategory
    armorClassBonus: number
    dexterityCap: number
    checkPenalty: number
    speedPenaltyFeet: number
    strengthRequirement: number
  }
  shield: { raisedArmorClassBonus: number }
  equipment: { usage: EquipmentUsage; requiredHands: number }
  consumption: { mode: ConsumptionMode; quantity: number }
  chargesEnabled: boolean
  charges: {
    maximumCharges: number
    defaultActivationCost: number
    recoveryRule: ChargeRecoveryRule
  }
  durabilityEnabled: boolean
  durability: { hardness: number; maximumHitPoints: number; brokenThreshold: number }
}

export type RulesSection = 'attacks' | 'armor' | 'shield' | 'equipment' | 'consumption'

const sectionsByCategory: Record<ItemCategory, RulesSection[]> = {
  Weapon: ['attacks', 'equipment'],
  Armor: ['armor', 'equipment'],
  Shield: ['shield', 'attacks', 'equipment'],
  Consumable: ['consumption', 'equipment'],
  Ammunition: ['consumption', 'attacks', 'equipment'],
  Rune: ['equipment'],
  Tool: ['equipment'],
  Container: ['equipment'],
  OtherEquipment: ['equipment'],
}

export function visibleSections(category: ItemCategory): RulesSection[] {
  return sectionsByCategory[category]
}

export function createAttack(): AttackFormModel {
  return {
    name: '',
    damageDieCount: 1,
    damageDieSize: 'D6',
    damageType: 'Slashing',
    hands: 1,
    rangeIncrementFeet: null,
  }
}

export function createDraftFormModel(): DraftFormModel {
  return {
    key: '',
    name: '',
    description: '',
    level: 0,
    priceInCopperPieces: 0,
    bulk: 0,
    category: 'OtherEquipment',
    rarity: 'Common',
    attacks: [createAttack()],
    armor: {
      category: 'Light',
      armorClassBonus: 1,
      dexterityCap: 4,
      checkPenalty: 0,
      speedPenaltyFeet: 0,
      strengthRequirement: 10,
    },
    shield: { raisedArmorClassBonus: 2 },
    equipment: { usage: 'Held', requiredHands: 1 },
    consumption: { mode: 'DestroyInstance', quantity: 1 },
    chargesEnabled: false,
    charges: { maximumCharges: 1, defaultActivationCost: 1, recoveryRule: 'DailyPreparations' },
    durabilityEnabled: false,
    durability: { hardness: 5, maximumHitPoints: 20, brokenThreshold: 10 },
  }
}

export function prefillDraftFromDefinition(definition: AdminItemDefinition): DraftFormModel {
  const latest = [...definition.revisions].sort(
    (left, right) => right.revisionNumber - left.revisionNumber,
  )[0]
  const model = createDraftFormModel()
  model.key = definition.key
  if (latest) {
    model.name = latest.name
    model.description = latest.description
    model.level = latest.level
    model.priceInCopperPieces = latest.priceInCopperPieces
    model.bulk = latest.bulk
    model.category = latest.primaryCategory
    model.rarity = latest.rarity
  }
  return model
}

export function buildRulesRequest(model: DraftFormModel): ItemRulesRequest {
  const sections = visibleSections(model.category)
  const request: ItemRulesRequest = {
    primaryCategory: model.category,
    rarity: model.rarity,
  }
  if (sections.includes('attacks') && model.attacks.length > 0) {
    request.attacks = model.attacks.map((attack) => ({ ...attack }))
  }
  if (sections.includes('armor')) {
    request.armor = { ...model.armor }
  }
  if (sections.includes('shield')) {
    request.shield = { ...model.shield }
  }
  if (sections.includes('equipment')) {
    request.equipment = { ...model.equipment }
  }
  if (sections.includes('consumption')) {
    request.consumption = { ...model.consumption }
  }
  if (model.chargesEnabled) {
    request.charges = { ...model.charges }
  }
  if (model.durabilityEnabled) {
    request.durability = { ...model.durability }
  }
  return request
}

export function draftValidationErrors(model: DraftFormModel, keyEditable: boolean): string[] {
  const errors: string[] = []
  if (keyEditable && !model.key.trim()) {
    errors.push('itemCatalogUi.draft.validation.keyRequired')
  }
  if (!model.name.trim()) {
    errors.push('itemCatalogUi.draft.validation.nameRequired')
  }
  if (model.level < 0) {
    errors.push('itemCatalogUi.draft.validation.levelInvalid')
  }
  if (model.priceInCopperPieces < 0) {
    errors.push('itemCatalogUi.draft.validation.priceInvalid')
  }
  if (model.bulk < 0) {
    errors.push('itemCatalogUi.draft.validation.bulkInvalid')
  }
  const sections = visibleSections(model.category)
  if (model.category === 'Weapon' && model.attacks.length === 0) {
    errors.push('itemCatalogUi.draft.validation.attacksRequired')
  }
  if (model.category === 'Shield' && (model.attacks.length === 0 || !model.durabilityEnabled)) {
    errors.push('itemCatalogUi.draft.validation.shieldComponents')
  }
  const rules = buildRulesRequest(model)
  const hasComponent = Boolean(
    (rules.attacks && rules.attacks.length > 0) ||
    rules.armor ||
    rules.shield ||
    rules.equipment ||
    rules.consumption ||
    rules.charges ||
    rules.durability,
  )
  if (sections.length > 0 && !hasComponent) {
    errors.push('itemCatalogUi.draft.validation.componentRequired')
  }
  return errors
}
