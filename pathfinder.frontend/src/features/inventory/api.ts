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

export interface ExchangeInventory {
  character: InventoryCharacterReference
  items: InventoryOperationItem[]
}

export type PartyStorageAccessPolicy = 'Unconfigured' | 'FreeForMembers' | 'GameMasterOnly'

export interface PartyStorage {
  partyId: number
  accessPolicy: PartyStorageAccessPolicy
  items: PartyStorageItem[]
  recentOperations: PartyStorageOperation[]
}

export interface PartyStorageItem {
  item: InventoryOperationItem
  depositedBy: InventoryCharacterReference | null
  depositedAtUtc: string | null
}

export interface PartyStorageOperation {
  kind:
    | 'GiftProposed'
    | 'GiftAccepted'
    | 'ExchangeProposed'
    | 'ExchangeCompleted'
    | 'ExchangeCancelled'
    | 'PartyStorageDeposited'
    | 'PartyStorageWithdrawn'
    | 'ForcedMove'
    | 'ForcedIssuance'
    | 'ForcedCorrection'
  character: InventoryCharacterReference | null
  item: InventoryOperationItem
  occurredAtUtc: string
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

export async function getPartyExchange(
  campaignId: number,
  exchangeKey: string,
): Promise<PartyExchange> {
  return (
    await http.get<PartyExchange>(`/api/campaigns/${campaignId}/inventory/exchanges/${exchangeKey}`)
  ).data
}

export async function getExchangeInventory(
  campaignId: number,
  participantCharacterId: number,
  ownerCharacterId: number,
): Promise<ExchangeInventory> {
  return (
    await http.get<ExchangeInventory>(
      `/api/campaigns/${campaignId}/inventory/exchange-inventories/${ownerCharacterId}`,
      { params: { participantCharacterId } },
    )
  ).data
}

export async function createPartyExchange(
  campaignId: number,
  request: {
    exchangeKey: string
    initiatorCharacterId: number
    counterpartyCharacterId: number
    lines: {
      fromCharacterId: number
      itemInstanceKey: string
      expectedItemVersion: number
      reservationOperationId: string
    }[]
  },
): Promise<PartyExchange['exchange']> {
  return (
    await http.post<PartyExchange['exchange']>(
      `/api/campaigns/${campaignId}/inventory/exchanges`,
      request,
    )
  ).data
}

async function finalizePartyExchange(
  campaignId: number,
  exchangeKey: string,
  action: 'complete' | 'cancel',
  operationId: string,
): Promise<PartyExchange['exchange']> {
  return (
    await http.post<PartyExchange['exchange']>(
      `/api/campaigns/${campaignId}/inventory/exchanges/${exchangeKey}/${action}`,
      { operationId },
    )
  ).data
}

export async function completePartyExchange(
  campaignId: number,
  exchangeKey: string,
  operationId: string,
): Promise<PartyExchange['exchange']> {
  return finalizePartyExchange(campaignId, exchangeKey, 'complete', operationId)
}

export async function cancelPartyExchange(
  campaignId: number,
  exchangeKey: string,
  operationId: string,
): Promise<PartyExchange['exchange']> {
  return finalizePartyExchange(campaignId, exchangeKey, 'cancel', operationId)
}

export async function getPartyStorage(campaignId: number): Promise<PartyStorage> {
  return (await http.get<PartyStorage>(`/api/campaigns/${campaignId}/inventory/party-storage`)).data
}

async function transferPartyStorage(
  campaignId: number,
  action: 'deposit' | 'withdraw',
  request: {
    characterId: number
    itemInstanceKey: string
    expectedItemVersion: number
    operationId: string
  },
): Promise<{ itemInstanceKey: string; containerKey: string; version: number }> {
  return (
    await http.post<{ itemInstanceKey: string; containerKey: string; version: number }>(
      `/api/campaigns/${campaignId}/inventory/party-storage/${action}`,
      request,
    )
  ).data
}

export async function depositPartyStorage(
  campaignId: number,
  request: {
    characterId: number
    itemInstanceKey: string
    expectedItemVersion: number
    operationId: string
  },
): Promise<{ itemInstanceKey: string; containerKey: string; version: number }> {
  return transferPartyStorage(campaignId, 'deposit', request)
}

export async function withdrawPartyStorage(
  campaignId: number,
  request: {
    characterId: number
    itemInstanceKey: string
    expectedItemVersion: number
    operationId: string
  },
): Promise<{ itemInstanceKey: string; containerKey: string; version: number }> {
  return transferPartyStorage(campaignId, 'withdraw', request)
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
