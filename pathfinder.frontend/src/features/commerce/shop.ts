import type { ShopOffer, Wallet } from './api'

export function availableOfferQuantity(offer: ShopOffer): number {
  return Math.max(0, offer.availableQuantity - offer.reservedQuantity)
}

export function maxAffordableQuantity(offer: ShopOffer, wallet: Wallet | null): number {
  if (!wallet || offer.unitPriceCopper <= 0) return availableOfferQuantity(offer)

  return Math.min(
    availableOfferQuantity(offer),
    Math.floor(wallet.availableCopper / offer.unitPriceCopper),
  )
}

export function purchaseShortfall(
  offer: ShopOffer,
  quantity: number,
  wallet: Wallet | null,
): number {
  if (!wallet) return 0

  return Math.max(0, offer.unitPriceCopper * quantity - wallet.availableCopper)
}
