namespace Pathfinder.Utils.Entities.Base;

public interface IEntityBase<TId>
{
    TId Id { get; }
}