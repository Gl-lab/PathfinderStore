using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.CharacterManagement.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CampaignCharacterDirectory : ICampaignCharacterDirectory
{
    private readonly CharacterManagementDbContext _dbContext;

    public CampaignCharacterDirectory( CharacterManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<CampaignCharacterReference?> GetOwnedCharacterAsync(
        int characterId,
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Character
            .AsNoTracking()
            .Where( character =>
                character.Id == characterId &&
                character.Account.UserId == userId )
            .Select( character => new CampaignCharacterReference( character.Id, character.Name ) )
            .SingleOrDefaultAsync( cancellationToken );
    }

    public async Task<IReadOnlyCollection<CampaignCharacterReference>> GetOwnedCharactersAsync(
        int userId,
        CancellationToken cancellationToken )
    {
        return await _dbContext.Character
            .AsNoTracking()
            .Where( character => character.Account.UserId == userId )
            .OrderBy( character => character.Name )
            .Select( character => new CampaignCharacterReference( character.Id, character.Name ) )
            .ToArrayAsync( cancellationToken );
    }
}
