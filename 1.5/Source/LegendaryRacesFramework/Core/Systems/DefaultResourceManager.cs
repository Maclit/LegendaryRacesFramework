// File path: Source/LegendaryRacesFramework/Core/Systems/DefaultResourceManager.cs
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    public class DefaultResourceManager : IResourceManager
    {
        private readonly string raceID;
        private readonly List<RaceResource> resources = new List<RaceResource>();
        private readonly Dictionary<int, Dictionary<string, float>> pawnResourceValues = new Dictionary<int, Dictionary<string, float>>();
        
        // Track which ticks resources were last updated
        private readonly Dictionary<int, int> lastUpdateTicks = new Dictionary<int, int>();
        
        // Performance optimizations for RimWorld 1.5
        private int resourceUpdateInterval = 250; // Default, will be overridden by settings
        
        public string RaceID => raceID;
        
        public List<RaceResource> ManagedResources => resources;
        
        public DefaultResourceManager(string raceID, List<RaceResourceDef> resourceDefs)
        {
            this.raceID = raceID;
            
            // Get update interval from settings if available
            var raceDef = DefDatabase<LegendaryRaceDef>.GetNamed(raceID, false);
            if (raceDef?.performanceSettings != null)
            {
                resourceUpdateInterval = Mathf.Max(1, raceDef.performanceSettings.resourceUpdateInterval);
            }
            
            // Convert resource defs to runtime resources
            if (resourceDefs != null)
            {
                foreach (var resourceDef in resourceDefs)
                {
                    var resource = new RaceResource
                    {
                        ResourceID = resourceDef.resourceID,
                        ResourceName = resourceDef.resourceName,
                        ResourceDescription = resourceDef.resourceDescription,
                        MinValue = resourceDef.minValue,
                        MaxValue = resourceDef.maxValue,
                        DefaultValue = resourceDef.defaultValue,
                        IsVisible = resourceDef.isVisible,
                        RegenerationRate = resourceDef.regenerationRate,
                        IconPath = resourceDef.iconPath,
                        Category = resourceDef.category,
                        ResourceLevelHediffs = resourceDef.resourceLevelHediffs?.ToDictionary(
                            x => x.level,
                            x => x.hediff
                        ) ?? new Dictionary<float, HediffDef>()
                    };
                    
                    RegisterResource(resource);
                }
            }
            
            // Register for game tick to update resources
            LRF_GameComponent.RegisterForTick(OnTick);
        }
        
        private void OnTick()
        {
            int currentTick = Find.TickManager.TicksGame;
            
            // Process pawns with resources
            foreach (var pawnEntry in pawnResourceValues.ToList())
            {
                int pawnID = pawnEntry.Key;
                
                // Skip if not time to update yet
                if (lastUpdateTicks.TryGetValue(pawnID, out int lastUpdate) && 
                    currentTick - lastUpdate < resourceUpdateInterval)
                {
                    continue;
                }
                
                // Update last update time
                lastUpdateTicks[pawnID] = currentTick;
                
                // Find pawn by ID
                Pawn pawn = FindPawnByID(pawnID);
                
                // Skip if pawn is no longer valid
                if (pawn == null || pawn.Destroyed || pawn.Dead)
                {
                    pawnResourceValues.Remove(pawnID);
                    lastUpdateTicks.Remove(pawnID);
                    continue;
                }
                
                // Apply regeneration and check hediffs for each resource
                foreach (var resource in resources)
                {
                    if (resource.RegenerationRate != 0f)
                    {
                        // Convert from per-day rate to per-update
                        float regenPerUpdate = resource.RegenerationRate / GenDate.TicksPerDay * resourceUpdateInterval;
                        
                        // Apply regeneration
                        AdjustResourceValue(pawn, resource.ResourceID, regenPerUpdate);
                    }
                    
                    // Update hediffs based on resource level
                    float resourceValue = GetResourceValue(pawn, resource.ResourceID);
                    UpdateResourceHediffs(pawn, resource, resourceValue);
                }
            }
        }
        
        private Pawn FindPawnByID(int pawnID)
        {
            // Find pawn by ID in maps
            foreach (Map map in Find.Maps)
            {
                Pawn pawn = map.mapPawns.AllPawns.FirstOrDefault(p => p.thingIDNumber == pawnID);
                if (pawn != null)
                    return pawn;
            }
            
            // Find pawn by ID in world pawns
            Pawn worldPawn = Find.WorldPawns.AllWorldPawns.FirstOrDefault(p => p.thingIDNumber == pawnID);
            if (worldPawn != null)
                return worldPawn;
            
            return null;
        }
        
        private void UpdateResourceHediffs(Pawn pawn, RaceResource resource, float resourceValue)
        {
            if (resource.ResourceLevelHediffs == null || resource.ResourceLevelHediffs.Count == 0)
                return;
            
            // Sort levels from highest to lowest
            List<float> levels = resource.ResourceLevelHediffs.Keys.OrderByDescending(x => x).ToList();
            
            // Find applicable hediff based on resource level
            HediffDef applicableHediff = null;
            foreach (float level in levels)
            {
                if (resourceValue <= level)
                {
                    applicableHediff = resource.ResourceLevelHediffs[level];
                    break;
                }
            }
            
            // Apply or remove hediffs as needed
            if (applicableHediff != null)
            {
                // Remove all other resource hediffs for this resource
                foreach (var hediffDef in resource.ResourceLevelHediffs.Values.Where(h => h != applicableHediff))
                {
                    Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (existingHediff != null)
                    {
                        pawn.health.RemoveHediff(existingHediff);
                    }
                }
                
                // Add or update the applicable hediff
                Hediff existingApplicableHediff = pawn.health.hediffSet.GetFirstHediffOfDef(applicableHediff);
                if (existingApplicableHediff == null)
                {
                    Hediff hediff = HediffMaker.MakeHediff(applicableHediff, pawn);
                    hediff.Severity = 1f;
                    pawn.health.AddHediff(hediff);
                }
            }
            else
            {
                // Remove all resource hediffs for this resource
                foreach (var hediffDef in resource.ResourceLevelHediffs.Values)
                {
                    Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (existingHediff != null)
                    {
                        pawn.health.RemoveHediff(existingHediff);
                    }
                }
            }
        }
        
        public float GetResourceValue(Pawn pawn, string resourceDefName)
        {
            if (pawn == null || string.IsNullOrEmpty(resourceDefName))
                return 0f;
            
            int pawnID = pawn.thingIDNumber;
            
            // Initialize resource values for this pawn if not already done
            if (!pawnResourceValues.TryGetValue(pawnID, out Dictionary<string, float> pawnResources))
            {
                pawnResources = new Dictionary<string, float>();
                pawnResourceValues.Add(pawnID, pawnResources);
            }
            
            // Find the resource
            RaceResource resource = resources.FirstOrDefault(r => r.ResourceID == resourceDefName);
            if (resource == null)
                return 0f;
            
            // Initialize resource value if not already done
            if (!pawnResources.TryGetValue(resourceDefName, out float value))
            {
                value = resource.DefaultValue;
                pawnResources[resourceDefName] = value;
            }
            
            return value;
        }
        
        public void SetResourceValue(Pawn pawn, string resourceDefName, float value)
        {
            if (pawn == null || string.IsNullOrEmpty(resourceDefName))
                return;
            
            int pawnID = pawn.thingIDNumber;
            
            // Find the resource
            RaceResource resource = resources.FirstOrDefault(r => r.ResourceID == resourceDefName);
            if (resource == null)
                return;
            
            // Clamp value to resource range
            value = Mathf.Clamp(value, resource.MinValue, resource.MaxValue);
            
            // Initialize resource values for this pawn if not already done
            if (!pawnResourceValues.TryGetValue(pawnID, out Dictionary<string, float> pawnResources))
            {
                pawnResources = new Dictionary<string, float>();
                pawnResourceValues.Add(pawnID, pawnResources);
                lastUpdateTicks[pawnID] = Find.TickManager.TicksGame;
            }
            
            // Set resource value
            pawnResources[resourceDefName] = value;
            
            // Update hediffs based on new value
            UpdateResourceHediffs(pawn, resource, value);
            
            // Send notification if resource is at min/max
            if (resource.IsVisible && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                if (Mathf.Approximately(value, resource.MinValue))
                {
                    Messages.Message(
                        string.Format("LRF.ResourceDepleted".Translate(), pawn.LabelCap, resource.ResourceName),
                        pawn,
                        MessageTypeDefOf.NegativeEvent);
                }
                else if (Mathf.Approximately(value, resource.MaxValue))
                {
                    Messages.Message(
                        string.Format("LRF.ResourceFilled".Translate(), pawn.LabelCap, resource.ResourceName),
                        pawn,
                        MessageTypeDefOf.PositiveEvent);
                }
            }
        }
        
        public void AdjustResourceValue(Pawn pawn, string resourceDefName, float delta)
        {
            if (pawn == null || string.IsNullOrEmpty(resourceDefName) || delta == 0f)
                return;
            
            float currentValue = GetResourceValue(pawn, resourceDefName);
            SetResourceValue(pawn, resourceDefName, currentValue + delta);
        }
        
        public void RegisterResource(RaceResource resource)
        {
            if (resource == null || string.IsNullOrEmpty(resource.ResourceID))
                return;
            
            // Check for duplicate resource
            if (resources.Any(r => r.ResourceID == resource.ResourceID))
            {
                Log.Warning($"Duplicate resource registration for {resource.ResourceID} in race {raceID}");
                return;
            }
            
            resources.Add(resource);
        }
        
        public List<Hediff> GetResourceHediffs(Pawn pawn)
        {
            List<Hediff> hediffs = new List<Hediff>();
            
            if (pawn == null)
                return hediffs;
            
            // Collect all hediffs related to resources
            foreach (var resource in resources)
            {
                if (resource.ResourceLevelHediffs != null)
                {
                    foreach (var hediffDef in resource.ResourceLevelHediffs.Values)
                    {
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                        if (hediff != null)
                        {
                            hediffs.Add(hediff);
                        }
                    }
                }
            }
            
            return hediffs;
        }
    }
}