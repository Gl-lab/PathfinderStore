import { http } from '@/api/http'

export type AncestryCode = 'Gnome' | 'Goblin' | 'Dwarf' | 'Halfling' | 'Human' | 'Elf'
export type CharacterGender = 'NotSpecified' | 'Male' | 'Female'
export type AbilityCode =
  | 'Strength'
  | 'Dexterity'
  | 'Constitution'
  | 'Intelligence'
  | 'Wisdom'
  | 'Charisma'

export type ProficiencyRank = 'Untrained' | 'Trained' | 'Expert' | 'Master' | 'Legendary'
export type ProficiencyCategory =
  | 'Perception'
  | 'SavingThrow'
  | 'Attack'
  | 'Defense'
  | 'ClassDc'
export type SpellTradition = 'Arcane' | 'Divine' | 'Occult' | 'Primal'
export type FeatCategory = 'Ancestry' | 'Skill' | 'Class'
export type CharacterFeatAcquisitionType = 'Selected' | 'Granted'
export type CharacterFeatSourceType = 'Ancestry' | 'Background' | 'Class' | 'ClassChoice'

export interface CharacterFeat {
  id: string
  name: string
  category: FeatCategory
  level: number
  traits: string[]
  prerequisites: string[]
  summary: string
  deferredDependencies: string[]
  source: { book: string; page: number }
  acquisitionType: CharacterFeatAcquisitionType
  sourceType: CharacterFeatSourceType
  sourceId: string
}

export interface Proficiency {
  targetId: string
  name: string
  category: ProficiencyCategory
  rank: ProficiencyRank
  sourceGrantId: string
  sourceGrantIds?: string[]
}

export interface Characteristic {
  value: number
  modifier: number
}

export interface CharacterAncestryPackage {
  selectedHeritageId: string | null
  selectedAncestryFeatId: string | null
  effectiveVision: string
  effectiveBaseHitPoints: number
  startingLanguageIds: string[]
  grantedItems: { itemId: string; quantity: number }[]
  grantedRules: { ruleId: string; effectKind: string; summary: string }[]
  selectedEffects: { effectId: string; effectKind: string; summary: string }[]
  deferredDependencies: string[]
}

export interface CharacterBackgroundPackage {
  backgroundId: string
  name: string
  restrictedBoost: AbilityCode
  freeBoost: AbilityCode
  grants: {
    id: string
    kind: string
    name: string
    summary: string
    requiresChoice: boolean
    allowsCustomLore: boolean
    targetId: string | null
    options: { id: string; name: string }[]
    deferredDependencies: string[]
  }[]
}

export interface CharacterClassPackage {
  classId: string
  name: string
  baseHitPoints: number
  keyAbility: AbilityCode
  additionalSkillCount: number
  spellTradition: SpellTradition | null
  rogueRacket: {
    id: string
    name: string
    alternativeKeyAbility: AbilityCode | null
    effects: { id: string; name: string; summary: string }[]
  } | null
  huntersEdge: {
    id: string
    name: string
    effects: { id: string; kind: string; name: string; summary: string }[]
  } | null
  druidicOrder: {
    id: string
    name: string
    skillGrant: { id: string; skillOptions: string[] }
    benefits: {
      id: string
      kind: 'ClassFeat' | 'FocusSpell'
      name: string
      deferredDependencies: string[]
    }[]
  } | null
  bardMuse: {
    id: string
    name: string
    benefits: {
      id: string
      kind: 'ClassFeat' | 'RepertoireSpell'
      name: string
      deferredDependencies: string[]
    }[]
  } | null
  bardSpellLoadout: {
    cantrips: { id: string; name: string; rank: number; kind: 'Cantrip' }[]
    rankOneRepertoire: {
      spell: { id: string; name: string; rank: number; kind: 'Spell' }
      source: 'Selected' | 'MuseGranted'
      sourceGrantId: string
    }[]
    rankOneSpellSlotCount: number
  } | null
  bardComposition: {
    maximumFocusPoints: number
    compositionCantrip: { id: string; name: string; rank: number; kind: 'Cantrip' }
    focusSpell: { id: string; name: string; rank: number; kind: 'Focus' }
    sourceGrantId: string
  } | null
  druidSpellLoadout: {
    cantrips: { id: string; name: string; rank: number; kind: 'Cantrip' }[]
    preparedSpells: { id: string; name: string; rank: number; kind: 'Spell' }[]
    rankOneSpellSlotCount: number
  } | null
  druidFocusPool: {
    maximumFocusPoints: number
    focusSpell: { id: string; name: string; rank: number; kind: 'Focus' }
    sourceGrantId: string
  } | null
  witchPatron: {
    id: string
    name: string
    spellTradition: SpellTradition
    skillGrant: { id: string; skillOptions: string[] }
    benefits: {
      id: string
      kind: 'Lesson' | 'HexCantrip' | 'FamiliarSpell' | 'FamiliarAbility'
      name: string
      summary: string
      deferredDependencies: string[]
    }[]
    selectedFamiliarSpell: {
      id: string
      kind: 'FamiliarSpell'
      name: string
      summary: string
      deferredDependencies: string[]
    }
  } | null
  witchSpellLoadout: {
    familiarCantrips: { id: string; name: string }[]
    familiarRankOneSpells: { id: string; name: string }[]
    patronGrantedSpell: { id: string; name: string } | null
    preparedCantrips: { id: string; name: string }[]
    preparedSpells: { id: string; name: string }[]
    rankOneSpellSlotCount: number
  } | null
  witchHexPackage: {
    maximumFocusPoints: number
    patronHexCantrip: { id: string; name: string } | null
    focusHex: { id: string; name: string } | null
    sourceGrantId: string
  } | null
  arcaneSchool: {
    id: string
    name: string
    hasCurriculum: boolean
    curriculumSpells: {
      id: string
      name: string
      rank: number
      isUncommon: boolean
    }[]
    benefits: {
      id: string
      kind: string
      name: string
      summary: string
      deferredDependencies: string[]
    }[]
  } | null
  wizardSpellLoadout: {
    spellbookCantrips: { id: string; name: string }[]
    spellbookRankOneSpells: { id: string; name: string }[]
    curriculumCantrip: { id: string; name: string } | null
    curriculumRankOneSpells: { id: string; name: string }[]
    preparedCantrips: { id: string; name: string }[]
    preparedRankOneSpells: { id: string; name: string }[]
    preparedCurriculumCantrip: { id: string; name: string } | null
    preparedCurriculumRankOneSpell: { id: string; name: string } | null
    baseRankOneSpellSlotCount: number
    curriculumRankOneSpellSlotCount: number
  } | null
  wizardSchoolMagic: {
    maximumFocusPoints: number
    drainBondedItemUsesPerDay: number
    initialSchoolSpell: { id: string; name: string } | null
    sourceGrantId: string
  } | null
  arcaneThesis: {
    id: string
    name: string
    effects: {
      id: string
      kind: string
      name: string
      summary: string
      milestoneLevels: number[]
      deferredDependencies: string[]
    }[]
  } | null
  clericDoctrine: {
    id: string
    name: string
    effects: {
      id: string
      name: string
      summary: string
      deferredDependencies: string[]
    }[]
    deferredDependencies: string[]
  } | null
  deity: {
    id: string
    name: string
    divineSkillId: string
    divineSkillReplacementId: string | null
    favoredWeapons: { id: string; name: string; category: string }[]
    divineFont: 'Heal' | 'Harm'
    sanctification: 'Holy' | 'Unholy' | null
    primaryDomainIds: string[]
    grantedSpells: { rank: number; id: string; name: string }[]
  } | null
  clericDomain: {
    id: string
    name: string
    initialFocusSpell: {
      id: string
      name: string
      rank: number
      kind: 'Focus'
    }
  } | null
  clericSpellLoadout: {
    cantrips: { id: string; name: string; rank: number; kind: 'Cantrip' }[]
    preparedSpells: {
      spell: { id: string; name: string; rank: number; kind: 'Spell' }
      accessSources: ('DivineTradition' | 'DeityGranted')[]
    }[]
    divineFontSpells: { id: string; name: string; rank: number; kind: 'Spell' }[]
  } | null
  clericFocusPool: {
    maximumFocusPoints: number
    focusSpell: { id: string; name: string; rank: number; kind: 'Focus' }
    sourceGrantId: string
  } | null
  rules: {
    id: string
    kind: string
    name: string
    summary: string
    requiresChoice: boolean
    deferredDependencies: string[]
  }[]
}

export interface CharacterHitPoints {
  maximum: number
  ancestry: number
  class: number
  constitutionModifier: number
}

export interface CharacterDerivedStatistics {
  hitPoints: CharacterHitPoints
  perception: CharacterProficiencyStatistic
  savingThrows: {
    fortitude: CharacterProficiencyStatistic
    reflex: CharacterProficiencyStatistic
    will: CharacterProficiencyStatistic
  }
  skillModifiers: CharacterSkillModifiers
}

export interface CharacterProficiencyStatistic {
  targetId: string
  name: string
  ability: AbilityCode
  abilityModifier: number
  proficiencyRank: ProficiencyRank
  proficiencyBonus: number
  total: number
  sourceGrantIds: string[]
}

export interface CharacterSkillModifiers {
  general: CharacterProficiencyStatistic[]
  lore: CharacterProficiencyStatistic[]
}

export interface CharacterTraining {
  skills: {
    id: string
    name: string
    keyAbility: AbilityCode | null
    sourceGrantId: string
  }[]
  lore: {
    id: string
    name: string
    keyAbility: AbilityCode
    sourceGrantId: string
  }[]
  deferredFeatGrants: {
    featId: string
    targetId: string
    reason: 'ReplacementChoiceRequired'
  }[]
}

export interface Character {
  id: number
  name: string
  concept: string | null
  age: number | null
  gender: CharacterGender
  avatarId: string
  avatarPath: string
  ancestryType: AncestryCode
  ancestryPackage: CharacterAncestryPackage | null
  backgroundPackage: CharacterBackgroundPackage | null
  classPackage: CharacterClassPackage | null
  finalFreeBoosts: AbilityCode[]
  derivedStatistics: CharacterDerivedStatistics | null
  training: CharacterTraining
  proficiencies: Proficiency[]
  feats: CharacterFeat[]
  characteristics: {
    strength: Characteristic
    dexterity: Characteristic
    constitution: Characteristic
    intelligence: Characteristic
    wisdom: Characteristic
    charisma: Characteristic
  }
}

export async function getCharacters(): Promise<Character[]> {
  return (await http.get<Character[]>('/api/character')).data
}

export async function getCharacter(id: number): Promise<Character> {
  return (await http.get<Character>(`/api/character/${id}`)).data
}

export async function setCharacterGender(
  id: number,
  gender: Exclude<CharacterGender, 'NotSpecified'>,
): Promise<void> {
  await http.put(`/api/character/${id}/gender`, { gender })
}

export async function deleteCharacter(id: number): Promise<void> {
  await http.delete(`/api/character/${id}`)
}
