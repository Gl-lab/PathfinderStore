import { i18n } from '@/i18n'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'

export function getAncestryLabel(code: AncestryCode): string {
  return i18n.global.t(`domain.ancestry.${code}`)
}

export function getAbilityLabel(code: AbilityCode): string {
  return i18n.global.t(`domain.ability.${code}`)
}
