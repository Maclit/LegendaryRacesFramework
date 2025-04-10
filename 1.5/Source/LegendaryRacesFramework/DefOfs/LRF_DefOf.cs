using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    [DefOf]
    public static class LRF_DefOf
    {
        // Reference to the base legendary race def
        public static LegendaryRaceDef LegendaryRaceBase;
        
        // Reference to the base legendary character def
        public static LegendaryCharacterDef LegendaryCharacterBase;
        
        static LRF_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(LRF_DefOf));
        }
    }
}