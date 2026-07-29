using Pathfinder.ItemCatalog.Domain.Configurations;

namespace Pathfinder.ItemCatalog.Application.Configurations;

public sealed record CreateItemConfigurationRequest(
    int CampaignId,
    int ItemDefinitionId,
    int RevisionNumber,
    ItemSize Size,
    ItemMaterialType MaterialType,
    ItemMaterialGrade MaterialGrade,
    IReadOnlyCollection<PermanentUpgrade> PermanentUpgrades,
    int ActingUserId );