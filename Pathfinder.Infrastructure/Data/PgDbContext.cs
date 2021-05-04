using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Entities.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Permissions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Shop;

namespace Pathfinder.Infrastructure.Data
{
    public class PgDbContext: IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options) { }
        public DbSet<Article> ArticleList { get; set; }
        public DbSet<Category> CategoryList { get; set; }
        public DbSet<DamageType> DamageTypeList { get; set; }
        public DbSet<Weapon> WeaponList { get; set; }
        public DbSet<WeaponType> WeaponTypeList { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Backpack> Backpack { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<Characteristic> Characteristic { get; set; }
        public DbSet<CharacteristicType> CharacteristicInfo { get; set; }
        public DbSet<GroupCharacteristic> GroupCharacteristic { get; set; }
        public DbSet<Race> Race { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<WeaponItemProperty> WeaponItemProperty { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopsProduct> ShopsProducts { get; set; }
        public DbSet<WeaponProficiency> WeaponProficiency { get; set; }
        

        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

        public async Task BeginTransactionAsync()
        {
            _currentTransaction ??= await Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);
                await _currentTransaction?.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
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

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
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