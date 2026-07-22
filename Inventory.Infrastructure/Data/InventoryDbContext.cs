using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Domain.Operations;
using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Infrastructure.Data;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext( DbContextOptions<InventoryDbContext> options )
        : base( options )
    {
    }

    public DbSet<InventoryContainer> Containers => Set<InventoryContainer>();
    public DbSet<ItemInstance> ItemInstances => Set<ItemInstance>();
    public DbSet<InventoryMovement> Movements => Set<InventoryMovement>();
    public DbSet<InventoryOperation> Operations => Set<InventoryOperation>();
    public DbSet<PartyGift> PartyGifts => Set<PartyGift>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "inventory" );

        modelBuilder.Entity<InventoryContainer>( builder =>
        {
            builder.ToTable( "InventoryContainer", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_InventoryContainer_PositiveIds",
                    "\"CampaignId\" > 0 AND \"OwnerId\" > 0" ) );
            builder.Property( container => container.OwnerKind )
                .HasConversion<int>();
            builder.HasIndex( container => container.ContainerKey )
                .IsUnique();
            builder.HasIndex( container => new
            {
                container.CampaignId,
                container.ContainerKey,
            } )
                .IsUnique();
            builder.HasIndex( container => new
            {
                container.CampaignId,
                container.OwnerKind,
                container.OwnerId,
            } )
                .IsUnique();
        } );

        modelBuilder.Entity<ItemInstance>( builder =>
        {
            builder.ToTable( "ItemInstance", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_State",
                    "\"CampaignId\" > 0 AND \"ItemConfigurationId\" > 0 AND \"Quantity\" >= 0" ) );
            builder.Property( instance => instance.CustomName )
                .HasMaxLength( ItemInstance.CustomNameMaxLength );
            builder.Property( instance => instance.Version )
                .IsConcurrencyToken();
            builder.HasIndex( instance => instance.InstanceKey )
                .IsUnique();
            builder.HasIndex( instance => instance.ItemConfigurationId );
            builder.HasOne<InventoryContainer>()
                .WithMany()
                .HasForeignKey( instance => new
                {
                    instance.CampaignId,
                    instance.CurrentContainerKey,
                } )
                .HasPrincipalKey( container => new
                {
                    container.CampaignId,
                    container.ContainerKey,
                } )
                .OnDelete( DeleteBehavior.Restrict );
            builder.HasMany( instance => instance.Movements )
                .WithOne()
                .HasForeignKey( "ItemInstanceId" )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasMany( instance => instance.Operations )
                .WithOne()
                .HasForeignKey( "ItemInstanceId" )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<InventoryMovement>( builder =>
        {
            builder.ToTable( "InventoryMovement", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_InventoryMovement_Quantity",
                    "\"Quantity\" > 0" ) );
            builder.Property( movement => movement.Reason )
                .HasMaxLength( ItemInstance.MovementReasonMaxLength )
                .IsRequired();
            builder.Property( movement => movement.PerformedBy )
                .HasMaxLength( ItemInstance.PerformedByMaxLength )
                .IsRequired();
            builder.HasIndex( "ItemInstanceId", nameof( InventoryMovement.OperationId ) )
                .IsUnique();
        } );

        modelBuilder.Entity<InventoryOperation>( builder =>
        {
            builder.ToTable( "InventoryOperation", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_InventoryOperation_State",
                    "\"Quantity\" > 0 AND \"VersionAfter\" > 0" ) );
            builder.Property( operation => operation.Kind )
                .HasConversion<int>();
            builder.HasIndex( "ItemInstanceId", nameof( InventoryOperation.OperationId ) )
                .IsUnique();
        } );

        modelBuilder.Entity<PartyGift>( builder =>
        {
            builder.ToTable( "PartyGift", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_PartyGift_State",
                    "\"CampaignId\" > 0 AND \"PartyId\" > 0 AND \"SourceCharacterId\" > 0 AND \"DestinationCharacterId\" > 0 AND \"ExpectedItemVersion\" >= 0" ) );
            builder.Property( gift => gift.Status )
                .HasConversion<int>();
            builder.HasIndex( gift => gift.GiftKey )
                .IsUnique();
            builder.HasIndex( gift => new
            {
                gift.CampaignId,
                gift.PartyId,
                gift.Status,
            } );
            builder.HasIndex( gift => gift.ItemInstanceKey );
        } );
    }
}
