using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class DruidicOrderRepository : IDruidicOrderRepository
{
    private static readonly Dictionary<string, DruidicOrder> DruidicOrders = CreateDruidicOrders()
        .ToDictionary( druidicOrder => druidicOrder.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<DruidicOrder> GetAll() => DruidicOrders.Values.ToArray();

    public DruidicOrder GetDruidicOrder( string druidicOrderId )
    {
        if ( String.IsNullOrWhiteSpace( druidicOrderId ) )
        {
            throw new ArgumentException( "Druidic Order id cannot be empty.", nameof( druidicOrderId ) );
        }

        if ( !DruidicOrders.TryGetValue( druidicOrderId, out DruidicOrder? druidicOrder ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( druidicOrderId ),
                $"Druidic Order '{druidicOrderId}' is not defined." );
        }

        return druidicOrder;
    }

    private static IReadOnlyCollection<DruidicOrder> CreateDruidicOrders()
    {
        return
        [
            Create( "animal", "Animal", 125, "athletics", "Animal Companion", "animal_companion", "Heal Animal", "heal_animal" ),
            Create( "leaf", "Leaf", 125, "diplomacy", "Leshy Familiar", "leshy_familiar", "Cornucopia", "cornucopia" ),
            Create( "storm", "Storm", 125, "acrobatics", "Storm Born", "storm_born", "Tempest Surge", "tempest_surge" ),
            Create( "untamed", "Untamed", 126, "intimidation", "Untamed Form", "untamed_form", "Untamed Shift", "untamed_shift" ),
        ];
    }

    private static DruidicOrder Create(
        string id,
        string name,
        int page,
        string skillId,
        string featName,
        string featId,
        string spellName,
        string spellId )
    {
        string druidicOrderId = $"druidic_order.{id}";
        return new DruidicOrder(
            druidicOrderId,
            name,
            new SourceReference( "Player Core", page ),
            new ClassSkillGrantDescriptor(
                $"{druidicOrderId}.skill.order",
                [ $"skill.{skillId}" ] ),
            [
                new DruidicOrderBenefitDescriptor(
                    $"feat.{featId}",
                    DruidicOrderBenefitKind.ClassFeat,
                    featName,
                    [ CharacterClassDependencyType.ClassFeatCatalog ] ),
                new DruidicOrderBenefitDescriptor(
                    $"spell.{spellId}",
                    DruidicOrderBenefitKind.FocusSpell,
                    spellName,
                    [] ),
            ] );
    }
}
