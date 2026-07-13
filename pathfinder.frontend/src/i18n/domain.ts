import { i18n } from '@/i18n'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'

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

export function getVisionLabel(vision: string): string {
  const key = `domain.vision.${vision}`

  return i18n.global.te(key) ? i18n.global.t(key) : vision
}

export function getLanguageLabel(id: string): string {
  const key = `domain.language.${id}`

  return i18n.global.te(key) ? i18n.global.t(key) : id
}
