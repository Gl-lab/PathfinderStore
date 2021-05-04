using System;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Exceptions;

namespace Pathfinder.Core.Entities.Account
{
    public class Character: Entity
    {
        public string Name { get; set; }
        public int RaceId { get; set; }
        public virtual Backpack Backpack { get; set; }
        public virtual Race Race { get; init; }
        public virtual GroupCharacteristic Characteristics { get; set; }

       public void Rename(string newName)
        {
            if (newName == null) throw new ArgumentNullException(nameof(newName));
            if (newName != string.Empty && newName != Name) Name = newName;
        }

        public void ChangeRace(int newRaceId)
        {
            if (newRaceId == 0) throw new CoreException("A nonexistent race");
            if (newRaceId != RaceId) RaceId = newRaceId;
        }
        
    }

    
}