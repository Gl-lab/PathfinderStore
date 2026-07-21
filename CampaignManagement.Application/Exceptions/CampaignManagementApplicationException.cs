namespace Pathfinder.CampaignManagement.Application.Exceptions;

public sealed class CampaignManagementApplicationException : Exception
{
    public CampaignManagementApplicationException( string message )
        : base( message )
    {
    }
}
