using System.Collections.Generic;
using System.Linq;

namespace Pathfinder.Utils.Entities.Base;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals( object obj )
    {
        if ( obj == null || obj.GetType() != GetType() )
        {
            return false;
        }

        ValueObject other = ( ValueObject )obj;
        return GetEqualityComponents()
           .SequenceEqual( other.GetEqualityComponents() );
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
           .Select( x => x?.GetHashCode() ?? 0 )
           .Aggregate( ( x, y ) => x ^ y );
    }

    public static bool operator ==( ValueObject left, ValueObject right )
    {
        if ( ReferenceEquals( left, null ) && ReferenceEquals( right, null ) )
        {
            return true;
        }

        if ( ReferenceEquals( left, null ) || ReferenceEquals( right, null ) )
        {
            return false;
        }

        return left.Equals( right );
    }

    public static bool operator !=( ValueObject left, ValueObject right ) => !( left == right );
}