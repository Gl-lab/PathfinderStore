using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Authentication.Permissions;
using Pathfinder.Core.Entities.Authentication.Role;
using Pathfinder.Core.Entities.Authentication.User;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Entities.Shop;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Infrastructure.Data
{
    public class PgDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IUnitOfWork
    {
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<DamageType> DamageType { get; set; }
        public DbSet<Weapon> Weapon { get; set; }
        public DbSet<WeaponType> WeaponType { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Backpack> Backpack { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<Characteristic> Characteristic { get; set; }
        public DbSet<CharacteristicType> CharacteristicType { get; set; }
        public DbSet<GroupCharacteristic> GroupCharacteristic { get; set; }
        public DbSet<Race> Race { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<WeaponItemProperty> WeaponItemProperty { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopsProduct> ShopsProducts { get; set; }
        public DbSet<WeaponProficiency> WeaponProficiency { get; set; }


        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

        public async Task BeginTransaction()
        {
            _currentTransaction ??= await Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryType);

            modelBuilder.Entity<Permission>()
                .ToTable("Permission")
                .HasData(SeedData.BuildPermissions());

            modelBuilder.Entity<Role>().ToTable("Role")
                .HasData(SeedData.BuildApplicationRoles());

            modelBuilder.Entity<User>().ToTable("User")
                .HasData(SeedData.BuildApplicationUsers());

            modelBuilder.Entity((Action<EntityTypeBuilder<RolePermission>>)(b =>
            {
                b.ToTable("RolePermission");
                b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                b.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(pt => pt.RoleId);

                b.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId);

                b.HasData(SeedData.BuildRolePermissions());
            }));

            modelBuilder.Entity((Action<EntityTypeBuilder<UserRole>>)(b =>
            {
                b.ToTable("UserRole");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                b.HasData(SeedData.BuildApplicationUserRoles());
            }));

            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");
            modelBuilder.Entity<UserToken>().ToTable("UserToken");
            modelBuilder.Entity<UserToken>().ToTable("UserToken");
        }
    }
}