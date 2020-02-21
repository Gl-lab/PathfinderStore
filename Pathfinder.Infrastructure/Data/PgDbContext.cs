using Pathfinder.Core.Entities;
using Pathfinder.Core.Entities.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pathfinder.Infrastructure.Data
{
    public class PgDbContext: DbContext
    {
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options) { }
        public DbSet<Product> ProductList { get; set; }
        public DbSet<Category> CategoryList { get; set; }
        public DbSet<DamageType> DamageTypeList { get; set; }
        public DbSet<BaseDice> DiceList { get; set; }
        public DbSet<Weapon> WeaponList { get; set; }
        public DbSet<WeaponType> WeaponTypeList { get; set; }

        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = _currentTransaction ?? await Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                _currentTransaction?.Commit();
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
    }
}
