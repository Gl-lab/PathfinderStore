using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class RogueRacketDtoMapper
{
    public static RogueRacketDto Map( RogueRacket racket )
    {
        ArgumentNullException.ThrowIfNull( racket );

        return new RogueRacketDto
        {
            Id = racket.Id,
            Name = racket.Name,
            Source = new SourceReferenceDto
            {
                Book = racket.Source.Book,
                Page = racket.Source.Page,
            },
            AlternativeKeyAbility = racket.AlternativeKeyAbility,
            SkillGrants = racket.SkillGrants
                .Select( grant => new RogueSkillGrantDto
                {
                    Id = grant.Id,
                    TargetId = grant.TargetId,
                    Options = grant.Options,
                    RequiresChoice = grant.RequiresChoice,
                } )
                .ToArray(),
            ProficiencyGrants = CharacterClassDtoMapper.MapProficiencies( racket.ProficiencyGrants ),
            Effects = racket.Effects.Select( Map ).ToArray(),
            DeferredDependencies = racket.DeferredDependencies,
        };
    }

    public static RogueRacketPackageDto MapPackage( RogueRacket racket )
    {
        ArgumentNullException.ThrowIfNull( racket );

        return new RogueRacketPackageDto
        {
            Id = racket.Id,
            Name = racket.Name,
            AlternativeKeyAbility = racket.AlternativeKeyAbility,
            Effects = racket.Effects.Select( Map ).ToArray(),
        };
    }

    private static RogueRacketEffectDto Map( RogueRacketEffectDescriptor effect )
    {
        return new RogueRacketEffectDto
        {
            Id = effect.Id,
            Name = effect.Name,
            Summary = effect.Summary,
        };
    }
}
