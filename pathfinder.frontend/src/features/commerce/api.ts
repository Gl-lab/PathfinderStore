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

export async function getWallet(campaignId: number, characterId: number): Promise<Wallet> {
  return (await http.get<Wallet>(`/api/commerce/campaigns/${campaignId}/wallets/${characterId}`))
    .data
}

export async function getSettlements(campaignId: number): Promise<Settlement[]> {
  return (await http.get<Settlement[]>(`/api/commerce/campaigns/${campaignId}/settlements`)).data
}
