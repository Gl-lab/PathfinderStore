import { http } from '@/api/http'

export interface WalletLedgerEntry {
  operationId: string
  kind: string
  amountCopper: number
  balanceAfterCopper: number
  description: string
  performedByUserId: number
  occurredAtUtc: string
}

export interface Wallet {
  campaignId: number
  characterId: number
  balanceCopper: number
  reservedCopper: number
  availableCopper: number
  version: number
  entries: WalletLedgerEntry[]
}

export interface Shop {
  id: number
  campaignId: number
  settlementId: number
  name: string
  specialization: string
  shopLevel: number
  catalogPricePercent: number
  buybackPricePercent: number
  pricingPolicyVersion: number
}

export interface Settlement {
  id: number
  campaignId: number
  name: string
  level: number
  region: string
  traits: string
  shops: Shop[]
}

export type ShopOfferKind = 'Catalog' | 'StockInstance'
export type ShopOfferStatus = 'Active' | 'Withdrawn' | 'SoldOut'
export type PurchaseReservationStatus = 'Active' | 'Completed' | 'Cancelled' | 'Expired'

export interface ShopOffer {
  offerKey: string
  campaignId: number
  shopId: number
  kind: ShopOfferKind
  itemConfigurationId: number
  itemInstanceKey: string | null
  itemName: string
  itemLevel: number
  availableQuantity: number
  reservedQuantity: number
  unitPriceCopper: number
  status: ShopOfferStatus
  version: number
}

export interface PurchaseReservation {
  reservationKey: string
  operationId: string
  campaignId: number
  offerKey: string
  buyerCharacterId: number
  itemName: string
  quantity: number
  unitPriceCopper: number
  totalPriceCopper: number
  status: PurchaseReservationStatus
  expiresAtUtc: string
  purchasedItemInstanceKey: string | null
}

export interface SellQuote {
  itemInstanceKey: string
  itemConfigurationId: number
  itemName: string
  quantity: number
  unitPriceCopper: number
  totalPriceCopper: number
}

export async function getWallet(campaignId: number, characterId: number): Promise<Wallet> {
  return (await http.get<Wallet>(`/api/commerce/campaigns/${campaignId}/wallets/${characterId}`))
    .data
}

export async function getSettlements(campaignId: number): Promise<Settlement[]> {
  return (await http.get<Settlement[]>(`/api/commerce/campaigns/${campaignId}/settlements`)).data
}

export async function getShopOffers(campaignId: number, shopId: number): Promise<ShopOffer[]> {
  return (
    await http.get<ShopOffer[]>(`/api/commerce/campaigns/${campaignId}/shops/${shopId}/offers`, {
      params: { status: 'Active' },
    })
  ).data
}

export async function getPurchaseReservations(
  campaignId: number,
  buyerCharacterId: number,
  status?: PurchaseReservationStatus,
): Promise<PurchaseReservation[]> {
  return (
    await http.get<PurchaseReservation[]>(
      `/api/commerce/campaigns/${campaignId}/purchase-reservations`,
      { params: { buyerCharacterId, status } },
    )
  ).data
}

export async function reservePurchase(
  campaignId: number,
  request: {
    operationId: string
    offerKey: string
    buyerCharacterId: number
    quantity: number
  },
): Promise<PurchaseReservation> {
  return (
    await http.post<PurchaseReservation>(
      `/api/commerce/campaigns/${campaignId}/purchase-reservations`,
      request,
    )
  ).data
}

export async function cancelPurchaseReservation(
  campaignId: number,
  reservationKey: string,
  operationId: string,
): Promise<PurchaseReservation> {
  return (
    await http.post<PurchaseReservation>(
      `/api/commerce/campaigns/${campaignId}/purchase-reservations/${reservationKey}/cancel`,
      { operationId },
    )
  ).data
}

export async function completePurchase(
  campaignId: number,
  reservationKey: string,
  operationId: string,
): Promise<PurchaseReservation> {
  return (
    await http.post<PurchaseReservation>(
      `/api/commerce/campaigns/${campaignId}/purchase-reservations/${reservationKey}/complete`,
      { operationId },
    )
  ).data
}

export async function getSellQuote(
  campaignId: number,
  shopId: number,
  sellerCharacterId: number,
  itemInstanceKey: string,
): Promise<SellQuote> {
  return (
    await http.get<SellQuote>(
      `/api/commerce/campaigns/${campaignId}/shops/${shopId}/sell-quote`,
      { params: { sellerCharacterId, itemInstanceKey } },
    )
  ).data
}

export async function sellItem(
  campaignId: number,
  shopId: number,
  request: {
    operationId: string
    sellerCharacterId: number
    itemInstanceKey: string
  },
): Promise<void> {
  await http.post(`/api/commerce/campaigns/${campaignId}/shops/${shopId}/sales`, request)
}
