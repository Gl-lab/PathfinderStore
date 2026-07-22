using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Configurations;
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
    public DbSet<ItemConfiguration> ItemConfigurations => Set<ItemConfiguration>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "item_catalog" );

        modelBuilder.Entity<ItemDefinition>( builder =>
        {
            builder.ToTable( "ItemDefinition", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_ItemDefinition_Scope",
                    "(\"Scope\" = 1 AND \"CampaignId\" IS NULL) OR " +
                    "(\"Scope\" = 2 AND \"CampaignId\" > 0)" ) );
            builder.Property( definition => definition.Key )
                .HasMaxLength( ItemDefinition.KeyMaxLength )
                .IsRequired();
            builder.Property( definition => definition.Scope )
                .HasConversion<int>()
                .HasDefaultValue( ItemCatalogScope.Global );
            builder.HasIndex( definition => definition.Key )
                .IsUnique()
                .HasFilter( "\"Scope\" = 1" );
            builder.HasIndex( definition => new
            {
                definition.CampaignId,
                definition.Key,
            } )
                .IsUnique()
                .HasFilter( "\"Scope\" = 2" );
            builder.HasMany( definition => definition.Revisions )
                .WithOne()
                .HasForeignKey( revision => revision.ItemDefinitionId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<ItemRevision>( builder =>
        {
            builder.ToTable( "ItemRevision", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_ItemRevision_Lifecycle",
                    "(\"Status\" = 1 AND \"PublishedAtUtc\" IS NULL AND \"RetiredAtUtc\" IS NULL) OR " +
                    "(\"Status\" = 2 AND \"PublishedAtUtc\" IS NOT NULL AND \"RetiredAtUtc\" IS NULL) OR " +
                    "(\"Status\" = 3 AND \"PublishedAtUtc\" IS NOT NULL AND " +
                    "\"RetiredAtUtc\" IS NOT NULL AND \"RetiredAtUtc\" >= \"PublishedAtUtc\")" ) );
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
            builder.Property( revision => revision.Status )
                .HasConversion<int>()
                .HasDefaultValue( ItemRevisionStatus.Draft );
            builder.HasIndex( revision => new
            {
                revision.ItemDefinitionId,
                revision.RevisionNumber,
            } )
                .IsUnique();
            builder.HasIndex( revision => revision.ItemDefinitionId )
                .IsUnique()
                .HasFilter( "\"Status\" = 2" );
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

        modelBuilder.Entity<ItemConfiguration>( builder =>
        {
            builder.ToTable( "ItemConfiguration" );
            builder.Property( configuration => configuration.ConfigurationKey )
                .HasMaxLength( ItemConfiguration.ConfigurationKeyLength )
                .IsFixedLength()
                .IsRequired();
            builder.Property( configuration => configuration.Size )
                .HasConversion<int>();
            builder.Property( configuration => configuration.MaterialType )
                .HasConversion<int>();
            builder.Property( configuration => configuration.MaterialGrade )
                .HasConversion<int>();
            builder.HasIndex( configuration => configuration.ConfigurationKey )
                .IsUnique();
            builder.HasOne<ItemRevision>()
                .WithMany()
                .HasForeignKey( configuration => configuration.ItemRevisionId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Restrict );
            builder.HasMany( configuration => configuration.PermanentUpgrades )
                .WithOne()
                .HasForeignKey( upgrade => upgrade.ItemConfigurationId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<PermanentUpgrade>( builder =>
        {
            builder.ToTable( "PermanentUpgrade" );
            builder.Property( upgrade => upgrade.Code )
                .HasMaxLength( PermanentUpgrade.CodeMaxLength )
                .IsRequired();
            builder.Property( upgrade => upgrade.Kind )
                .HasConversion<int>();
            builder.Property( upgrade => upgrade.Visibility )
                .HasConversion<int>();
            builder.HasIndex( upgrade => new
            {
                upgrade.ItemConfigurationId,
                upgrade.Code,
            } )
                .IsUnique();
        } );
    }
}
