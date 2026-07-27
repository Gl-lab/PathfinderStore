using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Money;
using Pathfinder.Commerce.Domain.Transactions;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Domain.Administration;

namespace Pathfinder.Commerce.Infrastructure.Data;

public sealed class CommerceDbContext : DbContext
{
    public CommerceDbContext( DbContextOptions<CommerceDbContext> options )
        : base( options )
    {
    }

    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<ShopOffer> ShopOffers => Set<ShopOffer>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletLedgerEntry> WalletLedgerEntries => Set<WalletLedgerEntry>();
    public DbSet<PurchaseReservation> PurchaseReservations => Set<PurchaseReservation>();
    public DbSet<ShopSale> ShopSales => Set<ShopSale>();
    public DbSet<RestockPolicy> RestockPolicies => Set<RestockPolicy>();
    public DbSet<RestockPolicyRevision> RestockPolicyRevisions => Set<RestockPolicyRevision>();
    public DbSet<RestockRun> RestockRuns => Set<RestockRun>();
    public DbSet<RestockRunLine> RestockRunLines => Set<RestockRunLine>();
    public DbSet<CommerceAdminOperation> CommerceAdminOperations => Set<CommerceAdminOperation>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "commerce" );
        modelBuilder.Entity<CommerceAdminOperation>( builder =>
        {
            builder.ToTable( "CommerceAdminOperation", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_CommerceAdminOperation_Identity",
                    "\"CampaignId\" > 0 AND \"PerformedByUserId\" > 0" );
            } );
            builder.Property( operation => operation.ActionKind )
                .HasMaxLength( CommerceAdminOperation.ActionKindMaxLength )
                .IsRequired();
            builder.Property( operation => operation.PayloadHash )
                .HasMaxLength( CommerceAdminOperation.PayloadHashMaxLength )
                .IsRequired();
            builder.HasIndex( operation => new
            {
                operation.CampaignId,
                operation.OperationId,
            } )
                .IsUnique();
            builder.HasOne( operation => operation.Settlement )
                .WithMany()
                .HasForeignKey( operation => operation.SettlementId )
                .OnDelete( DeleteBehavior.Restrict );
            builder.HasOne( operation => operation.Shop )
                .WithMany()
                .HasForeignKey( operation => operation.ShopId )
                .OnDelete( DeleteBehavior.Restrict );
            builder.HasOne( operation => operation.Offer )
                .WithMany()
                .HasForeignKey( operation => operation.OfferId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
        modelBuilder.Entity<Settlement>( builder =>
        {
            builder.ToTable( "Settlement", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint( "CK_Settlement_CampaignId", "\"CampaignId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_Settlement_Level",
                    "(\"Level\" >= 0) AND (\"Level\" <= 20)" );
            } );
            builder.Property( settlement => settlement.Name )
                .HasMaxLength( Settlement.NameMaxLength )
                .IsRequired();
            builder.Property( settlement => settlement.Region )
                .HasMaxLength( Settlement.RegionMaxLength )
                .IsRequired();
            builder.Property( settlement => settlement.Traits )
                .HasMaxLength( Settlement.TraitsMaxLength )
                .IsRequired();
            builder.HasIndex( settlement => new
            {
                settlement.CampaignId,
                settlement.Name,
            } )
                .IsUnique();
            builder.HasMany( settlement => settlement.Shops )
                .WithOne()
                .HasForeignKey( shop => shop.SettlementId )
                .OnDelete( DeleteBehavior.Cascade );
        } );
        modelBuilder.Entity<Shop>( builder =>
        {
            builder.ToTable( "Shop", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint( "CK_Shop_CampaignId", "\"CampaignId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_Shop_Level",
                    "(\"ShopLevel\" >= 0) AND (\"ShopLevel\" <= 20)" );
            } );
            builder.Property( shop => shop.Name )
                .HasMaxLength( Shop.NameMaxLength )
                .IsRequired();
            builder.Property( shop => shop.Specialization )
                .HasMaxLength( Shop.SpecializationMaxLength )
                .IsRequired();
            builder.Property( shop => shop.CatalogPricePercent )
                .HasDefaultValue( 100 );
            builder.Property( shop => shop.BuybackPricePercent )
                .HasDefaultValue( 50 );
            builder.Property( shop => shop.PricingPolicyVersion )
                .IsConcurrencyToken()
                .HasDefaultValue( 1 );
            builder.HasIndex( shop => new
            {
                shop.SettlementId,
                shop.Name,
            } )
                .IsUnique();
            builder.HasIndex( shop => new
            {
                shop.CampaignId,
                shop.Id,
            } );
        } );
        modelBuilder.Entity<ShopOffer>( builder =>
        {
            builder.ToTable( "ShopOffer", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_ShopOffer_Identity",
                    "\"CampaignId\" > 0 AND \"ShopId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_ShopOffer_QuantityPrice",
                    "\"AvailableQuantity\" > 0 AND \"ReservedQuantity\" >= 0 AND " +
                    "\"ReservedQuantity\" <= \"AvailableQuantity\" AND \"UnitPriceCopper\" >= 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_ShopOffer_Target",
                    "(\"Kind\" = 1 AND \"ItemConfigurationId\" IS NOT NULL AND \"ItemInstanceKey\" IS NULL) OR " +
                    "(\"Kind\" = 2 AND \"ItemConfigurationId\" IS NULL AND \"ItemInstanceKey\" IS NOT NULL)" );
            } );
            builder.Property( offer => offer.Kind )
                .HasConversion<int>();
            builder.Property( offer => offer.Status )
                .HasConversion<int>();
            builder.Property( offer => offer.Version )
                .IsConcurrencyToken();
            builder.HasIndex( offer => offer.OfferKey )
                .IsUnique();
            builder.HasIndex( offer => offer.ItemInstanceKey )
                .IsUnique()
                .HasFilter( "\"Status\" = 1 AND \"ItemInstanceKey\" IS NOT NULL" );
            builder.HasIndex( offer => new
            {
                offer.CampaignId,
                offer.ShopId,
                offer.Status,
            } );
            builder.HasOne<Shop>()
                .WithMany()
                .HasForeignKey( offer => offer.ShopId )
                .OnDelete( DeleteBehavior.Cascade );
        } );
        modelBuilder.Entity<Wallet>( builder =>
        {
            builder.ToTable( "Wallet", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Wallet_Identity",
                    "\"CampaignId\" > 0 AND \"CharacterId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_Wallet_Balance",
                    "\"BalanceCopper\" >= 0 AND \"ReservedCopper\" >= 0 AND " +
                    "\"ReservedCopper\" <= \"BalanceCopper\"" );
            } );
            builder.Property( wallet => wallet.Version )
                .IsConcurrencyToken();
            builder.HasIndex( wallet => new
            {
                wallet.CampaignId,
                wallet.CharacterId,
            } )
                .IsUnique();
            builder.HasMany( wallet => wallet.Entries )
                .WithOne()
                .HasForeignKey( entry => entry.WalletId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
        modelBuilder.Entity<WalletLedgerEntry>( builder =>
        {
            builder.ToTable( "WalletLedgerEntry", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_WalletLedgerEntry_Amount",
                    "\"AmountCopper\" <> 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_WalletLedgerEntry_Actor",
                    "\"PerformedByUserId\" > 0" );
            } );
            builder.Property( entry => entry.Kind )
                .HasConversion<int>();
            builder.Property( entry => entry.Description )
                .HasMaxLength( WalletLedgerEntry.DescriptionMaxLength )
                .IsRequired();
            builder.HasIndex( entry => new
            {
                entry.WalletId,
                entry.OperationId,
            } )
                .IsUnique();
        } );
        modelBuilder.Entity<PurchaseReservation>( builder =>
        {
            builder.ToTable( "PurchaseReservation", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_PurchaseReservation_Identity",
                    "\"CampaignId\" > 0 AND \"BuyerCharacterId\" > 0 AND \"Quantity\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_PurchaseReservation_Price",
                    "\"UnitPriceCopper\" >= 0 AND \"TotalPriceCopper\" >= 0" );
            } );
            builder.Property( reservation => reservation.Status )
                .HasConversion<int>();
            builder.HasIndex( reservation => reservation.ReservationKey )
                .IsUnique();
            builder.HasIndex( reservation => new
            {
                reservation.CampaignId,
                reservation.OperationId,
            } )
                .IsUnique();
            builder.HasIndex( reservation => new
            {
                reservation.CampaignId,
                reservation.OfferKey,
                reservation.Status,
            } );
        } );
        modelBuilder.Entity<ShopSale>( builder =>
        {
            builder.ToTable( "ShopSale", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_ShopSale_Identity",
                    "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND " +
                    "\"SellerCharacterId\" > 0 AND \"ItemConfigurationId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_ShopSale_Price",
                    "\"Quantity\" > 0 AND \"UnitPriceCopper\" >= 0 AND \"TotalPriceCopper\" >= 0" );
            } );
            builder.HasIndex( sale => sale.SaleKey )
                .IsUnique();
            builder.HasIndex( sale => new
            {
                sale.CampaignId,
                sale.OperationId,
            } )
                .IsUnique();
        } );
        modelBuilder.Entity<RestockPolicy>( builder =>
        {
            builder.ToTable( "RestockPolicy", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RestockPolicy_Identity",
                    "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND \"CurrentVersion\" > 0" );
            } );
            builder.Property( policy => policy.Name )
                .HasMaxLength( RestockPolicy.NameMaxLength )
                .IsRequired();
            builder.Property( policy => policy.CurrentVersion )
                .IsConcurrencyToken();
            builder.HasIndex( policy => policy.ShopId )
                .IsUnique();
            builder.HasOne<Shop>()
                .WithMany()
                .HasForeignKey( policy => policy.ShopId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasMany( policy => policy.Revisions )
                .WithOne()
                .HasForeignKey( revision => revision.RestockPolicyId )
                .OnDelete( DeleteBehavior.Cascade );
        } );
        modelBuilder.Entity<RestockPolicyRevision>( builder =>
        {
            builder.ToTable( "RestockPolicyRevision", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RestockPolicyRevision_Values",
                    "\"Version\" > 0 AND \"TargetOfferCount\" > 0 AND \"CreatedByUserId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_RestockPolicyRevision_Constraints",
                    "\"MinimumItemLevel\" >= 0 AND \"MaximumItemLevel\" >= \"MinimumItemLevel\" AND " +
                    "\"MaximumItemLevel\" <= 30 AND \"BudgetCopper\" >= 0 AND " +
                    "\"AllowedRarities\" > 0 AND \"AllowedAccess\" > 0 AND \"AllowedCategories\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_RestockPolicyRevision_Weights",
                    "\"ConsumableWeight\" >= 0 AND \"PermanentWeight\" >= 0 AND \"UniqueWeight\" >= 0 AND " +
                    "(\"ConsumableWeight\" + \"PermanentWeight\" + \"UniqueWeight\") > 0" );
            } );
            builder.HasIndex( revision => new
            {
                revision.RestockPolicyId,
                revision.Version,
            } )
                .IsUnique();
            builder.Property( revision => revision.AllowedRarities )
                .HasConversion<int>();
            builder.Property( revision => revision.AllowedAccess )
                .HasConversion<int>();
            builder.Property( revision => revision.AllowedCategories )
                .HasConversion<int>();
        } );
        modelBuilder.Entity<RestockRun>( builder =>
        {
            builder.ToTable( "RestockRun", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RestockRun_Identity",
                    "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND \"RestockPolicyId\" > 0 AND " +
                    "\"PolicyVersion\" > 0 AND \"CreatedByUserId\" > 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_RestockRun_Lifecycle",
                    "(\"Status\" = 1 AND \"CompletedByUserId\" IS NULL AND \"CompletedAtUtc\" IS NULL) OR " +
                    "(\"Status\" IN (2, 3) AND \"CompletedByUserId\" > 0 AND \"CompletedAtUtc\" IS NOT NULL)" );
            } );
            builder.Property( run => run.Status )
                .HasConversion<int>()
                .IsConcurrencyToken();
            builder.HasIndex( run => run.RunKey )
                .IsUnique();
            builder.HasIndex( run => new
            {
                run.ShopId,
                run.RestockPolicyId,
                run.PolicyVersion,
                run.Seed,
            } )
                .IsUnique();
            builder.HasOne<Shop>()
                .WithMany()
                .HasForeignKey( run => run.ShopId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne<RestockPolicy>()
                .WithMany()
                .HasForeignKey( run => run.RestockPolicyId )
                .OnDelete( DeleteBehavior.Restrict );
            builder.HasMany( run => run.Lines )
                .WithOne()
                .HasForeignKey( line => line.RestockRunId )
                .OnDelete( DeleteBehavior.Cascade );
        } );
        modelBuilder.Entity<RestockRunLine>( builder =>
        {
            builder.ToTable( "RestockRunLine", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RestockRunLine_Values",
                    "\"Sequence\" > 0 AND \"ItemConfigurationId\" > 0 AND \"Quantity\" > 0 AND " +
                    "\"UnitPriceCopper\" >= 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_RestockRunLine_Publication",
                    "(\"PublishedOfferKey\" IS NULL AND \"PublishedItemInstanceKey\" IS NULL) OR " +
                    "(\"PublishedOfferKey\" IS NOT NULL AND " +
                    "((\"Kind\" = 3 AND \"PublishedItemInstanceKey\" IS NOT NULL) OR " +
                    "(\"Kind\" <> 3 AND \"PublishedItemInstanceKey\" IS NULL)))" );
            } );
            builder.Property( line => line.Kind )
                .HasConversion<int>();
            builder.HasIndex( line => new
            {
                line.RestockRunId,
                line.Sequence,
            } )
                .IsUnique();
            builder.HasIndex( line => new
            {
                line.RestockRunId,
                line.ItemConfigurationId,
            } )
                .IsUnique();
        } );
    }
}
