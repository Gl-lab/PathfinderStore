using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Application.Administration;

public sealed record CreateItemDraftRequest(
    ItemCatalogScope Scope,
    int? CampaignId,
    string Key,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemRevisionRules Rules,
    int ActingUserId,
    string ActingUserName );