import axios from 'axios'
import { i18n } from '@/i18n'

export function getApiErrorMessages(error: unknown): string[] {
  if (!axios.isAxiosError(error)) {
    return [i18n.global.t('errors.connection')]
  }

  const responseData = error.response?.data

  if (Array.isArray(responseData)) {
    return responseData.map((item) => {
      if (typeof item === 'string') {
        return localizeBusinessError(item)
      }

      if (item && typeof item === 'object' && 'value' in item && typeof item.value === 'string') {
        return localizeBusinessError(item.value)
      }

      return i18n.global.t('errors.request')
    })
  }

  if (typeof responseData === 'string') {
    return [localizeBusinessError(responseData)]
  }

  if (error.response?.status === 401) {
    return [i18n.global.t('errors.invalidCredentials')]
  }
  if (error.response?.status === 403) {
    return [i18n.global.t('errors.business.accessDenied')]
  }
  if (error.response?.status === 404) {
    return [i18n.global.t('errors.business.notFound')]
  }
  if (error.response?.status === 409) {
    return [i18n.global.t('errors.business.itemChanged')]
  }

  return [i18n.global.t('errors.request')]
}

export function localizeBusinessError(message: string): string {
  const patterns: { pattern: RegExp; key: string }[] = [
    {
      pattern: /insufficient (funds|available balance)|cannot.*below reserved/i,
      key: 'errors.business.insufficientFunds',
    },
    {
      pattern: /version|changed after reservation|no longer valid/i,
      key: 'errors.business.itemChanged',
    },
    {
      pattern: /unavailable|insufficient quantity|sold out/i,
      key: 'errors.business.unavailable',
    },
    {
      pattern: /only .*game master|access .*denied|must control|unauthorized/i,
      key: 'errors.business.accessDenied',
    },
    {
      pattern: /does not belong|was not found|not found/i,
      key: 'errors.business.notFound',
    },
  ]
  const match = patterns.find((item) => item.pattern.test(message))
  return match ? i18n.global.t(match.key) : message
}
