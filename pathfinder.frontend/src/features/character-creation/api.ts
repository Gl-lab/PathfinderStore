import { http } from '@/api/http'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'

export interface AncestryBoost {
  abilityType: AbilityCode | null
  isFree: boolean
}

export interface Ancestry {
  type: AncestryCode
  name: string
  abilityBoosts: AncestryBoost[]
  abilityFlaws: AbilityCode[]
  baseHitPoints: number
  size: string
  baseSpeed: number
  vision: VisionType
  darkvision: boolean
  lowLightVision: boolean
  startingLanguageIds: string[]
  grantedItems: GrantedItem[]
  grantedRules: GrantedRule[]
  heritages: Heritage[]
  ancestryFeats: AncestryFeat[]
}

export type VisionType = 'None' | 'LowLight' | 'Darkvision'

export interface AncestryEffect {
  effectId: string
  effectKind: string
  summary: string
  visionOverride: VisionType | null
  baseHitPointsOverride: number | null
}

export interface Heritage {
  id: string
  name: string
  rarity: 'Common' | 'Uncommon'
  restrictions: string[]
  incompatibleChoiceIds: string[]
  effects: AncestryEffect[]
  deferredDependencies: string[]
}

export interface AncestryFeat extends Heritage {
  level: number
  prerequisites: string[]
}

export interface GrantedItem {
  itemId: string
  quantity: number
}

export interface GrantedRule {
  ruleId: string
  effectKind: string
  summary: string
}

export type BackgroundGrantKind = 'SkillTraining' | 'LoreTraining' | 'SkillFeat'

export interface BackgroundGrant {
  id: string
  kind: BackgroundGrantKind
  name: string
  summary: string
  requiresChoice: boolean
  options: string[]
  deferredDependencies: string[]
}

export interface Background {
  id: string
  name: string
  restrictedBoostOptions: AbilityCode[]
  freeBoostCount: number
  grants: BackgroundGrant[]
}

export type SpellTradition = 'Arcane' | 'Divine' | 'Occult' | 'Primal'

export interface CharacterClassRule {
  id: string
  kind: string
  name: string
  summary: string
  requiresChoice: boolean
  deferredDependencies: string[]
}

export interface CharacterClass {
  id: string
  name: string
  baseHitPoints: number
  keyAbilityOptions: AbilityCode[]
  spellTradition: SpellTradition | null
  rules: CharacterClassRule[]
  deferredDependencies: string[]
}

export interface CreateCharacterRequest {
  name: string
  concept: string | null
  age: number | null
  ancestryType: AncestryCode
  heritageId: string
  ancestryFeatId: string
  freeBoosts: AbilityCode[]
  backgroundId: string
  backgroundRestrictedBoost: AbilityCode
  backgroundFreeBoost: AbilityCode
  classId: string
  classKeyAbility: AbilityCode
}

export async function getAncestries(): Promise<Ancestry[]> {
  return (await http.get<Ancestry[]>('/api/ancestries')).data
}

export async function getBackgrounds(): Promise<Background[]> {
  return (await http.get<Background[]>('/api/backgrounds')).data
}

export async function getCharacterClasses(): Promise<CharacterClass[]> {
  return (await http.get<CharacterClass[]>('/api/classes')).data
}

export async function createCharacter(request: CreateCharacterRequest): Promise<void> {
  await http.post('/api/character', request)
}
