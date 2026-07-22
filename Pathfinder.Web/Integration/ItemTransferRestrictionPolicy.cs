using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Transfers;

namespace Pathfinder.Web.Integration;

public sealed class ItemTransferRestrictionPolicy : IItemTransferRestrictionPolicy
{
    private readonly CharacterManagementDbContext _dbContext;

    public ItemTransferRestrictionPolicy( CharacterManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsEquippedAsync(
        int characterId,
        Guid itemInstanceKey,
        CancellationToken cancellationToken )
    {
        DraftCharacter? character = await _dbContext.Character
            .AsNoTracking()
            .SingleOrDefaultAsync( item => item.Id == characterId, cancellationToken );
        return character?.RuntimeEquipmentItems.Any( item =>
            ( item.ItemInstanceKey == itemInstanceKey ) && item.IsEquipped ) ?? false;
    }
}
