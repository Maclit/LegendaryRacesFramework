using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace LegendaryRacesFramework
{
    // Definition class for legendary races
    public class LegendaryRaceDef : Def
    {
        // This class is already defined in LegendaryRaceDef.cs
        // but needs to be referenced in DefOf class, so we include it here
    }

    // Def reference class for ThingDef with count
    public class ThingDefCount
    {
        public ThingDef thingDef;
        public int count = 1;
    }
    
    // Simple wrapper for DefOf usage
    [DefOf]
    public static class DefinitionTypeDefOf
    {
        static DefinitionTypeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefinitionTypeDefOf));
        }
    }
}