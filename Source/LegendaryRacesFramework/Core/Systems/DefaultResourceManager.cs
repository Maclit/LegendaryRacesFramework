using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    public class DefaultResourceManager : IResourceManager
    {
        private readonly string raceID;
        private readonly List<RaceResource> resources = new List<RaceResource>();
        private readonly Dictionary<Pawn, Dictionary<string, float>> pawnResourceValues = new Dictionary<Pawn, Dictionary<string, float>>();
        
        public string RaceID => raceID;
        
        public List<RaceResource> ManagedResources => resources;
        
        public DefaultResourceManager(string raceID, List<RaceResourceDef> resourceDefs)
        {
            this.raceID = raceID;
            
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
                        )
                    };
                    
                    RegisterResource(resource);
                }
            }
            
            // Register for game tick to update resources
            LRF_GameComponent.RegisterForTick(OnTick);
        }
        
        private void OnTick()
        {
            // Update resources periodically
            foreach (var pawnResources in pawnResourceValues.ToList())
            {
                Pawn pawn = pawnResources.Key;
                
                // Skip if pawn is no longer valid
                if (pawn == null || pawn.Destroyed || pawn.Dead)
                {
                    pawnResourceValues.Remove(pawn);
                    continue;
                }
                
                // Apply regeneration and check hediffs for each resource
                foreach (var resource in resources)
                {
                    if (resource.RegenerationRate != 0f)
                    {
                        // Convert from per-day rate to per-tick
                        float regenPerTick = resource.RegenerationRate / GenDate.TicksPerDay;
                        
                        // Apply regeneration
                        AdjustResourceValue(pawn, resource.ResourceID, regenPerTick);
                    }
                    
                    // Update hediffs based on resource level
                    float resourceValue = GetResourceValue(pawn, resource.ResourceID);
                    UpdateResourceHediffs(pawn, resource, resourceValue);
                }
            }
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
            
            // Initialize resource values for this pawn if not already done
            if (!pawnResourceValues.TryGetValue(pawn, out Dictionary<string, float> pawnResources))
            {
                pawnResources = new Dictionary<string, float>();
                pawnResourceValues.Add(pawn, pawnResources);
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
            
            // Find the resource
            RaceResource resource = resources.FirstOrDefault(r => r.ResourceID == resourceDefName);
            if (resource == null)
                return;
            
            // Clamp value to resource range
            value = Mathf.Clamp(value, resource.MinValue, resource.MaxValue);
            
            // Initialize resource values for this pawn if not already done
            if (!pawnResourceValues.TryGetValue(pawn, out Dictionary<string, float> pawnResources))
            {
                pawnResources = new Dictionary<string, float>();
                pawnResourceValues.Add(pawn, pawnResources);
            }
            
            // Set resource value
            pawnResources[resourceDefName] = value;
            
            // Update hediffs based on new value
            UpdateResourceHediffs(pawn, resource, value);
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