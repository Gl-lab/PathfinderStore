export function hasRequiredValue(value: unknown): boolean {
  return typeof value === 'string' ? value.trim().length > 0 : Boolean(value)
}

export function passwordsMatch(password: string, passwordRepeat: string): boolean {
  return password === passwordRepeat
}
