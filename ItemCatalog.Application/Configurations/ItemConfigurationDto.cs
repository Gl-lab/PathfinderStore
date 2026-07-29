using Pathfinder.ItemCatalog.Domain.Configurations;

namespace Pathfinder.ItemCatalog.Application.Configurations;

public sealed record ItemConfigurationDto(
    int ItemConfigurationId,
    int? CampaignId,
    int ItemRevisionId,
    string ConfigurationKey,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade,
    IReadOnlyCollection<PermanentUpgradeDto> PermanentUpgrades,
    bool WasCreated );

public sealed record PermanentUpgradeDto(
    string Code,
    PermanentUpgradeKind Kind,
    int Rank,
    PermanentUpgradeVisibility Visibility );