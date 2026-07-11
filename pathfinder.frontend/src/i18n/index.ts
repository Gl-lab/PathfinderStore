import { createI18n } from 'vue-i18n'
import { messages } from '@/i18n/messages'

export type SupportedLocale = keyof typeof messages

const localeStorageKey = 'pathfinder.locale'
const persistedLocale = localStorage.getItem(localeStorageKey)
const locale: SupportedLocale = persistedLocale === 'en' ? 'en' : 'ru'

export const i18n = createI18n({ legacy: false, locale, fallbackLocale: 'en', messages })

export function setLocale(locale: SupportedLocale): void {
  i18n.global.locale.value = locale
  localStorage.setItem(localeStorageKey, locale)
}
