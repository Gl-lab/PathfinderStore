export interface MoneyPart {
  value: number
  unit: 'gold' | 'silver' | 'copper'
}

export function splitCopper(copper: number): MoneyPart[] {
  const safeCopper = Math.max(0, Math.trunc(copper))
  const parts: MoneyPart[] = []
  const gold = Math.floor(safeCopper / 100)
  const silver = Math.floor((safeCopper % 100) / 10)
  const remainingCopper = safeCopper % 10

  if (gold > 0) parts.push({ value: gold, unit: 'gold' })
  if (silver > 0) parts.push({ value: silver, unit: 'silver' })
  if (remainingCopper > 0 || parts.length === 0) {
    parts.push({ value: remainingCopper, unit: 'copper' })
  }

  return parts
}

export function combineMoneyParts(gold: number, silver: number, copper: number): number {
  return (
    Math.max(0, Math.trunc(gold)) * 100 +
    Math.max(0, Math.trunc(silver)) * 10 +
    Math.max(0, Math.trunc(copper))
  )
}
