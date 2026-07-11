import { http } from '@/api/http'

export interface Characteristic {
  value: number
  modifier: number
}

export interface Character {
  id: number
  name: string
  concept: string | null
  age: number | null
  ancestryType: number
  characteristics: {
    strength: Characteristic
    dexterity: Characteristic
    constitution: Characteristic
    intelligence: Characteristic
    wisdom: Characteristic
    charisma: Characteristic
  }
}

export const ancestryNames: Record<number, string> = {
  1: 'Гном',
  2: 'Гоблин',
  3: 'Дварф',
  4: 'Полурослик',
  5: 'Человек',
  6: 'Эльф',
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
