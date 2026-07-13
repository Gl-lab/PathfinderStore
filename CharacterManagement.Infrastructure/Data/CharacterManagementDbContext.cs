using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Data;

public class CharacterManagementDbContext( DbContextOptions<CharacterManagementDbContext> options )
    : DbContext( options )
{
    public DbSet<Account> Account { get; set; }
    public DbSet<DraftCharacter> Character { get; set; }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.HasDefaultSchema( "character_management" );

        modelBuilder.Entity<DraftCharacter>( b =>
        {
            b.ToTable( "Character" );

            b.Property( x => x.Name ).HasMaxLength( 200 );
            b.Property( x => x.Concept ).HasMaxLength( 1000 );
            b.Property( x => x.Age );
            b.Property( x => x.AncestryType ).HasConversion<int>();
            b.Property( x => x.SelectedHeritageId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedAncestryFeatId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedBackgroundId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedBackgroundRestrictedBoost ).HasConversion<int>();
            b.Property( x => x.SelectedBackgroundFreeBoost ).HasConversion<int>();
            b.Property( x => x.SelectedClassId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedClassKeyAbility ).HasConversion<int>();
            b.Property( x => x.SelectedRogueRacketId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedClericDoctrineId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedDeityId ).HasMaxLength( 100 );
            b.Property( x => x.SelectedDivineFont ).HasConversion<int>();
            b.Property( x => x.SelectedDivineSanctification ).HasConversion<int>();

            b.Property( x => x.AppliedFreeBoosts )
                .HasConversion(
                    v => JsonSerializer.Serialize( v.ToList(), (JsonSerializerOptions?)null ),
                    v => String.IsNullOrEmpty( v )
                        ? (IReadOnlyList<AbilityType>)Array.Empty<AbilityType>()
                        : (IReadOnlyList<AbilityType>)( JsonSerializer.Deserialize<List<AbilityType>>( v, (JsonSerializerOptions?)null ) ?? new List<AbilityType>() ) )
                .HasColumnType( "jsonb" )
                .HasDefaultValueSql( "'[]'::jsonb" )
                .Metadata.SetValueComparer( new ValueComparer<IReadOnlyList<AbilityType>>(
                    ( a, b ) => a != null && b != null && a.SequenceEqual( b ),
                    c => c.Aggregate( 0, ( h, v ) => HashCode.Combine( h, v ) ),
                    c => c.ToList() ) );

            b.Property( x => x.AppliedFinalFreeBoosts )
                .HasConversion(
                    v => JsonSerializer.Serialize( v.ToList(), (JsonSerializerOptions?)null ),
                    v => String.IsNullOrEmpty( v )
                        ? (IReadOnlyList<AbilityType>)Array.Empty<AbilityType>()
                        : (IReadOnlyList<AbilityType>)( JsonSerializer.Deserialize<List<AbilityType>>( v, (JsonSerializerOptions?)null ) ?? new List<AbilityType>() ) )
                .HasColumnType( "jsonb" )
                .HasDefaultValueSql( "'[]'::jsonb" )
                .Metadata.SetValueComparer( new ValueComparer<IReadOnlyList<AbilityType>>(
                    ( a, b ) => a != null && b != null && a.SequenceEqual( b ),
                    c => c.Aggregate( 0, ( h, v ) => HashCode.Combine( h, v ) ),
                    c => c.ToList() ) );

            b.Property( x => x.TrainedSkills )
                .HasConversion(
                    value => JsonSerializer.Serialize( value.ToList(), (JsonSerializerOptions?)null ),
                    value => String.IsNullOrEmpty( value )
                        ? (IReadOnlyList<TrainedSkill>)Array.Empty<TrainedSkill>()
                        : (IReadOnlyList<TrainedSkill>)( JsonSerializer.Deserialize<List<TrainedSkill>>( value, (JsonSerializerOptions?)null ) ?? new List<TrainedSkill>() ) )
                .HasColumnType( "jsonb" )
                .HasDefaultValueSql( "'[]'::jsonb" )
                .Metadata.SetValueComparer( new ValueComparer<IReadOnlyList<TrainedSkill>>(
                    ( first, second ) => first != null && second != null && first.SequenceEqual( second ),
                    collection => collection.Aggregate( 0, ( hash, item ) => HashCode.Combine( hash, item ) ),
                    collection => collection.ToList() ) );

            b.Property( x => x.TrainedLore )
                .HasConversion(
                    value => JsonSerializer.Serialize( value.ToList(), (JsonSerializerOptions?)null ),
                    value => String.IsNullOrEmpty( value )
                        ? (IReadOnlyList<TrainedLore>)Array.Empty<TrainedLore>()
                        : (IReadOnlyList<TrainedLore>)( JsonSerializer.Deserialize<List<TrainedLore>>( value, (JsonSerializerOptions?)null ) ?? new List<TrainedLore>() ) )
                .HasColumnType( "jsonb" )
                .HasDefaultValueSql( "'[]'::jsonb" )
                .Metadata.SetValueComparer( new ValueComparer<IReadOnlyList<TrainedLore>>(
                    ( first, second ) => first != null && second != null && first.SequenceEqual( second ),
                    collection => collection.Aggregate( 0, ( hash, item ) => HashCode.Combine( hash, item ) ),
                    collection => collection.ToList() ) );

            b.OwnsOne( x => x.AbilityScores, ab =>
            {
                ab.Ignore( a => a.MaxPortableWeight );
                ab.OwnsOne( a => a.Strength,      s => { s.Property( x => x.Value ).HasColumnName( "Str" ); s.Ignore( x => x.Modifier ); } );
                ab.OwnsOne( a => a.Dexterity,     s => { s.Property( x => x.Value ).HasColumnName( "Dex" ); s.Ignore( x => x.Modifier ); } );
                ab.OwnsOne( a => a.Constitution,  s => { s.Property( x => x.Value ).HasColumnName( "Con" ); s.Ignore( x => x.Modifier ); } );
                ab.OwnsOne( a => a.Intelligence,  s => { s.Property( x => x.Value ).HasColumnName( "Int" ); s.Ignore( x => x.Modifier ); } );
                ab.OwnsOne( a => a.Wisdom,        s => { s.Property( x => x.Value ).HasColumnName( "Wis" ); s.Ignore( x => x.Modifier ); } );
                ab.OwnsOne( a => a.Charisma,      s => { s.Property( x => x.Value ).HasColumnName( "Cha" ); s.Ignore( x => x.Modifier ); } );
            } );
        } );

        modelBuilder.Entity<Account>( b =>
        {
            b.ToTable( "Account" );

            b.Property( x => x.Id )
                .UseIdentityByDefaultColumn()
                .HasIdentityOptions( startValue: 3 );
            b.Property( x => x.Name ).HasMaxLength( 100 );
            b.Property( x => x.Surname ).HasMaxLength( 100 );
            b.HasIndex( x => x.UserId )
                .IsUnique();

            b.HasData(
                new
                {
                    Id = 1,
                    UserId = 1,
                    Name = "System",
                    Surname = "Administrator",
                },
                new
                {
                    Id = 2,
                    UserId = 2,
                    Name = "Test",
                    Surname = "User",
                } );

            b.HasMany( a => a.DraftCharacters )
                .WithOne( c => c.Account )
                .HasForeignKey( c => c.AccountId )
                .IsRequired()
                .OnDelete( DeleteBehavior.Cascade );
        } );
    }
}
