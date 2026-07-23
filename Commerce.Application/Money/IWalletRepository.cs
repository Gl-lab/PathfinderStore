using Pathfinder.Commerce.Domain.Money;

namespace Pathfinder.Commerce.Application.Money;

public interface IWalletRepository
{
    Task<Wallet?> GetAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken );
    void Add( Wallet wallet );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}
