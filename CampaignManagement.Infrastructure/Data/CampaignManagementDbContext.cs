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
    }
}
