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
        return item
      }

      if (item && typeof item === 'object' && 'value' in item && typeof item.value === 'string') {
        return item.value
      }

      return i18n.global.t('errors.request')
    })
  }

  if (typeof responseData === 'string') {
    return [responseData]
  }

  if (error.response?.status === 401) {
    return [i18n.global.t('errors.invalidCredentials')]
  }

  return [i18n.global.t('errors.request')]
}
