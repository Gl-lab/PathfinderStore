namespace Pathfinder.CharacterManagement.Domain.Rules.Statistics;

public enum StatisticBonusType
{
    Item,
    Status,
    Circumstance
}

public sealed record StatisticBonus(
    string SourceId,
    StatisticBonusType Type,
    int Value );
