import axios from 'axios'

export function getApiErrorMessages(error: unknown): string[] {
  if (!axios.isAxiosError(error)) {
    return ['Не удалось подключиться к серверу. Повторите попытку позже.']
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

      return 'Не удалось выполнить запрос.'
    })
  }

  if (typeof responseData === 'string') {
    return [responseData]
  }

  if (error.response?.status === 401) {
    return ['Неверный логин или пароль.']
  }

  return ['Не удалось выполнить запрос. Повторите попытку позже.']
}
