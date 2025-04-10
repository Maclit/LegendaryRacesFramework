using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace LegendaryRacesFramework
{
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