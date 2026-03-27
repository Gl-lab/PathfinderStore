namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Account : Utils.Entities.Base.Entity
{
    private readonly List<DraftCharacter> _draftCharacters = [];

    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int UserId { get; set; }
    public IReadOnlyList<DraftCharacter> DraftCharacters { get => _draftCharacters; }

    public void AddDraftCharacter( DraftCharacter draftCharacter )
    {
        ArgumentNullException.ThrowIfNull( draftCharacter );

        _draftCharacters.Add( draftCharacter );
    }
}
