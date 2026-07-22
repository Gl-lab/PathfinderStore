using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Configurations;

public sealed class PermanentUpgrade : Entity
{
    public const int CodeMaxLength = 100;

    private bool _isAssigned;

    private PermanentUpgrade()
    {
    }

    public int ItemConfigurationId { get; private set; }
    public string Code { get; private set; } = String.Empty;
    public PermanentUpgradeKind Kind { get; private set; }
    public int Rank { get; private set; }
    public PermanentUpgradeVisibility Visibility { get; private set; }

    public static PermanentUpgrade Create(
        string code,
        PermanentUpgradeKind kind,
        int rank,
        PermanentUpgradeVisibility visibility )
    {
        string normalizedCode = NormalizeCode( code );
        if ( !Enum.IsDefined( kind ) || !Enum.IsDefined( visibility ) )
        {
            throw new ItemCatalogException( "Permanent upgrade type or visibility is invalid." );
        }

        if ( rank <= 0 )
        {
            throw new ItemCatalogException( "Permanent upgrade rank must be greater than zero." );
        }

        return new PermanentUpgrade
        {
            Code = normalizedCode,
            Kind = kind,
            Rank = rank,
            Visibility = visibility,
        };
    }

    internal void AssignToConfiguration()
    {
        EnsureCanAssignToConfiguration();
        _isAssigned = true;
    }

    internal void EnsureCanAssignToConfiguration()
    {
        if ( _isAssigned )
        {
            throw new ItemCatalogException(
                "The same permanent upgrade cannot be assigned to multiple configurations." );
        }
    }

    private static string NormalizeCode( string code )
    {
        if ( String.IsNullOrWhiteSpace( code ) )
        {
            throw new ItemCatalogException( "Permanent upgrade code cannot be empty." );
        }

        string normalizedCode = code.Trim();
        if ( normalizedCode.Length > CodeMaxLength )
        {
            throw new ItemCatalogException(
                $"Permanent upgrade code cannot exceed {CodeMaxLength} characters." );
        }

        bool hasInvalidCharacter = normalizedCode.Any( character =>
            !( Char.IsAsciiLetterLower( character ) ||
               Char.IsAsciiDigit( character ) ||
               ( character == '.' ) ||
               ( character == '-' ) ) );
        if ( hasInvalidCharacter )
        {
            throw new ItemCatalogException(
                "Permanent upgrade code must use lowercase ASCII letters, digits, dots, and hyphens." );
        }

        return normalizedCode;
    }
}