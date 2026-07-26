using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Knowledge;

public sealed class ItemPropertyKnowledge : Entity, IAggregateRoot
{
    public const int UpgradeCodeMaxLength = 100;

    private ItemPropertyKnowledge()
    {
    }

    public int CampaignId { get; private set; }
    public Guid InstanceKey { get; private set; }
    public ItemKnowledgeSubjectKind SubjectKind { get; private set; }
    public int SubjectId { get; private set; }
    public string UpgradeCode { get; private set; } = String.Empty;
    public int RevealedByUserId { get; private set; }
    public DateTimeOffset RevealedAtUtc { get; private set; }

    public static ItemPropertyKnowledge Create(
        int campaignId,
        Guid instanceKey,
        ItemKnowledgeSubjectKind subjectKind,
        int subjectId,
        string upgradeCode,
        int revealedByUserId,
        DateTimeOffset revealedAtUtc )
    {
        if ( campaignId <= 0 || subjectId <= 0 || revealedByUserId <= 0 )
        {
            throw new ItemCatalogException(
                "Knowledge campaign, subject, and actor ids must be greater than zero." );
        }

        if ( instanceKey == Guid.Empty || !Enum.IsDefined( subjectKind ) )
        {
            throw new ItemCatalogException(
                "Knowledge item instance or subject kind is invalid." );
        }

        string normalizedCode = upgradeCode?.Trim() ?? String.Empty;
        if ( normalizedCode.Length == 0 ||
             normalizedCode.Length > UpgradeCodeMaxLength )
        {
            throw new ItemCatalogException(
                "Knowledge upgrade code is invalid." );
        }

        if ( revealedAtUtc.Offset != TimeSpan.Zero )
        {
            throw new ItemCatalogException( "Knowledge timestamp must use UTC." );
        }

        return new ItemPropertyKnowledge
        {
            CampaignId = campaignId,
            InstanceKey = instanceKey,
            SubjectKind = subjectKind,
            SubjectId = subjectId,
            UpgradeCode = normalizedCode,
            RevealedByUserId = revealedByUserId,
            RevealedAtUtc = revealedAtUtc,
        };
    }
}