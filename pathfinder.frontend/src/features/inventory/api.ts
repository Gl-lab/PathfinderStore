import { http } from '@/api/http'

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

export type OperationStatus = 'Pending' | 'Reserved' | 'Completed' | 'Cancelled' | 'Expired'

export interface CharacterInventory {
  campaignId: number
  characterId: number
  isReadOnly: boolean
  containers: {
    containerKey: string
    kind: string
  }[]
  items: CharacterInventoryItem[]
  bulk: CharacterInventoryBulk
}

export interface CharacterInventoryItem {
  itemInstanceKey: string
  version: number
  containerKey: string
  quantity: number
  isEquipped: boolean
  revision: {
    name: string
    revisionNumber: number
    level: number
    primaryCategory: ItemCategory
    priceInCopperPieces: number
    bulkTenths: number
    description: string
  }
  provenance: {
    kind: string
    occurredAtUtc: string
  }
}

export interface CharacterInventoryBulk {
  totalTenths: number
  encumberedAtTenths: number
  maximumTenths: number
}

export interface InventoryOperationItem {
  itemInstanceKey: string
  version: number
  quantity: number
  name: string
  primaryCategory: ItemCategory
  bulkTenths: number
}

export interface InventoryCharacterReference {
  characterId: number
  name: string
}

export interface PartyGift {
  gift: {
    giftKey: string
    campaignId: number
    partyId: number
    sourceCharacterId: number
    destinationCharacterId: number
    itemInstanceKey: string
    expectedItemVersion: number
    status: 'Pending' | 'Accepted'
    createdAtUtc: string
    expiresAtUtc: string
    acceptedAtUtc: string | null
    acceptanceOperationId: string | null
  }
  item: InventoryOperationItem
  sourceCharacter: InventoryCharacterReference
  destinationCharacter: InventoryCharacterReference
}

export interface PartyExchange {
  exchange: {
    exchangeKey: string
    campaignId: number
    partyId: number
    initiatorCharacterId: number
    counterpartyCharacterId: number
    status: 'Pending' | 'Completed' | 'Cancelled'
    createdAtUtc: string
    expiresAtUtc: string
    version: number
    completedAtUtc: string | null
    cancelledAtUtc: string | null
    finalOperationId: string | null
  }
  initiatorCharacter: InventoryCharacterReference
  counterpartyCharacter: InventoryCharacterReference
  items: {
    fromCharacterId: number
    item: InventoryOperationItem
  }[]
}

export async function getCharacterInventory(
  campaignId: number,
  characterId: number,
): Promise<CharacterInventory> {
  return (
    await http.get<CharacterInventory>(
      `/api/campaigns/${campaignId}/inventory/characters/${characterId}`,
    )
  ).data
}

export async function getPartyGifts(
  campaignId: number,
  characterId: number,
  role: 'Incoming' | 'Outgoing',
): Promise<PartyGift[]> {
  return (
    await http.get<PartyGift[]>(`/api/campaigns/${campaignId}/inventory/gifts`, {
      params: { characterId, role, status: 'Pending' },
    })
  ).data
}

export async function getPartyExchanges(
  campaignId: number,
  characterId: number,
): Promise<PartyExchange[]> {
  return (
    await http.get<PartyExchange[]>(`/api/campaigns/${campaignId}/inventory/exchanges`, {
      params: { participantCharacterId: characterId, status: 'Pending' },
    })
  ).data
}

export async function createPartyGift(
  campaignId: number,
  request: {
    giftKey: string
    sourceCharacterId: number
    destinationCharacterId: number
    itemInstanceKey: string
    expectedItemVersion: number
  },
): Promise<PartyGift['gift']> {
  return (
    await http.post<PartyGift['gift']>(`/api/campaigns/${campaignId}/inventory/gifts`, request)
  ).data
}

export async function acceptPartyGift(
  campaignId: number,
  giftKey: string,
  operationId: string,
): Promise<PartyGift['gift']> {
  return (
    await http.post<PartyGift['gift']>(
      `/api/campaigns/${campaignId}/inventory/gifts/${giftKey}/accept`,
      { operationId },
    )
  ).data
}
