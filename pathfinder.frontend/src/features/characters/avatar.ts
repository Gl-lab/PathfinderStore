export const unknownAvatarPath = '/avatars/system/unknown.webp'

export function resolveAvatarPath(path: string | null | undefined): string {
  return path?.trim() || unknownAvatarPath
}

export function applyUnknownAvatarFallback(event: Event): void {
  const image = event.currentTarget
  if (!(image instanceof HTMLImageElement)) return
  if (image.getAttribute('src') === unknownAvatarPath) return

  image.src = unknownAvatarPath
}
