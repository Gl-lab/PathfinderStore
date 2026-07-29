import { http } from '@/api/http'

export type ItemCatalogScope = 'Global' | 'Campaign'
export type ItemCatalogScopeFilter = 'All' | 'Global' | 'Campaign'
export type ItemRevisionStatus = 'Draft' | 'Published' | 'Retired'
export type ItemSize = 'Tiny' | 'Small' | 'Medium' | 'Large' | 'Huge' | 'Gargantuan'
export type ItemMaterialType = 'Standard' | 'ColdIron' | 'Silver' | 'Adamantine' | 'Darkwood'
export type ItemMaterialGrade = 'Low' | 'Standard' | 'High'
export type ItemRarity = 'Common' | 'Uncommon' | 'Rare' | 'Unique'
export type ItemCategory =
  | 'Weapon'
  | 'Armor'
  | 'Shield'
  | 'Consumable'
  | 'Ammunition'
  | 'Rune'
  | 'Tool'
  | 'Container'
  | 'OtherEquipment'
export type DamageDieSize = 'D4' | 'D6' | 'D8' | 'D10' | 'D12'
export type ItemDamageType =
  | 'Bludgeoning'
  | 'Piercing'
  | 'Slashing'
  | 'Acid'
  | 'Cold'
  | 'Electricity'
  | 'Fire'
  | 'Force'
  | 'Mental'
  | 'Poison'
  | 'Sonic'
  | 'Spirit'
  | 'Vitality'
  | 'Void'
export type ArmorCategory = 'Unarmored' | 'Light' | 'Medium' | 'Heavy'
export type EquipmentUsage = 'Held' | 'Worn' | 'Installed' | 'Stored'
export type ConsumptionMode = 'DestroyInstance' | 'ReduceStack' | 'ConsumeAmmunition'
export type ChargeRecoveryRule = 'None' | 'DailyPreparations' | 'Manual'
export type PermanentUpgradeKind =
  | 'WeaponPotencyRune'
  | 'StrikingRune'
  | 'ArmorPotencyRune'
  | 'ResilientRune'
  | 'PropertyRune'
  | 'TypedEffect'
export type PermanentUpgradeVisibility = 'Public' | 'Hidden'

export interface AdminItemRevision {
  itemRevisionId: number
  revisionNumber: number
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  primaryCategory: ItemCategory
  rarity: ItemRarity
  status: ItemRevisionStatus
  createdAtUtc: string
  publishedAtUtc: string | null
  retiredAtUtc: string | null
}

export interface AdminItemDefinition {
  itemDefinitionId: number
  key: string
  scope: ItemCatalogScope
  campaignId: number | null
  createdAtUtc: string
  revisions: AdminItemRevision[]
}

export interface AdminItemDefinitionList {
  totalCount: number
  items: AdminItemDefinition[]
}

export interface ItemRulesRequest {
  primaryCategory: ItemCategory
  attacks?: {
    name: string
    damageDieCount: number
    damageDieSize: DamageDieSize
    damageType: ItemDamageType
    hands: number
    rangeIncrementFeet: number | null
  }[]
  armor?: {
    category: ArmorCategory
    armorClassBonus: number
    dexterityCap: number
    checkPenalty: number
    speedPenaltyFeet: number
    strengthRequirement: number
  }
  shield?: { raisedArmorClassBonus: number }
  equipment?: { usage: EquipmentUsage; requiredHands: number }
  consumption?: { mode: ConsumptionMode; quantity: number }
  charges?: {
    maximumCharges: number
    defaultActivationCost: number
    recoveryRule: ChargeRecoveryRule
  }
  durability?: { hardness: number; maximumHitPoints: number; brokenThreshold: number }
  rarity: ItemRarity
}

export interface CreateDraftRequest {
  scope: ItemCatalogScope
  campaignId: number | null
  key: string
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  rules: ItemRulesRequest
}

export interface CreateRevisionDraftRequest {
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  rules: ItemRulesRequest
}

export interface ItemRevisionDto {
  itemDefinitionId: number
  key: string
  scope: ItemCatalogScope
  campaignId: number | null
  revisionNumber: number
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  primaryCategory: ItemCategory
  rarity: ItemRarity
  status: ItemRevisionStatus
  createdAtUtc: string
  publishedAtUtc: string | null
  retiredAtUtc: string | null
}

export interface PermanentUpgradeRequest {
  code: string
  kind: PermanentUpgradeKind
  rank: number
  visibility: PermanentUpgradeVisibility
}

export interface ItemConfigurationDto {
  itemConfigurationId: number
  campaignId: number | null
  itemRevisionId: number
  configurationKey: string
  size: ItemSize
  materialType: ItemMaterialType
  materialGrade: ItemMaterialGrade
  permanentUpgrades: PermanentUpgradeRequest[]
  wasCreated: boolean
}

export async function getAdminDefinitions(params: {
  scope?: ItemCatalogScopeFilter
  campaignId?: number
  status?: ItemRevisionStatus
  search?: string
  skip?: number
  take?: number
}): Promise<AdminItemDefinitionList> {
  return (
    await http.get<AdminItemDefinitionList>('/api/item-catalog-admin/definitions', { params })
  ).data
}

export async function createDraft(request: CreateDraftRequest): Promise<ItemRevisionDto> {
  return (await http.post<ItemRevisionDto>('/api/item-catalog-admin/drafts', request)).data
}

export async function createRevisionDraft(
  itemDefinitionId: number,
  request: CreateRevisionDraftRequest,
): Promise<ItemRevisionDto> {
  return (
    await http.post<ItemRevisionDto>(
      `/api/item-catalog-admin/definitions/${itemDefinitionId}/revisions`,
      request,
    )
  ).data
}

export async function publishRevision(
  itemDefinitionId: number,
  revisionNumber: number,
): Promise<ItemRevisionDto> {
  return (
    await http.post<ItemRevisionDto>(
      `/api/item-catalog-admin/definitions/${itemDefinitionId}/revisions/${revisionNumber}/publish`,
    )
  ).data
}

export async function retireRevision(
  itemDefinitionId: number,
  revisionNumber: number,
): Promise<ItemRevisionDto> {
  return (
    await http.post<ItemRevisionDto>(
      `/api/item-catalog-admin/definitions/${itemDefinitionId}/revisions/${revisionNumber}/retire`,
    )
  ).data
}

export async function createConfiguration(
  campaignId: number,
  request: {
    itemDefinitionId: number
    revisionNumber: number
    size: ItemSize
    materialType: ItemMaterialType
    materialGrade: ItemMaterialGrade
    permanentUpgrades: PermanentUpgradeRequest[]
  },
): Promise<ItemConfigurationDto> {
  return (
    await http.post<ItemConfigurationDto>(
      `/api/item-catalog-admin/campaigns/${campaignId}/configurations`,
      request,
    )
  ).data
}
