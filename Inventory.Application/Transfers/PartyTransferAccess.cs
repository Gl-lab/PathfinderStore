namespace Pathfinder.Inventory.Application.Transfers;

public sealed record PartyTransferAccess(
    bool SameActiveParty,
    int PartyId,
    bool ControlsSource,
    bool ControlsDestination )
{
    public static PartyTransferAccess Denied { get; } = new( false, 0, false, false );
}
