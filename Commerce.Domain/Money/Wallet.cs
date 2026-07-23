using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Money;

public sealed class Wallet : Entity, IAggregateRoot
{
    private readonly List<WalletLedgerEntry> _entries = [];

    private Wallet()
    {
    }

    public int CampaignId { get; private set; }
    public int CharacterId { get; private set; }
    public long BalanceCopper { get; private set; }
    public long ReservedCopper { get; private set; }
    public int Version { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<WalletLedgerEntry> Entries { get => _entries; }
    public long AvailableCopper { get => BalanceCopper - ReservedCopper; }

    public static Wallet Create(
        int campaignId,
        int characterId,
        DateTimeOffset createdAtUtc )
    {
        if ( ( campaignId <= 0 ) || ( characterId <= 0 ) )
        {
            throw new CommerceException( "Campaign and character ids must be greater than zero." );
        }

        EnsureUtc( createdAtUtc );
        return new Wallet
        {
            CampaignId = campaignId,
            CharacterId = characterId,
            BalanceCopper = 0,
            ReservedCopper = 0,
            Version = 0,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public WalletLedgerEntry ApplyAdjustment(
        Guid operationId,
        long amountCopper,
        string description,
        int performedByUserId,
        DateTimeOffset occurredAtUtc )
    {
        WalletLedgerEntry? existing = FindEntry( operationId );
        if ( existing is not null )
        {
            if ( ( existing.AmountCopper != amountCopper ) ||
                 ( existing.PerformedByUserId != performedByUserId ) ||
                 !String.Equals( existing.Description, description.Trim(), StringComparison.Ordinal ) )
            {
                throw new CommerceException(
                    "Wallet operation id was already used with different data." );
            }

            return existing;
        }

        EnsureOperation(
            operationId,
            amountCopper,
            description,
            performedByUserId,
            occurredAtUtc );
        long nextBalance = checked( BalanceCopper + amountCopper );
        if ( nextBalance < ReservedCopper )
        {
            throw new CommerceException( "Wallet has insufficient available funds." );
        }

        WalletTransactionKind kind = amountCopper > 0
            ? WalletTransactionKind.AdjustmentCredit
            : WalletTransactionKind.AdjustmentDebit;
        BalanceCopper = nextBalance;
        Version++;
        WalletLedgerEntry entry = WalletLedgerEntry.Create(
            operationId,
            kind,
            amountCopper,
            BalanceCopper,
            description.Trim(),
            performedByUserId,
            occurredAtUtc );
        _entries.Add( entry );
        return entry;
    }

    public WalletLedgerEntry ReserveFunds(
        Guid operationId,
        long amountCopper,
        int performedByUserId,
        DateTimeOffset occurredAtUtc )
    {
        EnsureReservationOperation( operationId, performedByUserId, occurredAtUtc );
        if ( amountCopper <= 0 )
        {
            throw new CommerceException( "Reserved amount must be greater than zero." );
        }

        WalletLedgerEntry? existing = FindEntry( operationId );
        if ( existing is not null )
        {
            if ( ( existing.Kind != WalletTransactionKind.ReservationHold ) ||
                 ( existing.AmountCopper != -amountCopper ) )
            {
                throw new CommerceException(
                    "Wallet operation id was already used with different data." );
            }

            return existing;
        }

        if ( amountCopper > AvailableCopper )
        {
            throw new CommerceException( "Wallet has insufficient available funds." );
        }

        ReservedCopper = checked( ReservedCopper + amountCopper );
        Version++;
        WalletLedgerEntry entry = WalletLedgerEntry.Create(
            operationId,
            WalletTransactionKind.ReservationHold,
            -amountCopper,
            BalanceCopper,
            "Purchase funds reserved.",
            performedByUserId,
            occurredAtUtc );
        _entries.Add( entry );
        return entry;
    }

    public WalletLedgerEntry ReleaseFunds(
        Guid operationId,
        long amountCopper,
        int performedByUserId,
        DateTimeOffset occurredAtUtc )
    {
        EnsureReservationOperation( operationId, performedByUserId, occurredAtUtc );
        if ( ( amountCopper <= 0 ) || ( amountCopper > ReservedCopper ) )
        {
            throw new CommerceException( "Released reservation amount is invalid." );
        }

        WalletLedgerEntry? existing = FindEntry( operationId );
        if ( existing is not null )
        {
            return existing;
        }

        ReservedCopper -= amountCopper;
        Version++;
        WalletLedgerEntry entry = WalletLedgerEntry.Create(
            operationId,
            WalletTransactionKind.ReservationRelease,
            amountCopper,
            BalanceCopper,
            "Purchase funds released.",
            performedByUserId,
            occurredAtUtc );
        _entries.Add( entry );
        return entry;
    }

    private static void EnsureReservationOperation(
        Guid operationId,
        int performedByUserId,
        DateTimeOffset occurredAtUtc )
    {
        if ( operationId == Guid.Empty )
        {
            throw new CommerceException( "Wallet operation id cannot be empty." );
        }

        if ( performedByUserId <= 0 )
        {
            throw new CommerceException( "Wallet actor user id must be greater than zero." );
        }

        EnsureUtc( occurredAtUtc );
    }

    private WalletLedgerEntry? FindEntry( Guid operationId ) =>
        _entries.SingleOrDefault( entry => entry.OperationId == operationId );

    private static void EnsureOperation(
        Guid operationId,
        long amountCopper,
        string description,
        int performedByUserId,
        DateTimeOffset occurredAtUtc )
    {
        if ( operationId == Guid.Empty )
        {
            throw new CommerceException( "Wallet operation id cannot be empty." );
        }

        if ( amountCopper == 0 )
        {
            throw new CommerceException( "Wallet adjustment cannot be zero." );
        }

        if ( performedByUserId <= 0 )
        {
            throw new CommerceException( "Wallet actor user id must be greater than zero." );
        }

        if ( String.IsNullOrWhiteSpace( description ) ||
             ( description.Trim().Length > WalletLedgerEntry.DescriptionMaxLength ) )
        {
            throw new CommerceException(
                $"Wallet description must contain at most {WalletLedgerEntry.DescriptionMaxLength} characters." );
        }

        EnsureUtc( occurredAtUtc );
    }

    private static void EnsureUtc( DateTimeOffset value )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Wallet timestamp must use UTC." );
        }
    }
}
