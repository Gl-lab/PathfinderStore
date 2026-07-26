import axios from 'axios'

function responseMessages(data: unknown): string[] {
  if (typeof data === 'string') return [data]
  if (!Array.isArray(data)) return []

  return data.flatMap((item) => {
    if (typeof item === 'string') return [item]
    if (item && typeof item === 'object' && 'value' in item && typeof item.value === 'string') {
      return [item.value]
    }
    return []
  })
}

export function isItemVersionConflict(error: unknown): boolean {
  if (!axios.isAxiosError(error) || error.response?.status !== 400) return false

  return responseMessages(error.response.data).some((message) =>
    /version (mismatch|has changed)|expected.*version|верси/i.test(message),
  )
}
