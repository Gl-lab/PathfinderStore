import type { PartyExchange, PartyGift } from './api'
import type { PurchaseReservation } from '@/features/commerce/api'

export function uniquePendingGifts(gifts: PartyGift[]): PartyGift[] {
  return Array.from(new Map(gifts.map((gift) => [gift.gift.giftKey, gift])).values())
}

export function uniquePendingExchanges(exchanges: PartyExchange[]): PartyExchange[] {
  return Array.from(
    new Map(exchanges.map((exchange) => [exchange.exchange.exchangeKey, exchange])).values(),
  )
}

export function uniquePendingReservations(
  reservations: PurchaseReservation[],
): PurchaseReservation[] {
  return Array.from(
    new Map(reservations.map((reservation) => [reservation.reservationKey, reservation])).values(),
  )
}
