using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Infrastructure.Data;

public sealed class CampaignManagementDbContext : DbContext
{
    public CampaignManagementDbContext( DbContextOptions<CampaignManagementDbContext> options )
        : base( options )
    {
    }

    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignMembership> CampaignMemberships => Set<CampaignMembership>();
    public DbSet<CampaignInvitation> CampaignInvitations => Set<CampaignInvitation>();
    public DbSet<CampaignParty> CampaignParties => Set<CampaignParty>();
    public DbSet<CampaignPartyCharacter> CampaignPartyCharacters => Set<CampaignPartyCharacter>();
    public DbSet<CampaignPartyStorage> CampaignPartyStorages => Set<CampaignPartyStorage>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "campaign_management" );

        modelBuilder.Entity<Campaign>( builder =>
        {
            builder.ToTable( "Campaign" );
            builder.Property( campaign => campaign.Name )
                .HasMaxLength( Campaign.NameMaxLength )
                .IsRequired();
            builder.Property( campaign => campaign.Status )
                .HasConversion<int>();
            builder.HasMany( campaign => campaign.Memberships )
                .WithOne()
                .HasForeignKey( membership => membership.CampaignId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasMany( campaign => campaign.Invitations )
                .WithOne()
                .HasForeignKey( invitation => invitation.CampaignId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasMany( campaign => campaign.Parties )
                .WithOne()
                .HasForeignKey( party => party.CampaignId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<CampaignMembership>( builder =>
        {
            builder.ToTable( "CampaignMembership" );
            builder.Property( membership => membership.Role )
                .HasConversion<int>();
            builder.Property( membership => membership.Status )
                .HasConversion<int>();
            builder.HasIndex( membership => new
                {
                    membership.CampaignId,
                    membership.UserId,
                    membership.Role,
                } )
                .IsUnique();
            builder.HasIndex( membership => membership.UserId );
        } );

        modelBuilder.Entity<CampaignInvitation>( builder =>
        {
            builder.ToTable( "CampaignInvitation" );
            builder.Property( invitation => invitation.Status )
                .HasConversion<int>();
            builder.HasIndex( invitation => invitation.InvitedUserId );
            builder.HasIndex( invitation => new
                {
                    invitation.CampaignId,
                    invitation.InvitedUserId,
                    invitation.Status,
                } );
        } );

        modelBuilder.Entity<CampaignParty>( builder =>
        {
            builder.ToTable( "CampaignParty" );
            builder.Property( party => party.Name )
                .HasMaxLength( CampaignParty.NameMaxLength )
                .IsRequired();
            builder.Property( party => party.Status )
                .HasConversion<int>();
            builder.HasIndex( party => party.CampaignId );
            builder.HasMany( party => party.Characters )
                .WithOne()
                .HasForeignKey( character => character.CampaignPartyId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
            builder.HasOne( party => party.Storage )
                .WithOne()
                .HasForeignKey<CampaignPartyStorage>( storage => storage.CampaignPartyId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<CampaignPartyCharacter>( builder =>
        {
            builder.ToTable( "CampaignPartyCharacter" );
            builder.HasIndex( character => character.CharacterId );
            builder.HasIndex( character => new
                {
                    character.CampaignPartyId,
                    character.CharacterId,
                } )
                .IsUnique();
        } );

        modelBuilder.Entity<CampaignPartyStorage>( builder =>
        {
            builder.ToTable( "CampaignPartyStorage" );
            builder.Property( storage => storage.AccessPolicy )
                .HasConversion<int>();
            builder.HasIndex( storage => storage.CampaignPartyId )
                .IsUnique();
        } );
    }
}
