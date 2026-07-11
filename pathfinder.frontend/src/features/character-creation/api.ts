import { http } from '@/api/http'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'

export interface AncestryBoost {
  abilityType: AbilityCode | null
  isFree: boolean
}

export interface Ancestry {
  type: AncestryCode
  abilityBoosts: AncestryBoost[]
  abilityFlaws: AbilityCode[]
  baseHitPoints: number
  size: string
  baseSpeed: number
  darkvision: boolean
  lowLightVision: boolean
}

export interface CreateCharacterRequest {
  name: string
  concept: string | null
  age: number | null
  ancestryType: AncestryCode
  freeBoosts: AbilityCode[]
}

export async function getAncestries(): Promise<Ancestry[]> {
  return (await http.get<Ancestry[]>('/api/ancestries')).data
}

export async function createCharacter(request: CreateCharacterRequest): Promise<void> {
  await http.post('/api/character', request)
}
