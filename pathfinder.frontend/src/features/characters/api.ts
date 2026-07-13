import { http } from '@/api/http'

export type AncestryCode = 'Gnome' | 'Goblin' | 'Dwarf' | 'Halfling' | 'Human' | 'Elf'
export type AbilityCode =
  | 'Strength'
  | 'Dexterity'
  | 'Constitution'
  | 'Intelligence'
  | 'Wisdom'
  | 'Charisma'

export interface Characteristic {
  value: number
  modifier: number
}

export interface CharacterAncestryPackage {
  selectedHeritageId: string | null
  selectedAncestryFeatId: string | null
  effectiveVision: string
  effectiveBaseHitPoints: number
  startingLanguageIds: string[]
  grantedItems: { itemId: string; quantity: number }[]
  grantedRules: { ruleId: string; effectKind: string; summary: string }[]
  selectedEffects: { effectId: string; effectKind: string; summary: string }[]
  deferredDependencies: string[]
}

export interface CharacterBackgroundPackage {
  backgroundId: string
  name: string
  restrictedBoost: AbilityCode
  freeBoost: AbilityCode
  grants: {
    id: string
    kind: string
    name: string
    summary: string
    requiresChoice: boolean
    allowsCustomLore: boolean
    targetId: string | null
    options: { id: string; name: string }[]
    deferredDependencies: string[]
  }[]
}

export interface CharacterClassPackage {
  classId: string
  name: string
  baseHitPoints: number
  keyAbility: AbilityCode
  rules: {
    id: string
    kind: string
    name: string
    summary: string
    requiresChoice: boolean
    deferredDependencies: string[]
  }[]
}

export interface CharacterHitPoints {
  maximum: number
  ancestry: number
  class: number
  constitutionModifier: number
}

export interface CharacterDerivedStatistics {
  hitPoints: CharacterHitPoints
}

export interface CharacterTraining {
  skills: {
    id: string
    name: string
    keyAbility: AbilityCode | null
    sourceGrantId: string
  }[]
  lore: {
    id: string
    name: string
    keyAbility: AbilityCode
    sourceGrantId: string
  }[]
}

export interface Character {
  id: number
  name: string
  concept: string | null
  age: number | null
  ancestryType: AncestryCode
  ancestryPackage: CharacterAncestryPackage | null
  backgroundPackage: CharacterBackgroundPackage | null
  classPackage: CharacterClassPackage | null
  finalFreeBoosts: AbilityCode[]
  derivedStatistics: CharacterDerivedStatistics | null
  training: CharacterTraining
  characteristics: {
    strength: Characteristic
    dexterity: Characteristic
    constitution: Characteristic
    intelligence: Characteristic
    wisdom: Characteristic
    charisma: Characteristic
  }
}

export async function getCharacters(): Promise<Character[]> {
  return (await http.get<Character[]>('/api/character')).data
}

export async function getCharacter(id: number): Promise<Character> {
  return (await http.get<Character>(`/api/character/${id}`)).data
}

export async function deleteCharacter(id: number): Promise<void> {
  await http.delete(`/api/character/${id}`)
}
