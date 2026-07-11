import { http } from '@/api/http'

export interface AncestryBoost {
  abilityType: number | null
  isFree: boolean
}

export interface Ancestry {
  type: number
  name: string
  abilityBoosts: AncestryBoost[]
  abilityFlaws: number[]
  baseHitPoints: number
  size: number
  baseSpeed: number
  darkvision: boolean
  lowLightVision: boolean
}

export interface CreateCharacterRequest {
  name: string
  concept: string | null
  age: number | null
  ancestryType: number
  freeBoosts: number[]
}

export async function getAncestries(): Promise<Ancestry[]> {
  return (await http.get<Ancestry[]>('/api/ancestries')).data
}

export async function createCharacter(request: CreateCharacterRequest): Promise<void> {
  await http.post('/api/character', request)
}
