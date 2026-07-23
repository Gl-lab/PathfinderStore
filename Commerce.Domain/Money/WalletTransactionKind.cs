namespace Pathfinder.Commerce.Domain.Money;

public enum WalletTransactionKind
{
    AdjustmentCredit = 1,
    AdjustmentDebit = 2,
    Purchase = 3,
    Sale = 4,
    ReservationHold = 5,
    ReservationRelease = 6,
}
