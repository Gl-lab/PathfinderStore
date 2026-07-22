using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Application.Administration;

public sealed record ItemRevisionDto(
    int ItemDefinitionId,
    string Key,
    ItemCatalogScope Scope,
    int? CampaignId,
    int RevisionNumber,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemCategory PrimaryCategory,
    ItemRevisionStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset? RetiredAtUtc );