using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    [DefOf]
    public static class LRF_DefOf
    {
        public static LegendaryRaceDef LegendaryRaceBase;
        public static LegendaryCharacterDef LegendaryCharacterBase;
        
        static LRF_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(LRF_DefOf));
        }
    }
}