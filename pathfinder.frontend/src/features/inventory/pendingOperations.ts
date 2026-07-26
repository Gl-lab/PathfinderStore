import type { PartyExchange, PartyGift } from './api'

export function uniquePendingGifts(gifts: PartyGift[]): PartyGift[] {
  return Array.from(new Map(gifts.map((gift) => [gift.gift.giftKey, gift])).values())
}

export function uniquePendingExchanges(exchanges: PartyExchange[]): PartyExchange[] {
  return Array.from(
    new Map(exchanges.map((exchange) => [exchange.exchange.exchangeKey, exchange])).values(),
  )
}
