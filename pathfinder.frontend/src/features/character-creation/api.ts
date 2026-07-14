import { http } from '@/api/http'
import type { AbilityCode, AncestryCode, Proficiency } from '@/features/characters/api'

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
  allowsCustomLore: boolean
  targetId: string | null
  options: { id: string; name: string }[]
  deferredDependencies: string[]
}

export interface BackgroundTrainingChoice {
  grantId: string
  targetId: string | null
  customLoreTopic: string | null
}

export interface Skill {
  id: string
  name: string
  keyAbility: AbilityCode
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
  initialProficiencies: Proficiency[]
  initialSkillGrants: ClassSkillGrant[]
  additionalSkillCountBase: number
  spellTradition: SpellTradition | null
  rules: CharacterClassRule[]
  deferredDependencies: string[]
}

export interface ClassSkillGrant {
  id: string
  skillOptions: string[]
}

export interface ClassTrainingTargetChoice {
  skillId: string | null
  customLoreTopic: string | null
}

export interface ClassSkillGrantChoice {
  grantId: string
  selectedSkillId: string | null
  replacementTarget: ClassTrainingTargetChoice | null
}

export interface RogueSkillGrant {
  id: string
  targetId: string | null
  options: string[]
  requiresChoice: boolean
}

export interface RogueRacket {
  id: string
  name: string
  alternativeKeyAbility: AbilityCode | null
  skillGrants: RogueSkillGrant[]
  proficiencyGrants: Proficiency[]
  effects: { id: string; name: string; summary: string }[]
  deferredDependencies: string[]
}

export type HuntersEdgeEffectKind = 'MultipleAttackPenalty' | 'ConditionalBonuses' | 'PrecisionDamage'

export interface HuntersEdge {
  id: string
  name: string
  effects: {
    id: string
    kind: HuntersEdgeEffectKind
    name: string
    summary: string
  }[]
  deferredDependencies: string[]
}

export type DruidicOrderBenefitKind = 'ClassFeat' | 'FocusSpell'

export interface DruidicOrder {
  id: string
  name: string
  skillGrant: ClassSkillGrant
  benefits: {
    id: string
    kind: DruidicOrderBenefitKind
    name: string
    deferredDependencies: string[]
  }[]
}

export type BardMuseBenefitKind = 'ClassFeat' | 'RepertoireSpell'

export interface BardMuse {
  id: string
  name: string
  benefits: {
    id: string
    kind: BardMuseBenefitKind
    name: string
    deferredDependencies: string[]
  }[]
}

export type WitchPatronBenefitKind = 'Lesson' | 'HexCantrip' | 'FamiliarSpell' | 'FamiliarAbility'

export interface WitchPatronBenefit {
  id: string
  kind: WitchPatronBenefitKind
  name: string
  summary: string
  deferredDependencies: string[]
}

export interface WitchPatron {
  id: string
  name: string
  spellTradition: SpellTradition
  skillGrant: ClassSkillGrant
  benefits: WitchPatronBenefit[]
}

export interface RogueTrainingChoice {
  grantId: string
  selectedSkillId: string | null
  replacementSkillId: string | null
}

export interface ClericDoctrineEffect {
  id: string
  name: string
  summary: string
  deferredDependencies: string[]
}

export interface ClericDoctrine {
  id: string
  name: string
  proficiencyGrants: Proficiency[]
  effects: ClericDoctrineEffect[]
  deferredDependencies: string[]
}

export type DivineFont = 'Heal' | 'Harm'
export type DivineSanctification = 'Holy' | 'Unholy'

export interface Deity {
  id: string
  name: string
  canGrantClericPowers: boolean
  divineSkillId: string | null
  favoredWeapons: {
    id: string
    name: string
    category: 'Simple' | 'Martial' | 'Advanced' | 'Unarmed'
  }[]
  divineFontOptions: DivineFont[]
  sanctificationOptions: DivineSanctification[]
  requiredSanctification: DivineSanctification | null
  primaryDomainIds: string[]
  grantedSpells: { rank: number; id: string; name: string }[]
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
  backgroundTrainingChoices: BackgroundTrainingChoice[]
  classId: string
  classKeyAbility: AbilityCode
  rogueRacketId: string | null
  rogueTrainingChoices: RogueTrainingChoice[]
  huntersEdgeId: string | null
  druidicOrderId: string | null
  bardMuseId: string | null
  witchPatronId: string | null
  witchPatronFamiliarSpellId: string | null
  clericDoctrineId: string | null
  deityId: string | null
  divineFont: DivineFont | null
  divineSanctification: DivineSanctification | null
  deitySkillReplacementId: string | null
  finalFreeBoosts: AbilityCode[]
  classSkillGrantChoices: ClassSkillGrantChoice[]
  additionalClassTrainingChoices: ClassTrainingTargetChoice[]
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

export async function getRogueRackets(): Promise<RogueRacket[]> {
  return (await http.get<RogueRacket[]>('/api/classes/rogue/rackets')).data
}

export async function getHuntersEdges(): Promise<HuntersEdge[]> {
  return (await http.get<HuntersEdge[]>('/api/classes/ranger/hunters-edges')).data
}

export async function getDruidicOrders(): Promise<DruidicOrder[]> {
  return (await http.get<DruidicOrder[]>('/api/classes/druid/orders')).data
}

export async function getBardMuses(): Promise<BardMuse[]> {
  return (await http.get<BardMuse[]>('/api/classes/bard/muses')).data
}

export async function getWitchPatrons(): Promise<WitchPatron[]> {
  return (await http.get<WitchPatron[]>('/api/classes/witch/patrons')).data
}

export async function getClericDoctrines(): Promise<ClericDoctrine[]> {
  return (await http.get<ClericDoctrine[]>('/api/classes/cleric/doctrines')).data
}

export async function getDeities(): Promise<Deity[]> {
  return (await http.get<Deity[]>('/api/classes/cleric/deities')).data
}

export async function getSkills(): Promise<Skill[]> {
  return (await http.get<Skill[]>('/api/skills')).data
}

export async function createCharacter(request: CreateCharacterRequest): Promise<void> {
  await http.post('/api/character', request)
}
