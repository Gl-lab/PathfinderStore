import { describe, expect, it } from 'vitest'
import { getIncompleteWizardRequirements } from './wizardRequirements'

describe('getIncompleteWizardRequirements', () => {
  it('returns only incomplete requirements in display order', () => {
    expect(
      getIncompleteWizardRequirements([
        { complete: false, message: 'Choose an ancestry' },
        { complete: true, message: 'Choose a heritage' },
        { complete: false, message: 'Choose an ancestry feat' },
      ]),
    ).toEqual(['Choose an ancestry', 'Choose an ancestry feat'])
  })

  it('returns an empty list when the step is complete', () => {
    expect(
      getIncompleteWizardRequirements([
        { complete: true, message: 'Choose a class' },
        { complete: true, message: 'Choose a key ability' },
      ]),
    ).toEqual([])
  })
})
