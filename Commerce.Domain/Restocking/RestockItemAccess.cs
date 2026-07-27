namespace Pathfinder.Commerce.Domain.Restocking;

[Flags]
public enum RestockItemAccess
{
    Global = 1,
    Campaign = 2,
    All = Global | Campaign,
}
