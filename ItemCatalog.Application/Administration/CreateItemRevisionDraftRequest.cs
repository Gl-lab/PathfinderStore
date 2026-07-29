using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Application.Administration;

public sealed record CreateItemRevisionDraftRequest(
    int ItemDefinitionId,
    string Name,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    ItemRevisionRules Rules,
    int ActingUserId,
    string ActingUserName );