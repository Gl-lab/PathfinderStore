using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Data;

public class CharacterManagementDbContext( DbContextOptions<CharacterManagementDbContext> options )
    : DbContext( options )
{
    public DbSet<Account> Account { get; set; }
    public DbSet<DraftCharacter> Character { get; set; }
    public DbSet<Characteristic> Characteristic { get; set; }
    public DbSet<AbilityScores> GroupCharacteristic { get; set; }
    public DbSet<Race> Race { get; set; }
}