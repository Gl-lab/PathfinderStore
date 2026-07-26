using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Commerce.Application.Money;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Application.Transactions;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CommerceReadProjectionService
{
    private readonly CommerceDbContext _commerceDbContext;
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly ICommerceBuyerAccessPolicy _buyerAccessPolicy;

    public CommerceReadProjectionService(
        CommerceDbContext commerceDbContext,
        CampaignManagementDbContext campaignDbContext,
        ICommerceBuyerAccessPolicy buyerAccessPolicy )
    {
        _commerceDbContext = commerceDbContext;
        _campaignDbContext = campaignDbContext;
        _buyerAccessPolicy = buyerAccessPolicy;
    }

    public async Task<WalletDto> GetWalletAsync(
        int campaignId,
        int characterId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        bool controlsCharacter = await _buyerAccessPolicy.ControlsCharacterAsync(
            campaignId,
            actingUserId,
            characterId,
            cancellationToken );
        if ( !controlsCharacter )
        {
            throw new CommerceReadAccessDeniedException();
        }

        Wallet? wallet = await _commerceDbContext.Wallets
            .AsNoTracking()
            .Include( item => item.Entries )
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.CharacterId == characterId,
                cancellationToken );
        if ( wallet is null )
        {
            return new WalletDto(
                campaignId,
                characterId,
                0,
                0,
                0,
                0,
                [] );
        }

        WalletLedgerEntryDto[] entries = wallet.Entries
            .OrderByDescending( entry => entry.OccurredAtUtc )
            .ThenBy( entry => entry.OperationId )
            .Select( entry => new WalletLedgerEntryDto(
                entry.OperationId,
                entry.Kind,
                entry.AmountCopper,
                entry.BalanceAfterCopper,
                entry.Description,
                entry.PerformedByUserId,
                entry.OccurredAtUtc ) )
            .ToArray();
        return new WalletDto(
            wallet.CampaignId,
            wallet.CharacterId,
            wallet.BalanceCopper,
            wallet.ReservedCopper,
            wallet.AvailableCopper,
            wallet.Version,
            entries );
    }

    public async Task<IReadOnlyCollection<SettlementDto>> GetSettlementsAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken )
    {
        Campaign? campaign = await _campaignDbContext.Campaigns
            .AsNoTracking()
            .Include( item => item.Memberships )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == campaignId &&
                    item.Status == CampaignStatus.Active,
                cancellationToken );
        bool isMember = campaign is not null &&
            ( campaign.HasActiveRole( actingUserId, CampaignMembershipRole.Player ) ||
              campaign.HasActiveRole( actingUserId, CampaignMembershipRole.GameMaster ) );
        if ( !isMember )
        {
            throw new CommerceReadAccessDeniedException();
        }

        Settlement[] settlements = await _commerceDbContext.Settlements
            .AsNoTracking()
            .Include( settlement => settlement.Shops )
            .Where( settlement => settlement.CampaignId == campaignId )
            .OrderBy( settlement => settlement.Name )
            .ThenBy( settlement => settlement.Id )
            .ToArrayAsync( cancellationToken );
        return settlements
            .Select( ToDto )
            .ToArray();
    }

    private static SettlementDto ToDto( Settlement settlement ) =>
        new SettlementDto(
            settlement.Id,
            settlement.CampaignId,
            settlement.Name,
            settlement.Level,
            settlement.Region,
            settlement.Traits,
            settlement.Shops
                .OrderBy( shop => shop.Name )
                .ThenBy( shop => shop.Id )
                .Select( shop => new ShopDto(
                    shop.Id,
                    shop.CampaignId,
                    shop.SettlementId,
                    shop.Name,
                    shop.Specialization,
                    shop.ShopLevel,
                    shop.CatalogPricePercent,
                    shop.BuybackPricePercent,
                    shop.PricingPolicyVersion ) )
                .ToArray() );
}

public sealed class CommerceReadAccessDeniedException : Exception
{
    public CommerceReadAccessDeniedException()
        : base( "Commerce read access is denied." )
    {
    }
}