export function formatSignedModifier(value: number): string {
  return value >= 0 ? `+${value}` : String(value)
}
