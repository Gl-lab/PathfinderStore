import type { PartyStorageAccessPolicy } from './api'

export function canWithdrawFromStorage(
  policy: PartyStorageAccessPolicy,
  isGameMaster: boolean,
): boolean {
  return policy !== 'GameMasterOnly' || isGameMaster
}
