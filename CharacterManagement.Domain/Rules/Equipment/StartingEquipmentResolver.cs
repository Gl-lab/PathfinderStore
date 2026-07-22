using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Equipment;

public sealed record CharacterEquipmentItem(
    string EquipmentId,
    int PurchaseQuantity,
    int EquippedQuantity = 0 );

public sealed record StartingEquipmentSelection(
    string ClassKitId,
    string CharacterClassId,
    IReadOnlyList<string> SelectedOptionIds,
    IReadOnlyList<CharacterEquipmentItem> Items,
    int TotalPriceCopper );

public static class StartingEquipmentResolver
{
    public static StartingEquipmentSelection Resolve(
        ClassKitDefinition classKit,
        IReadOnlyCollection<EquipmentDefinition> catalog,
        IReadOnlyList<string> selectedOptionIds,
        Deity? deity = null,
        string? deityFavoredWeaponEquipmentId = null )
    {
        ArgumentNullException.ThrowIfNull( classKit );
        ArgumentNullException.ThrowIfNull( catalog );
        ArgumentNullException.ThrowIfNull( selectedOptionIds );

        if ( selectedOptionIds.Any( String.IsNullOrWhiteSpace ) ||
             selectedOptionIds.Distinct( StringComparer.Ordinal ).Count() != selectedOptionIds.Count )
        {
            throw new CharacterManagementException( "Starting equipment option ids must be non-empty and unique." );
        }

        Dictionary<string, ClassKitOption> options = classKit.OptionGroups
            .SelectMany( group => group.Options )
            .ToDictionary( option => option.Id, StringComparer.Ordinal );
        foreach ( string optionId in selectedOptionIds )
        {
            if ( !options.ContainsKey( optionId ) )
            {
                throw new CharacterManagementException(
                    $"Starting equipment option '{optionId}' does not belong to kit '{classKit.Id}'." );
            }
        }

        bool selectsFavoredWeapon = selectedOptionIds
            .Select( optionId => options[ optionId ] )
            .Any( option => option.Dependency == ClassKitOptionDependency.DeityFavoredWeapon );
        if ( !selectsFavoredWeapon && !String.IsNullOrWhiteSpace( deityFavoredWeaponEquipmentId ) )
        {
            throw new CharacterManagementException(
                "A deity favored weapon can only be supplied with the matching class kit option." );
        }

        foreach ( ClassKitOptionGroup group in classKit.OptionGroups )
        {
            int selectedCount = group.Options.Count( option => selectedOptionIds.Contains( option.Id, StringComparer.Ordinal ) );
            if ( group.Selection == ClassKitOptionSelection.AtMostOne && selectedCount > 1 )
            {
                throw new CharacterManagementException(
                    $"Starting equipment option group '{group.Id}' allows at most one selection." );
            }
        }

        List<ClassKitItem> kitItems = classKit.Items.ToList();
        foreach ( string optionId in selectedOptionIds )
        {
            ClassKitOption option = options[ optionId ];
            CharacterEquipmentItem? dependencyItem = ResolveDependencyItem(
                option,
                deity,
                deityFavoredWeaponEquipmentId );
            kitItems.AddRange( option.Items );
            if ( dependencyItem is not null )
            {
                kitItems.Add( new ClassKitItem( dependencyItem.EquipmentId, dependencyItem.PurchaseQuantity ) );
            }
        }

        Dictionary<string, EquipmentDefinition> definitions = catalog
            .ToDictionary( definition => definition.Id, StringComparer.Ordinal );
        List<CharacterEquipmentItem> items = kitItems
            .GroupBy( item => item.EquipmentId, StringComparer.Ordinal )
            .Select( group => new CharacterEquipmentItem( group.Key, group.Sum( item => item.PurchaseQuantity ) ) )
            .OrderBy( item => item.EquipmentId, StringComparer.Ordinal )
            .ToList();

        int totalPriceCopper = 0;
        foreach ( CharacterEquipmentItem item in items )
        {
            if ( !definitions.TryGetValue( item.EquipmentId, out EquipmentDefinition? definition ) )
            {
                throw new CharacterManagementException(
                    $"Equipment '{item.EquipmentId}' from kit '{classKit.Id}' is absent from the catalog." );
            }

            if ( definition.Rarity != EquipmentRarity.Common &&
                 !IsDeityFavoredWeapon( definition, deity ) )
            {
                throw new CharacterManagementException(
                    $"Uncommon equipment '{definition.Id}' requires an explicit availability rule." );
            }

            totalPriceCopper += definition.PriceCopper * item.PurchaseQuantity;
        }

        if ( totalPriceCopper > ClassKitDefinition.StartingWealthCopper )
        {
            throw new CharacterManagementException(
                $"Starting equipment costs {totalPriceCopper} cp, exceeding the {ClassKitDefinition.StartingWealthCopper} cp budget." );
        }

        return new StartingEquipmentSelection(
            classKit.Id,
            classKit.CharacterClassId,
            selectedOptionIds.ToArray(),
            items,
            totalPriceCopper );
    }

    private static CharacterEquipmentItem? ResolveDependencyItem(
        ClassKitOption option,
        Deity? deity,
        string? deityFavoredWeaponEquipmentId )
    {
        if ( option.Dependency != ClassKitOptionDependency.DeityFavoredWeapon )
        {
            return null;
        }

        if ( deity is null )
        {
            throw new CharacterManagementException( "A deity is required to select the favored weapon option." );
        }

        if ( String.IsNullOrWhiteSpace( deityFavoredWeaponEquipmentId ) )
        {
            if ( deity.FavoredWeapons.Any( weapon => weapon.Category == FavoredWeaponCategory.Unarmed ) )
            {
                return null;
            }

            throw new CharacterManagementException( "A purchasable deity favored weapon must be selected." );
        }

        bool matches = deity.FavoredWeapons.Any(
            weapon => deityFavoredWeaponEquipmentId == $"equipment.{weapon.Id[ "weapon.".Length.. ]}" );
        if ( !matches )
        {
            throw new CharacterManagementException(
                $"Equipment '{deityFavoredWeaponEquipmentId}' is not a favored weapon of deity '{deity.Id}'." );
        }

        return new CharacterEquipmentItem( deityFavoredWeaponEquipmentId, 1 );
    }

    private static bool IsDeityFavoredWeapon( EquipmentDefinition definition, Deity? deity )
    {
        return deity is not null && deity.FavoredWeapons.Any(
            weapon => definition.Id == $"equipment.{weapon.Id[ "weapon.".Length.. ]}" );
    }
}
