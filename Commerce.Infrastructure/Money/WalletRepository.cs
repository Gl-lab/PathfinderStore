using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Money;

public sealed class WalletRepository : IWalletRepository
{
    private readonly CommerceDbContext _dbContext;

    public WalletRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<Wallet?> GetAsync(
        int campaignId,
        int characterId,
        CancellationToken cancellationToken ) => _dbContext.Wallets
        .Include( wallet => wallet.Entries )
        .SingleOrDefaultAsync(
            wallet =>
                ( wallet.CampaignId == campaignId ) &&
                ( wallet.CharacterId == characterId ),
            cancellationToken );

    public void Add( Wallet wallet )
    {
        _dbContext.Wallets.Add( wallet );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}
