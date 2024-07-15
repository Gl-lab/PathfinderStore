using System;
using Pathfinder.Core.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class Character : Entity
    {
        public AncestryType AncestryType { get; set; }
        public string Name { get; set; }
        //public int RaceId { get; set; }
        public Inventory Inventory { get; set; }
        //public Race Race { get; init; }
        public AbilityScores AbilityScores { get; set; }

        // public void Rename( string newName )
        // {
        //     if ( newName is null )
        //     {
        //         throw new ArgumentNullException( nameof( newName ) );
        //     }
        //
        //     if ( !String.IsNullOrEmpty( newName ) && newName != Name )
        //     {
        //         Name = newName;
        //     }
        // }

        // public void ChangeRace( int newRaceId )
        // {
        //     if ( newRaceId <= 0 )
        //     {
        //         throw new CoreException( "A nonexistent race" );
        //     }
        //     if ( newRaceId != RaceId )
        //     {
        //         RaceId = newRaceId;
        //     }
        // }
    }
}