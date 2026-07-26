export function secondsUntil(expiresAtUtc: string, now: number = Date.now()): number {
  const expiration = Date.parse(expiresAtUtc)
  if (!Number.isFinite(expiration)) return 0
  return Math.max(0, Math.ceil((expiration - now) / 1000))
}

export function formatCountdown(seconds: number): string {
  const safeSeconds = Math.max(0, Math.trunc(seconds))
  const minutes = Math.floor(safeSeconds / 60)
  const remainder = safeSeconds % 60
  return `${minutes}:${String(remainder).padStart(2, '0')}`
}
