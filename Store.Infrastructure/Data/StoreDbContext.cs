using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pathfinder.Contracts.Core.Entities.Account;
using Pathfinder.Contracts.Core.Entities.Shop;

namespace Pathfinder.Store.Infrastructure.Data;

public class StoreDbContext : DbContext
{
    public StoreDbContext( DbContextOptions<StoreDbContext> options ) : base( options )
    {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Weapon> Weapon { get; set; }
    public DbSet<WeaponType> WeaponType { get; set; }
    public DbSet<Inventory> Backpack { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<WeaponItemProperty> WeaponItemProperty { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<ShopsProduct> ShopsProducts { get; set; }
    public DbSet<WeaponProficiency> WeaponProficiency { get; set; }


    private IDbContextTransaction _currentTransaction;
    public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

    public async Task BeginTransaction()
    {
        _currentTransaction ??= await Database.BeginTransactionAsync().ConfigureAwait( false );
    }

    public async Task Commit()
    {
        try
        {
            await SaveChangesAsync().ConfigureAwait( false );
            if ( _currentTransaction != null )
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await Rollback();
            throw;
        }
        finally
        {
            if ( _currentTransaction != null )
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task Rollback()
    {
        try
        {
            if ( _currentTransaction != null )
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if ( _currentTransaction != null )
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}