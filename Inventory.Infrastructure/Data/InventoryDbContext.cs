using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Movements;
using Pathfinder.Inventory.Domain.Operations;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Domain.Audit;

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
    public DbSet<PartyExchange> PartyExchanges => Set<PartyExchange>();
    public DbSet<InventoryAuditEntry> AuditEntries => Set<InventoryAuditEntry>();

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
            {
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_State",
                    "\"CampaignId\" > 0 AND \"ItemConfigurationId\" > 0 AND \"Quantity\" >= 0" );
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_Charges",
                    "(\"MaximumCharges\" IS NULL AND \"CurrentCharges\" IS NULL AND " +
                    "\"DefaultActivationCost\" IS NULL AND \"ChargeRecoveryRule\" IS NULL) OR " +
                    "(\"MaximumCharges\" > 0 AND \"CurrentCharges\" >= 0 AND " +
                    "\"CurrentCharges\" <= \"MaximumCharges\" AND \"DefaultActivationCost\" > 0 AND " +
                    "\"DefaultActivationCost\" <= \"MaximumCharges\" AND " +
                    "\"ChargeRecoveryRule\" IN (1, 2, 3))" );
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_Consumption",
                    "(\"ConsumptionMode\" IS NULL AND \"ConsumptionQuantity\" IS NULL) OR " +
                    "(\"ConsumptionMode\" IN (1, 2, 3) AND \"ConsumptionQuantity\" > 0 AND " +
                    "((\"IsStackable\" AND \"ConsumptionMode\" IN (2, 3)) OR " +
                    "(NOT \"IsStackable\" AND \"ConsumptionMode\" = 1 AND " +
                    "\"ConsumptionQuantity\" = 1)))" );
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_Durability",
                    "(\"Hardness\" IS NULL AND \"MaximumHitPoints\" IS NULL AND " +
                    "\"CurrentHitPoints\" IS NULL AND \"BrokenThreshold\" IS NULL) OR " +
                    "(NOT \"IsStackable\" AND \"Hardness\" >= 0 AND " +
                    "\"MaximumHitPoints\" > 0 AND \"CurrentHitPoints\" >= 0 AND " +
                    "\"CurrentHitPoints\" <= \"MaximumHitPoints\" AND " +
                    "\"BrokenThreshold\" > 0 AND " +
                    "\"BrokenThreshold\" <= \"MaximumHitPoints\" AND " +
                    "((\"CurrentHitPoints\" = 0 AND \"Quantity\" = 0) OR " +
                    "(\"CurrentHitPoints\" > 0 AND \"Quantity\" = 1)))" );
                tableBuilder.HasCheckConstraint(
                    "CK_ItemInstance_Rune",
                    "(\"RuneTargetKind\" IS NULL AND \"AttachableRuneCode\" IS NULL AND " +
                    "\"AttachedToInstanceKey\" IS NULL) OR " +
                    "(\"RuneTargetKind\" IN (1, 2) AND " +
                    "((\"AttachableRuneCode\" IS NULL AND \"AttachedToInstanceKey\" IS NULL) OR " +
                    "(\"AttachableRuneCode\" IS NOT NULL)))" );
            } );
            builder.Property( instance => instance.CustomName )
                .HasMaxLength( ItemInstance.CustomNameMaxLength );
            builder.Property( instance => instance.ChargeRecoveryRule )
                .HasConversion<int?>();
            builder.Property( instance => instance.ConsumptionMode )
                .HasConversion<int?>();
            builder.Property( instance => instance.RuneTargetKind )
                .HasConversion<int?>();
            builder.Property( instance => instance.AttachableRuneCode )
                .HasMaxLength( ItemInstance.CustomNameMaxLength );
            builder.Property( instance => instance.Version )
                .IsConcurrencyToken();
            builder.HasIndex( instance => instance.InstanceKey )
                .IsUnique();
            builder.HasIndex( instance => instance.ItemConfigurationId );
            builder.HasIndex( instance => instance.AttachedToInstanceKey );
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

        modelBuilder.Entity<PartyExchange>( builder =>
        {
            builder.ToTable( "PartyExchange", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_PartyExchange_State",
                    "\"CampaignId\" > 0 AND \"PartyId\" > 0 AND \"InitiatorCharacterId\" > 0 AND \"CounterpartyCharacterId\" > 0" ) );
            builder.Property( exchange => exchange.Status )
                .HasConversion<int>();
            builder.Property( exchange => exchange.Version )
                .IsConcurrencyToken();
            builder.HasIndex( exchange => exchange.ExchangeKey )
                .IsUnique();
            builder.HasMany( exchange => exchange.Lines )
                .WithOne()
                .HasForeignKey( line => line.PartyExchangeId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<PartyExchangeLine>( builder =>
        {
            builder.ToTable( "PartyExchangeLine", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_PartyExchangeLine_State",
                    "\"FromCharacterId\" > 0 AND \"ExpectedItemVersion\" >= 0" ) );
            builder.HasIndex( line => new
            {
                line.PartyExchangeId,
                line.ItemInstanceKey,
            } )
                .IsUnique();
        } );

        modelBuilder.Entity<InventoryAuditEntry>( builder =>
        {
            builder.ToTable( "InventoryAuditEntry", tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_InventoryAuditEntry_Identity",
                    "\"CampaignId\" > 0 AND \"ActorUserId\" > 0" ) );
            builder.Property( audit => audit.ActionKind )
                .HasConversion<int>();
            builder.Property( audit => audit.Reason )
                .HasMaxLength( InventoryAuditEntry.ReasonMaxLength )
                .IsRequired();
            builder.HasIndex( audit => audit.AuditKey )
                .IsUnique();
            builder.HasIndex( audit => new
            {
                audit.CampaignId,
                audit.OperationId,
                audit.ActionKind,
            } )
                .IsUnique();
        } );
    }
}
