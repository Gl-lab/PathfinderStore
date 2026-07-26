import type { ItemCategory } from './api'

const categoryIcons: Record<ItemCategory, string> = {
  Weapon: 'mdi-sword',
  Armor: 'mdi-shield-half-full',
  Shield: 'mdi-shield',
  Consumable: 'mdi-flask-round-bottom',
  Ammunition: 'mdi-arrow-projectile-multiple',
  Rune: 'mdi-auto-fix',
  Tool: 'mdi-tools',
  Container: 'mdi-sack',
  OtherEquipment: 'mdi-sack',
}

export function getItemCategoryIcon(category: ItemCategory): string {
  return categoryIcons[category]
}
