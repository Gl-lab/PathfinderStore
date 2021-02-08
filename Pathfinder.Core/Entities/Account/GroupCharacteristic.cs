using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class GroupCharacteristic: Entity
    {
        public GroupCharacteristic()
        {
            MaxPortableWeightByLevel = new Dictionary<int, int>()
            {
                {0,0},
                {1,10},
                {2,20},
                {3,30},
                {4,40},
                {5,50},
                {6,60},
                {7,70},
                {8,80},
                {9,90},
                {10,100},
                {11,115},
                {12,130},
                {13,150},
                {14,175},
                {15,200},
                {16,230},
                {17,260},
                {18,300},
                {19,350},
                {20,400},
                {21,460},
                {22,520},
                {23,600},
                {24,700},
                {25,800},
                {26,920},
                {27,1040},
                {28,1200},
                {29,1400}
            };
        }
        public virtual Characteristic Strength { get; set; }
        public virtual Characteristic Dexterity { get; set; }
        public virtual Characteristic Constitution { get; set; }
        public virtual Characteristic Intelligence { get; set; }
        public virtual Characteristic Wisdom { get; set; }
        public virtual Characteristic Charisma { get; set; }
        public int MaxPortableWeight => MaxPortableWeightByLevel[Strength.Value < 0? 0: Strength.Value>29?29:Strength.Value];

        private readonly Dictionary<int, int> MaxPortableWeightByLevel;
    }
    
   
}