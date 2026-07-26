export function formatBulk(tenths: number): string {
  const normalized = Math.max(0, Math.trunc(tenths))
  return normalized % 10 === 0 ? String(normalized / 10) : (normalized / 10).toFixed(1)
}

export function bulkProgress(totalTenths: number, maximumTenths: number): number {
  if (maximumTenths <= 0) return 0
  return Math.min(100, Math.max(0, (totalTenths / maximumTenths) * 100))
}
