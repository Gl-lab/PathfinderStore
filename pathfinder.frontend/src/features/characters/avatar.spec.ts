import { describe, expect, it } from 'vitest'
import {
  applyUnknownAvatarFallback,
  resolveAvatarPath,
  unknownAvatarPath,
} from '@/features/characters/avatar'

describe('character avatar', () => {
  it('uses the stored avatar path', () => {
    expect(resolveAvatarPath('/avatars/human/fighter/male-01.webp')).toBe(
      '/avatars/human/fighter/male-01.webp',
    )
  })

  it('uses the unknown asset when the read model has no path', () => {
    expect(resolveAvatarPath(null)).toBe(unknownAvatarPath)
    expect(resolveAvatarPath('  ')).toBe(unknownAvatarPath)
  })

  it('replaces a failed image with the unknown asset without a fallback loop', () => {
    const image = document.createElement('img')
    image.src = '/avatars/missing.webp'

    const event = new Event('error')
    Object.defineProperty(event, 'currentTarget', { value: image })
    applyUnknownAvatarFallback(event)
    expect(image.getAttribute('src')).toBe(unknownAvatarPath)

    applyUnknownAvatarFallback(event)
    expect(image.getAttribute('src')).toBe(unknownAvatarPath)
  })
})
