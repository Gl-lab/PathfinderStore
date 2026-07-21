using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed record CampaignDto(
    int Id,
    int CurrentUserId,
    string Name,
    CampaignStatus Status,
    int CreatedByUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ArchivedAtUtc,
    IReadOnlyCollection<CampaignMembershipRole> Roles,
    IReadOnlyCollection<CampaignMemberDto> Members,
    IReadOnlyCollection<CampaignPartyDto> Parties );

public sealed record CampaignMemberDto(
    int UserId,
    IReadOnlyCollection<CampaignMembershipRole> Roles );

public sealed record CampaignPartyDto(
    int Id,
    string Name,
    CampaignPartyStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ArchivedAtUtc,
    IReadOnlyCollection<CampaignPartyCharacterDto> Characters,
    CampaignPartyStorageDto Storage );

public sealed record CampaignPartyCharacterDto(
    int Id,
    int CharacterId,
    int ControlledByUserId,
    DateTimeOffset AssignedAtUtc );

public sealed record CampaignPartyStorageDto(
    int Id,
    CampaignPartyStorageAccessPolicy AccessPolicy,
    DateTimeOffset CreatedAtUtc );
