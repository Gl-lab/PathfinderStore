using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Domain.Offers;

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

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "commerce" );
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
                    "\"AvailableQuantity\" > 0 AND \"UnitPriceCopper\" >= 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_ShopOffer_Target",
                    "(\"Kind\" = 1 AND \"ItemConfigurationId\" IS NOT NULL AND \"ItemInstanceKey\" IS NULL) OR " +
                    "(\"Kind\" = 2 AND \"ItemConfigurationId\" IS NULL AND \"ItemInstanceKey\" IS NOT NULL)" );
            } );
            builder.Property( offer => offer.Kind )
                .HasConversion<int>();
            builder.Property( offer => offer.Status )
                .HasConversion<int>();
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
    }
}
