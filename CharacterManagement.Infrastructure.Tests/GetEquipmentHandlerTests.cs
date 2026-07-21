using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetEquipmentHandlerTests
{
    [Fact]
    public async Task EquipmentHandler_ReturnsCatalogSortedByName()
    {
        GetEquipmentHandler handler = new GetEquipmentHandler( new EquipmentRepository() );

        IReadOnlyCollection<EquipmentDto> result = await handler.Handle(
            new GetEquipmentCommand(),
            CancellationToken.None );

        Assert.Equal( 43, result.Count );
        Assert.Equal(
            result.Select( definition => definition.Name ).OrderBy( name => name ),
            result.Select( definition => definition.Name ) );
    }

    [Fact]
    public async Task ClassKitsHandler_ReturnsEightSupportedClassKits()
    {
        GetClassKitsHandler handler = new GetClassKitsHandler( new EquipmentRepository() );

        IReadOnlyCollection<ClassKitDto> result = await handler.Handle(
            new GetClassKitsCommand(),
            CancellationToken.None );

        Assert.Equal( 8, result.Count );
        Assert.All( result, kit => Assert.Equal( 1500, kit.StartingWealthCopper ) );
    }
}
