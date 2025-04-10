using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    [DefOf]
    public static class LRF_DefOf
    {
        // Removing references to any DefOf in static initialization
        // We'll use DefDatabase.GetNamed() to find defs instead
        
        static LRF_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(LRF_DefOf));
        }
    }
}