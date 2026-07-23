using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Money;

public sealed class WalletLedgerEntry : Entity
{
    public const int DescriptionMaxLength = 500;

    private WalletLedgerEntry()
    {
    }

    public int WalletId { get; private set; }
    public Guid OperationId { get; private set; }
    public WalletTransactionKind Kind { get; private set; }
    public long AmountCopper { get; private set; }
    public long BalanceAfterCopper { get; private set; }
    public string Description { get; private set; } = String.Empty;
    public int PerformedByUserId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }

    internal static WalletLedgerEntry Create(
        Guid operationId,
        WalletTransactionKind kind,
        long amountCopper,
        long balanceAfterCopper,
        string description,
        int performedByUserId,
        DateTimeOffset occurredAtUtc ) => new WalletLedgerEntry
    {
        OperationId = operationId,
        Kind = kind,
        AmountCopper = amountCopper,
        BalanceAfterCopper = balanceAfterCopper,
        Description = description,
        PerformedByUserId = performedByUserId,
        OccurredAtUtc = occurredAtUtc,
    };
}
