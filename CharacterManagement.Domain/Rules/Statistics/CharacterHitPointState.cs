namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public sealed record CharacterHitPointState(
    int Current,
    int Temporary,
    int Maximum );

public enum HitPointOperation
{
    ApplyDamage,
    Restore,
    GrantTemporary,
    ClearTemporary
}
