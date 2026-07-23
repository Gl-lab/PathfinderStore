using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Infrastructure.Data;

public sealed class CommerceDbContext : DbContext
{
    public CommerceDbContext( DbContextOptions<CommerceDbContext> options )
        : base( options )
    {
    }

    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Shop> Shops => Set<Shop>();

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
    }
}
