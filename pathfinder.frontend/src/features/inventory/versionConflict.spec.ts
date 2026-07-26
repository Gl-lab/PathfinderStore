import axios from 'axios'
import { describe, expect, it } from 'vitest'
import { isItemVersionConflict } from './versionConflict'

describe('isItemVersionConflict', () => {
  it('recognizes a backend item version mismatch', () => {
    const error = new axios.AxiosError('Bad request', 'ERR_BAD_REQUEST', undefined, undefined, {
      data: ['Item instance version mismatch. Expected 1, current 2.'],
      status: 400,
      statusText: 'Bad Request',
      headers: {},
      config: { headers: new axios.AxiosHeaders() },
    })

    expect(isItemVersionConflict(error)).toBe(true)
  })

  it('does not classify other business errors as conflicts', () => {
    const error = new axios.AxiosError('Bad request', 'ERR_BAD_REQUEST', undefined, undefined, {
      data: ['Item is transfer restricted.'],
      status: 400,
      statusText: 'Bad Request',
      headers: {},
      config: { headers: new axios.AxiosHeaders() },
    })

    expect(isItemVersionConflict(error)).toBe(false)
  })
})
