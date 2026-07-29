import { describe, expect, it } from 'vitest'
import type { AdminItemDefinition, AdminItemRevision } from './api'
import {
  canConfigure,
  canManageDefinition,
  canPublish,
  canRetire,
  latestDraft,
  newRevisionBlockReason,
  publishConsequence,
} from './lifecycle'

function revision(overrides: Partial<AdminItemRevision>): AdminItemRevision {
  return {
    itemRevisionId: 1,
    revisionNumber: 1,
    name: 'Longsword',
    description: 'Versatile blade.',
    level: 0,
    priceInCopperPieces: 100,
    bulk: 1,
    primaryCategory: 'Weapon',
    rarity: 'Common',
    status: 'Draft',
    createdAtUtc: '2026-07-27T10:00:00Z',
    publishedAtUtc: null,
    retiredAtUtc: null,
    ...overrides,
  }
}

function definition(overrides: Partial<AdminItemDefinition>): AdminItemDefinition {
  return {
    itemDefinitionId: 1,
    key: 'equipment.longsword',
    scope: 'Campaign',
    campaignId: 42,
    createdAtUtc: '2026-07-27T10:00:00Z',
    revisions: [],
    ...overrides,
  }
}

describe('lifecycle', () => {
  it('allows publish only for drafts', () => {
    expect(canPublish(revision({ status: 'Draft' }))).toBe(true)
    expect(canPublish(revision({ status: 'Published' }))).toBe(false)
    expect(canPublish(revision({ status: 'Retired' }))).toBe(false)
  })

  it('allows retire only for published revisions', () => {
    expect(canRetire(revision({ status: 'Published' }))).toBe(true)
    expect(canRetire(revision({ status: 'Draft' }))).toBe(false)
    expect(canRetire(revision({ status: 'Retired' }))).toBe(false)
  })

  it('limits management to matching scope per mode', () => {
    expect(canManageDefinition(definition({ scope: 'Campaign' }), 'campaign')).toBe(true)
    expect(canManageDefinition(definition({ scope: 'Global' }), 'campaign')).toBe(false)
    expect(canManageDefinition(definition({ scope: 'Global' }), 'global')).toBe(true)
    expect(canManageDefinition(definition({ scope: 'Campaign' }), 'global')).toBe(false)
  })

  it('returns the published sibling as publish consequence', () => {
    const published = revision({ revisionNumber: 1, status: 'Published' })
    const target = definition({ revisions: [published, revision({ revisionNumber: 2 })] })

    expect(publishConsequence(target)).toBe(published)
    expect(publishConsequence(definition({ revisions: [revision({})] }))).toBeNull()
  })

  it('blocks a new revision when a draft already exists or scope is foreign', () => {
    const withDraft = definition({ revisions: [revision({ status: 'Draft' })] })
    const withoutDraft = definition({ revisions: [revision({ status: 'Published' })] })

    expect(newRevisionBlockReason(withDraft, 'campaign')).toBe('draftExists')
    expect(newRevisionBlockReason(withoutDraft, 'campaign')).toBeNull()
    expect(latestDraft(withDraft)?.revisionNumber).toBe(1)
    expect(newRevisionBlockReason(definition({ scope: 'Global', revisions: [] }), 'campaign')).toBe(
      'readOnly',
    )
  })

  it('allows configuration only for published revisions in campaign mode', () => {
    const published = revision({ status: 'Published' })

    expect(canConfigure(published, 'campaign')).toBe(true)
    expect(canConfigure(revision({ status: 'Draft' }), 'campaign')).toBe(false)
    expect(canConfigure(published, 'global')).toBe(false)
  })
})
