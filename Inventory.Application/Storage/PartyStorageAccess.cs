namespace Pathfinder.Inventory.Application.Storage;

public enum PartyStorageWithdrawalPolicy
{
    Unconfigured = 0,
    FreeForMembers = 1,
    GameMasterOnly = 2,
}

public sealed record PartyStorageAccess(
    bool ActiveParty,
    int PartyId,
    bool ControlsCharacter,
    bool IsGameMaster,
    PartyStorageWithdrawalPolicy WithdrawalPolicy )
{
    public static PartyStorageAccess Denied { get; } = new(
        false,
        0,
        false,
        false,
        PartyStorageWithdrawalPolicy.Unconfigured );
}
