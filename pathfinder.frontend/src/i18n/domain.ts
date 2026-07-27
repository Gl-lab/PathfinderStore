import { i18n } from '@/i18n'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'
import type { ShopOfferKind } from '@/features/commerce/api'

export function getAncestryLabel(code: AncestryCode): string {
  return i18n.global.t(`domain.ancestry.${code}`)
}

export function getAbilityLabel(code: AbilityCode): string {
  return i18n.global.t(`domain.ability.${code}`)
}

export function getAncestryChoiceLabel(id: string, fallback: string): string {
  const key = `domain.ancestryChoice.${id.replaceAll('.', '_')}`

  return i18n.global.te(key) ? i18n.global.t(key) : fallback
}

export function getBackgroundLabel(id: string, fallback: string): string {
  const key = `domain.background.${id.replace('background.', '')}`

  return i18n.global.te(key) ? i18n.global.t(key) : fallback
}

export function getCharacterClassLabel(id: string, fallback: string): string {
  const key = `classUi.names.${id.replace('class.', '')}`

  return i18n.global.te(key) ? i18n.global.t(key) : fallback
}

export function getCatalogLabel(id: string, fallback: string): string {
  const key = `domain.catalog.${id.replaceAll('.', '_')}`
  if (i18n.global.te(key)) {
    return i18n.global.t(key)
  }

  if (fallback && fallback !== id) {
    return fallback
  }

  const name = id.split('.').at(-1) ?? id
  return name
    .replaceAll('_', ' ')
    .replace(/\b\p{L}/gu, (character) => character.toLocaleUpperCase())
}

export function getVisionLabel(vision: string): string {
  const key = `domain.vision.${vision}`

  return i18n.global.te(key) ? i18n.global.t(key) : vision
}

export function getLanguageLabel(id: string): string {
  const key = `domain.language.${id}`

  return i18n.global.te(key) ? i18n.global.t(key) : getCatalogLabel(id, id)
}

export function getOperationStatusLabel(status: string): string {
  return i18n.global.t(`inventoryUi.operationStatuses.${status}`)
}

export function getShopOfferKindLabel(kind: ShopOfferKind): string {
  return i18n.global.t(`commerceUi.shop.offerKinds.${kind}`)
}
