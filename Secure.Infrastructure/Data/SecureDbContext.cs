using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Infrastructure.Data;

public class SecureDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public SecureDbContext( DbContextOptions<SecureDbContext> options ) : base( options )
    {
    }

    public DbSet<Permission> Permission { get; set; }
    public DbSet<RolePermission> RolePermission { get; set; }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        base.OnModelCreating( modelBuilder );
        modelBuilder.HasDefaultSchema( "secure" );
        modelBuilder.Entity<Permission>()
                    .ToTable( "Permission" )
                    .HasData( SeedData.BuildPermissions() );

        modelBuilder.Entity<Role>().ToTable( "Role" )
                    .HasData( SeedData.BuildApplicationRoles() );

        modelBuilder.Entity<User>().ToTable( "User" )
                    .HasData( SeedData.BuildApplicationUsers() );

        modelBuilder.Entity( ( Action<EntityTypeBuilder<RolePermission>> )( b =>
        {
            b.ToTable( "RolePermission" );
            b.HasKey( rp => new { rp.RoleId, rp.PermissionId } );

            b.HasOne( rp => rp.Role )
             .WithMany( r => r.RolePermissions )
             .HasForeignKey( pt => pt.RoleId );

            b.HasOne( rp => rp.Permission )
             .WithMany( p => p.RolePermissions )
             .HasForeignKey( rp => rp.PermissionId );

            b.HasData( SeedData.BuildRolePermissions() );
        } ) );

        modelBuilder.Entity( ( Action<EntityTypeBuilder<UserRole>> )( b =>
        {
            b.ToTable( "UserRole" );

            b.HasOne( ur => ur.User )
             .WithMany( u => u.UserRoles )
             .HasForeignKey( ur => ur.UserId );

            b.HasOne( ur => ur.Role )
             .WithMany( r => r.UserRoles )
             .HasForeignKey( ur => ur.RoleId );

            b.HasData( SeedData.BuildApplicationUserRoles() );
        } ) );

        modelBuilder.Entity<UserClaim>().ToTable( "UserClaim" );
        modelBuilder.Entity<UserLogin>().ToTable( "UserLogin" );
        modelBuilder.Entity<RoleClaim>().ToTable( "RoleClaim" );
        modelBuilder.Entity<UserToken>().ToTable( "UserToken" );
    }

    public async Task Commit()
    {
        await SaveChangesAsync();
    }
}