namespace Pathfinder.CampaignManagement.Domain.Exceptions;

public sealed class CampaignManagementException : Exception
{
    public CampaignManagementException( string message )
        : base( message )
    {
    }
}
