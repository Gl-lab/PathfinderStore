using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Transfers;

public sealed class PartyExchange : Entity, IAggregateRoot
{
    private readonly List<PartyExchangeLine> _lines = [];

    private PartyExchange()
    {
    }

    public Guid ExchangeKey { get; private set; }
    public int CampaignId { get; private set; }
    public int PartyId { get; private set; }
    public int InitiatorCharacterId { get; private set; }
    public int CounterpartyCharacterId { get; private set; }
    public PartyExchangeStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public IReadOnlyList<PartyExchangeLine> Lines { get => _lines.AsReadOnly(); }

    public static PartyExchange Create(
        Guid exchangeKey,
        int campaignId,
        int partyId,
        int initiatorCharacterId,
        int counterpartyCharacterId,
        IReadOnlyCollection<PartyExchangeLineRequest> lines,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc )
    {
        if ( exchangeKey == Guid.Empty )
        {
            throw new InventoryException( "Exchange key cannot be empty." );
        }

        if ( ( campaignId <= 0 ) || ( partyId <= 0 ) ||
             ( initiatorCharacterId <= 0 ) || ( counterpartyCharacterId <= 0 ) )
        {
            throw new InventoryException( "Exchange campaign, party, and character ids must be positive." );
        }

        if ( initiatorCharacterId == counterpartyCharacterId )
        {
            throw new InventoryException( "Exchange characters must be different." );
        }

        if ( ( lines.Count < 2 ) ||
             !lines.Any( line => line.FromCharacterId == initiatorCharacterId ) ||
             !lines.Any( line => line.FromCharacterId == counterpartyCharacterId ) ||
             lines.Any( line =>
                 ( line.FromCharacterId != initiatorCharacterId ) &&
                 ( line.FromCharacterId != counterpartyCharacterId ) ) )
        {
            throw new InventoryException( "Exchange must contain items from both characters only." );
        }

        if ( lines.Select( line => line.ItemInstanceKey ).Distinct().Count() != lines.Count )
        {
            throw new InventoryException( "Exchange item keys must be unique." );
        }

        EnsureUtc( createdAtUtc );
        EnsureUtc( expiresAtUtc );
        if ( expiresAtUtc <= createdAtUtc )
        {
            throw new InventoryException( "Exchange expiration must be after creation." );
        }

        PartyExchange exchange = new()
        {
            ExchangeKey = exchangeKey,
            CampaignId = campaignId,
            PartyId = partyId,
            InitiatorCharacterId = initiatorCharacterId,
            CounterpartyCharacterId = counterpartyCharacterId,
            Status = PartyExchangeStatus.Pending,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc,
        };
        exchange._lines.AddRange( lines.Select( line => PartyExchangeLine.Create(
            line.FromCharacterId,
            line.ItemInstanceKey,
            line.ExpectedItemVersion ) ) );
        return exchange;
    }

    public void EnsureMatches(
        int campaignId,
        int initiatorCharacterId,
        int counterpartyCharacterId,
        IReadOnlyCollection<PartyExchangeLineRequest> lines )
    {
        bool linesMatch = ( lines.Count == _lines.Count ) && lines.All( requested =>
            _lines.Any( existing =>
                ( existing.FromCharacterId == requested.FromCharacterId ) &&
                ( existing.ItemInstanceKey == requested.ItemInstanceKey ) &&
                ( existing.ExpectedItemVersion == requested.ExpectedItemVersion ) ) );
        if ( ( CampaignId != campaignId ) ||
             ( InitiatorCharacterId != initiatorCharacterId ) ||
             ( CounterpartyCharacterId != counterpartyCharacterId ) ||
             !linesMatch )
        {
            throw new InventoryException( "Exchange key was already used for another proposal." );
        }
    }

    private static void EnsureUtc( DateTimeOffset value )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( "Exchange timestamps must use UTC." );
        }
    }
}

public sealed record PartyExchangeLineRequest(
    int FromCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion );
