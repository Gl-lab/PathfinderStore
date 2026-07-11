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

export interface Character {
  id: number
  name: string
  concept: string | null
  age: number | null
  ancestryType: AncestryCode
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
