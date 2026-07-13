using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class BackgroundDtoMapper
{
    public static BackgroundDto Map( Background background )
    {
        ArgumentNullException.ThrowIfNull( background );

        return new BackgroundDto
        {
            Id = background.Id,
            Name = background.Name,
            Source = new SourceReferenceDto
            {
                Book = background.Source.Book,
                Page = background.Source.Page,
            },
            RestrictedBoostOptions = background.RestrictedBoostOptions.ToList(),
            FreeBoostCount = background.FreeBoostCount,
            Grants = background.Grants
                .Select( Map )
                .ToList(),
        };
    }

    public static CharacterBackgroundPackageDto MapPackage(
        DraftCharacter character,
        Background background )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( background );

        if ( !character.HasBackgroundBoostPackage )
        {
            throw new InvalidOperationException( "Character does not have a complete background boost package." );
        }

        return new CharacterBackgroundPackageDto
        {
            BackgroundId = background.Id,
            Name = background.Name,
            RestrictedBoost = character.SelectedBackgroundRestrictedBoost!.Value,
            FreeBoost = character.SelectedBackgroundFreeBoost!.Value,
            Grants = background.Grants
                .Select( Map )
                .ToList(),
        };
    }

    private static BackgroundGrantDto Map( BackgroundGrantDescriptor grant )
    {
        return new BackgroundGrantDto
        {
            Id = grant.Id,
            Kind = grant.Kind,
            Name = grant.Name,
            Summary = grant.Summary,
            RequiresChoice = grant.RequiresChoice,
            Options = grant.Options.ToList(),
            DeferredDependencies = grant.DeferredDependencies.ToList(),
        };
    }
}
