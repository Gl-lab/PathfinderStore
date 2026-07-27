using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Pathfinder.Commerce.Domain.Administration;
using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;

namespace Pathfinder.Commerce.Application.Administration;

public static class CommerceAdminOperationSupport
{
    public static string HashPayload<TPayload>( TPayload payload )
    {
        string json = JsonSerializer.Serialize( payload );
        byte[] hash = SHA256.HashData( Encoding.UTF8.GetBytes( json ) );
        return Convert.ToHexString( hash );
    }

    public static void EnsureOperationId( Guid operationId )
    {
        if ( operationId == Guid.Empty )
        {
            throw new CommerceException( "Operation id cannot be empty." );
        }
    }

    public static CommerceAdminOperation Create(
        int campaignId,
        Guid operationId,
        string actionKind,
        string payloadHash,
        int actingUserId,
        DateTimeOffset createdAtUtc,
        Settlement? settlement = null,
        Shop? shop = null,
        ShopOffer? offer = null ) => CommerceAdminOperation.Create(
        campaignId,
        operationId,
        actionKind,
        payloadHash,
        actingUserId,
        createdAtUtc,
        settlement,
        shop,
        offer );
}
