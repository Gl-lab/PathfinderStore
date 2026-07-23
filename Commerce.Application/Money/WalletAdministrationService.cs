using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Money;

namespace Pathfinder.Commerce.Application.Money;

public sealed class WalletAdministrationService
{
    private readonly IWalletRepository _repository;
    private readonly ICommerceCampaignAccessPolicy _accessPolicy;
    private readonly TimeProvider _timeProvider;

    public WalletAdministrationService(
        IWalletRepository repository,
        ICommerceCampaignAccessPolicy accessPolicy,
        TimeProvider timeProvider )
    {
        _repository = repository;
        _accessPolicy = accessPolicy;
        _timeProvider = timeProvider;
    }

    public async Task<WalletDto> AdjustAsync(
        int campaignId,
        int characterId,
        Guid operationId,
        long amountCopper,
        string description,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool isGameMaster = await _accessPolicy.IsGameMasterAsync(
            campaignId,
            actingUserId,
            cancellationToken );
        if ( !isGameMaster )
        {
            throw new UnauthorizedAccessException(
                "Only an active campaign Game Master can adjust wallets." );
        }

        Wallet? wallet = await _repository.GetAsync(
            campaignId,
            characterId,
            cancellationToken );
        if ( wallet is null )
        {
            wallet = Wallet.Create( campaignId, characterId, _timeProvider.GetUtcNow() );
            _repository.Add( wallet );
        }

        wallet.ApplyAdjustment(
            operationId,
            amountCopper,
            description,
            actingUserId,
            _timeProvider.GetUtcNow() );
        await _repository.SaveChangesAsync( cancellationToken );
        return ToDto( wallet );
    }

    private static WalletDto ToDto( Wallet wallet ) => new WalletDto(
        wallet.CampaignId,
        wallet.CharacterId,
        wallet.BalanceCopper,
        wallet.ReservedCopper,
        wallet.AvailableCopper,
        wallet.Version,
        wallet.Entries
            .OrderBy( entry => entry.OccurredAtUtc )
            .Select( entry => new WalletLedgerEntryDto(
                entry.OperationId,
                entry.Kind,
                entry.AmountCopper,
                entry.BalanceAfterCopper,
                entry.Description,
                entry.PerformedByUserId,
                entry.OccurredAtUtc ) )
            .ToArray() );
}
