export interface WizardRequirement {
  complete: boolean
  message: string
}

export function getIncompleteWizardRequirements(
  requirements: WizardRequirement[],
): string[] {
  return requirements.filter((requirement) => !requirement.complete).map((requirement) => requirement.message)
}
