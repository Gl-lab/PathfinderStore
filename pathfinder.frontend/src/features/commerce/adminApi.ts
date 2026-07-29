import { http } from '@/api/http'
import type { InventoryOperationItem } from '@/features/inventory/api'
import type { Settlement, Shop, ShopOffer, Wallet } from './api'

export interface AdminWalletSummary {
  campaignId: number
  characterId: number
  characterName: string
  balanceCopper: number
  reservedCopper: number
  availableCopper: number
  version: number
}

export interface AdminInventoryContainer {
  containerKey: string
  ownerKind: 'Character' | 'Party' | 'Shop' | 'World'
  ownerId: number
  ownerName: string | null
  items: InventoryOperationItem[]
}

export interface PublishedItemRevision {
  itemDefinitionId: number
  key: string
  scope: 'Global' | 'Campaign'
  campaignId: number | null
  itemRevisionId: number
  revisionNumber: number
  name: string
  description: string
  level: number
  priceInCopperPieces: number
  bulk: number
  primaryCategory: string
  rarity: string
  configurations: {
    itemConfigurationId: number
    campaignId: number | null
    size: string
    materialType: string
    materialGrade: string
    permanentUpgrades: {
      code: string
      kind: string
      rank: number
      visibility: string
    }[]
  }[]
}

export async function getAdminWallets(campaignId: number): Promise<AdminWalletSummary[]> {
  return (
    await http.get<AdminWalletSummary[]>(`/api/commerce-admin/campaigns/${campaignId}/wallets`)
  ).data
}

export async function getAdminWallet(campaignId: number, characterId: number): Promise<Wallet> {
  return (
    await http.get<Wallet>(`/api/commerce-admin/campaigns/${campaignId}/wallets/${characterId}`)
  ).data
}

export async function getAdminContainers(campaignId: number): Promise<AdminInventoryContainer[]> {
  return (
    await http.get<AdminInventoryContainer[]>(`/api/campaigns/${campaignId}/inventory/containers`)
  ).data
}

export async function searchPublishedItemRevisions(
  campaignId: number,
  search: string,
  scope: 'All' | 'Global' | 'Campaign' = 'All',
): Promise<PublishedItemRevision[]> {
  return (
    await http.get<PublishedItemRevision[]>('/api/item-catalog/revisions', {
      params: { campaignId, search, scope },
    })
  ).data
}

export async function createSettlement(
  campaignId: number,
  request: { operationId: string; name: string; level: number; region: string; traits: string },
): Promise<Settlement> {
  return (
    await http.post<Settlement>(`/api/commerce-admin/campaigns/${campaignId}/settlements`, request)
  ).data
}

export async function createShop(
  campaignId: number,
  settlementId: number,
  request: { operationId: string; name: string; specialization: string; shopLevel: number },
): Promise<Shop> {
  return (
    await http.post<Shop>(
      `/api/commerce-admin/campaigns/${campaignId}/settlements/${settlementId}/shops`,
      request,
    )
  ).data
}

export async function updateShopPricingPolicy(
  campaignId: number,
  shopId: number,
  request: {
    operationId: string
    catalogPricePercent: number
    buybackPricePercent: number
  },
): Promise<Shop> {
  return (
    await http.post<Shop>(
      `/api/commerce-admin/campaigns/${campaignId}/shops/${shopId}/pricing-policy`,
      request,
    )
  ).data
}

export async function createCatalogOffer(
  campaignId: number,
  shopId: number,
  request: { operationId: string; itemConfigurationId: number; quantity: number },
): Promise<ShopOffer> {
  return (
    await http.post<ShopOffer>(
      `/api/commerce-admin/campaigns/${campaignId}/shops/${shopId}/catalog-offers`,
      request,
    )
  ).data
}

export async function createStockOffer(
  campaignId: number,
  shopId: number,
  request: {
    operationId: string
    itemInstanceKey: string
    quantity: number
    unitPriceCopper: number
  },
): Promise<ShopOffer> {
  return (
    await http.post<ShopOffer>(
      `/api/commerce-admin/campaigns/${campaignId}/shops/${shopId}/stock-offers`,
      request,
    )
  ).data
}

export async function adjustWallet(
  campaignId: number,
  characterId: number,
  request: { operationId: string; amountCopper: number; description: string },
): Promise<Wallet> {
  return (
    await http.post<Wallet>(
      `/api/commerce-admin/campaigns/${campaignId}/wallets/${characterId}/adjustments`,
      request,
    )
  ).data
}

export async function forceMoveItem(
  campaignId: number,
  request: {
    itemInstanceKey: string
    destinationContainerKey: string
    expectedItemVersion: number
    operationId: string
    reason: string
  },
): Promise<{ itemInstanceKey: string; containerKey: string; version: number; auditKey: string }> {
  return (
    await http.post<{
      itemInstanceKey: string
      containerKey: string
      version: number
      auditKey: string
    }>(`/api/campaigns/${campaignId}/inventory/force-move`, request)
  ).data
}
