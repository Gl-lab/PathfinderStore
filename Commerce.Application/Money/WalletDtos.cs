using Pathfinder.Commerce.Domain.Money;

namespace Pathfinder.Commerce.Application.Money;

public sealed record WalletLedgerEntryDto(
    Guid OperationId,
    WalletTransactionKind Kind,
    long AmountCopper,
    long BalanceAfterCopper,
    string Description,
    int PerformedByUserId,
    DateTimeOffset OccurredAtUtc );

public sealed record WalletDto(
    int CampaignId,
    int CharacterId,
    long BalanceCopper,
    long ReservedCopper,
    long AvailableCopper,
    int Version,
    IReadOnlyCollection<WalletLedgerEntryDto> Entries );
