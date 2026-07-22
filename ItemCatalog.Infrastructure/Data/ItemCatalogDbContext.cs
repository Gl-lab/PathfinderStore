using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Infrastructure.Data;

public sealed class ItemCatalogDbContext : DbContext
{
    public ItemCatalogDbContext( DbContextOptions<ItemCatalogDbContext> options )
        : base( options )
    {
    }

    public DbSet<ItemDefinition> ItemDefinitions => Set<ItemDefinition>();
    public DbSet<ItemRevision> ItemRevisions => Set<ItemRevision>();
    public DbSet<AttackComponent> AttackComponents => Set<AttackComponent>();

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
            builder.Property( revision => revision.PrimaryCategory )
                .HasConversion<int>()
                .HasDefaultValue( ItemCategory.OtherEquipment );
            builder.HasIndex( revision => new
            {
                revision.ItemDefinitionId,
                revision.RevisionNumber,
            } )
                .IsUnique();
            builder.HasMany( revision => revision.Attacks )
                .WithOne()
                .HasForeignKey( component => component.ItemRevisionId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Armor )
                .WithOne()
                .HasForeignKey<ArmorComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Shield )
                .WithOne()
                .HasForeignKey<ShieldComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Equipment )
                .WithOne()
                .HasForeignKey<EquipmentComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Consumption )
                .WithOne()
                .HasForeignKey<ConsumptionComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Charges )
                .WithOne()
                .HasForeignKey<ChargeComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( revision => revision.Durability )
                .WithOne()
                .HasForeignKey<DurabilityComponent>( component => component.ItemRevisionId )
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<AttackComponent>( builder =>
        {
            builder.ToTable( "AttackComponent" );
            builder.Property( component => component.Name )
                .HasMaxLength( AttackComponent.NameMaxLength )
                .IsRequired();
            builder.Property( component => component.DamageDieSize )
                .HasConversion<int>();
            builder.Property( component => component.DamageType )
                .HasConversion<int>();
        } );

        modelBuilder.Entity<ArmorComponent>( builder =>
        {
            builder.ToTable( "ArmorComponent" );
            builder.Property( component => component.Category )
                .HasConversion<int>();
        } );

        modelBuilder.Entity<ShieldComponent>().ToTable( "ShieldComponent" );
        modelBuilder.Entity<EquipmentComponent>( builder =>
        {
            builder.ToTable( "EquipmentComponent" );
            builder.Property( component => component.Usage )
                .HasConversion<int>();
        } );
        modelBuilder.Entity<ConsumptionComponent>( builder =>
        {
            builder.ToTable( "ConsumptionComponent" );
            builder.Property( component => component.Mode )
                .HasConversion<int>();
        } );
        modelBuilder.Entity<ChargeComponent>( builder =>
        {
            builder.ToTable( "ChargeComponent" );
            builder.Property( component => component.RecoveryRule )
                .HasConversion<int>();
        } );
        modelBuilder.Entity<DurabilityComponent>().ToTable( "DurabilityComponent" );
    }
}