import { describe, expect, it } from 'vitest'
import type { PartyExchange, PartyGift } from './api'
import type { PurchaseReservation } from '@/features/commerce/api'
import {
  uniquePendingExchanges,
  uniquePendingGifts,
  uniquePendingReservations,
} from './pendingOperations'

describe('pending operation aggregation', () => {
  it('deduplicates gifts by idempotency key', () => {
    const gift = { gift: { giftKey: 'gift-1' } } as PartyGift
    expect(uniquePendingGifts([gift, gift])).toHaveLength(1)
  })

  it('deduplicates exchanges visible to multiple controlled characters', () => {
    const exchange = { exchange: { exchangeKey: 'exchange-1' } } as PartyExchange
    expect(uniquePendingExchanges([exchange, exchange])).toHaveLength(1)
  })

  it('deduplicates active reservations by reservation key', () => {
    const reservation = { reservationKey: 'reservation-1' } as PurchaseReservation
    expect(uniquePendingReservations([reservation, reservation])).toHaveLength(1)
  })
})
