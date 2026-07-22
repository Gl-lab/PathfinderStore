using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Items;

namespace Pathfinder.ItemCatalog.Infrastructure.Data;

public sealed class ItemCatalogDbContext : DbContext
{
    public ItemCatalogDbContext( DbContextOptions<ItemCatalogDbContext> options )
        : base( options )
    {
    }

    public DbSet<ItemDefinition> ItemDefinitions => Set<ItemDefinition>();
    public DbSet<ItemRevision> ItemRevisions => Set<ItemRevision>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "item_catalog" );

        modelBuilder.Entity<ItemDefinition>( builder =>
        {
            builder.ToTable( "ItemDefinition" );
            builder.Property( definition => definition.Key )
                .HasMaxLength( ItemDefinition.KeyMaxLength )
                .IsRequired();
            builder.HasIndex( definition => definition.Key )
                .IsUnique();
            builder.HasMany( definition => definition.Revisions )
                .WithOne()
                .HasForeignKey( revision => revision.ItemDefinitionId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<ItemRevision>( builder =>
        {
            builder.ToTable( "ItemRevision" );
            builder.Property( revision => revision.Name )
                .HasMaxLength( ItemRevision.NameMaxLength )
                .IsRequired();
            builder.Property( revision => revision.Description )
                .HasMaxLength( ItemRevision.DescriptionMaxLength )
                .IsRequired();
            builder.Property( revision => revision.Bulk )
                .HasPrecision( 8, 2 );
            builder.HasIndex( revision => new
            {
                revision.ItemDefinitionId,
                revision.RevisionNumber,
            } )
                .IsUnique();
        } );
    }
}