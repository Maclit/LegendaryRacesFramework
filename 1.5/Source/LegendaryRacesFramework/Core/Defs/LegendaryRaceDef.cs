using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    public class LegendaryRaceDef : Def
    {
        // Basic race information
        public RaceProperties raceProperties;
        
        // Race resources
        public List<RaceResourceDef> raceResources;
        
        // Race abilities
        public List<RaceAbilityDef> raceAbilities;
        
        // Special mechanics
        public SpecialMechanics specialMechanics;
        
        // VEF Integration
        public VEFIntegration vefIntegration;
        
        // HAR Integration (Optional)
        public HARIntegration harIntegration;
        
        // CE Integration (Optional)
        public CEIntegration ceIntegration;
        
        // Balance Settings
        public BalanceSettings balanceSettings;
        
        // Performance Settings
        public PerformanceSettings performanceSettings;
        
        // Cached race handler instance
        private IRaceExtension raceHandler;

        public bool Abstract;

        // Get the race handler instance
        public IRaceExtension GetRaceHandler()
        {
            if (raceHandler == null)
            {
                // Create instance of the race handler based on configuration
                raceHandler = LegendaryRacesFrameworkMod.GetRegisteredRaceExtension(this.defName) ?? new DefaultRaceHandler(this);
                raceHandler.Initialize();
            }
            return raceHandler;
        }
        
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            
            // Resolve any references to other defs here
            raceProperties?.ResolveReferences(this);
            
            // Initialize resources
            if (raceResources != null)
            {
                foreach (var resource in raceResources)
                {
                    resource.ResolveReferences(this);
                }
            }
            
            // Initialize abilities
            if (raceAbilities != null)
            {
                foreach (var ability in raceAbilities)
                {
                    ability.ResolveReferences(this);
                }
            }
            
            // Initialize special mechanics
            specialMechanics?.ResolveReferences(this);
            
            // Initialize integration settings
            vefIntegration?.ResolveReferences(this);
            harIntegration?.ResolveReferences(this);
            ceIntegration?.ResolveReferences(this);
        }
        
        public override void PostLoad()
        {
            base.PostLoad();
            
            // Any post-loading initialization
            GetRaceHandler().PostLoadInit();
        }
    }
    
    // Supporting classes for race definition
    
    public class RaceProperties
    {
        public string raceName;
        public string raceDescription;
        public string raceIcon;
        
        public GeneDef coreGeneDef;
        public List<GeneDef> secondaryGeneDefs;
        
        public float lifeSpanYears = 80f;
        public float maxLifeSpanYears = 100f;
        public List<LifeStageProperties> lifeStages;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve references to GeneDefs
            if (lifeStages != null)
            {
                foreach (var stage in lifeStages)
                {
                    stage.ResolveReferences(parent);
                }
            }
        }
    }
    
    public class LifeStageProperties
    {
        public string stageID;
        public string stageName;
        public float minAgeYears;
        public float maxAgeYears;
        public float bodySizeFactor = 1f;
        public float healthScaleFactor = 1f;
        public float resourceGainFactor = 1f;
        public bool abilitiesUnlocked = true;
        public List<HediffDef> stageHediffs;
        public List<StatModifier> statFactors;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve references to HediffDefs and StatDefs
            if (stageHediffs != null)
            {
                for (int i = 0; i < stageHediffs.Count; i++)
                {
                    if (stageHediffs[i] == null)
                    {
                        Log.Error($"Null hediff in life stage {stageID} for race {parent.defName}");
                    }
                }
            }
            
            if (statFactors != null)
            {
                for (int i = 0; i < statFactors.Count; i++)
                {
                    var statFactor = statFactors[i];
                    if (statFactor.statDef == null)
                    {
                        Log.Error($"Null stat def in life stage {stageID} for race {parent.defName}");
                    }
                }
            }
        }
    }
    
    public class StatModifier
    {
        public StatDef statDef;
        public float value = 1f;
    }
    
    public class RaceResourceDef
    {
        public string resourceID;
        public string resourceName;
        public string resourceDescription;
        public float minValue = 0f;
        public float maxValue = 100f;
        public float defaultValue = 50f;
        public bool isVisible = true;
        public float regenerationRate = 0f;
        public string iconPath;
        public string category;
        public List<ResourceLevelHediff> resourceLevelHediffs;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve references to HediffDefs for resource levels
            if (resourceLevelHediffs != null)
            {
                foreach (var levelHediff in resourceLevelHediffs)
                {
                    if (levelHediff.hediff == null)
                    {
                        Log.Error($"Null hediff for resource level in resource {resourceID} for race {parent.defName}");
                    }
                }
            }
        }
    }
    
    public class ResourceLevelHediff
    {
        public float level;
        public HediffDef hediff;
    }
    
    public class RaceAbilityDef
    {
        public string abilityID;
        public string abilityName;
        public string abilityDescription;
        public string iconPath;
        public int cooldownTicks = 60000; // 1 day
        public bool isPassive = false;
        public List<ResourceCost> resourceCosts;
        public List<StatRequirement> statRequirements;
        public List<HediffDef> hediffsApplied;
        public float effectRadius = 0f;
        public string targetingType = "SingleTarget";
        public string abilityClass;
        public string category = "General";
        public string soundDefName;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve references to HediffDefs and other defs
            if (hediffsApplied != null)
            {
                for (int i = 0; i < hediffsApplied.Count; i++)
                {
                    if (hediffsApplied[i] == null)
                    {
                        Log.Error($"Null hediff in ability {abilityID} for race {parent.defName}");
                    }
                }
            }
            
            if (statRequirements != null)
            {
                foreach (var req in statRequirements)
                {
                    if (req.statDef == null)
                    {
                        Log.Error($"Null stat def in ability {abilityID} for race {parent.defName}");
                    }
                }
            }
        }
    }
    
    public class ResourceCost
    {
        public string resourceID;
        public float cost;
    }
    
    public class StatRequirement
    {
        public StatDef statDef;
        public float value;
    }
    
    public class SpecialMechanics
    {
        public List<EnvironmentalAdaptation> environmentalAdaptations;
        public List<SpecialNeedDef> specialNeeds;
        public List<UniqueInteraction> uniqueInteractions;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve references in mechanics
            if (environmentalAdaptations != null)
            {
                foreach (var adaptation in environmentalAdaptations)
                {
                    // Validate and resolve references
                }
            }
            
            if (specialNeeds != null)
            {
                foreach (var need in specialNeeds)
                {
                    // Validate and resolve references
                }
            }
        }
    }
    
    public class EnvironmentalAdaptation
    {
        public string environmentType;
        public List<StatModifier> statModifications;
        public List<NeedModification> needModifications;
    }
    
    public class NeedModification
    {
        public NeedDef needDef;
        public float factor = 1f;
    }
    
    public class SpecialNeedDef
    {
        public string needID;
        public string needName;
        public string needDescription;
        public float minLevel = 0f;
        public float maxLevel = 100f;
        public float defaultLevel = 50f;
        public float fallRate = 0.15f;
        public float criticalThreshold = 10f;
        public string iconPath;
        public HediffDef criticalHediff;
    }
    
    public class UniqueInteraction
    {
        public string interactionID;
        public string interactionName;
        public string interactionDescription;
        public List<string> targetsAllowed;
        public string interactionClass;
    }
    
    public class VEFIntegration
    {
        public bool isUsingVEF = true;
        public VEFGeneExtension vefGeneExtension;
        public List<ConditionalStatModifier> conditionalStatModifiers;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve VEF references
            if (vefGeneExtension != null)
            {
                // Validate and resolve references
            }
            
            if (conditionalStatModifiers != null)
            {
                foreach (var modifier in conditionalStatModifiers)
                {
                    // Validate and resolve references
                }
            }
        }
    }
    
    public class VEFGeneExtension
    {
        public string backgroundPathEndogenes;
        public HediffDef hediffToWholeBody;
        public ThingDef customBloodThingDef;
        public string customBloodIcon;
        public float diseaseProgressionFactor = 1f;
        public float pregnancySpeedFactor = 1f;
    }
    
    public class ConditionalStatModifier
    {
        public string conditionType;
        public List<StatModifier> statFactors;
        public List<StatModifier> statOffsets;
    }
    
    public class HARIntegration
    {
        public bool isUsingHAR = false;
        public List<string> bodyTypes;
        public string fallbackBodyType = "Thin";
        public Vector2 customDrawSize = new Vector2(1f, 1f);
        public object alienPartGenerator; // This will need special handling
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve HAR references if HAR is available
        }
    }
    
    public class CEIntegration
    {
        public bool isUsingCE = false;
        public List<CEStatModifier> ceStats;
        
        public void ResolveReferences(LegendaryRaceDef parent)
        {
            // Resolve CE references if CE is available
        }
    }
    
    public class CEStatModifier
    {
        public string statName;
        public float value;
    }
    
    public class BalanceSettings
    {
        public int powerLevel = 3; // 1-5 scale
        public float resourceMultiplier = 1f;
        public float abilityPowerMultiplier = 1f;
        public bool difficultyScaling = true;
    }
    
    public class PerformanceSettings
    {
        public bool usePerformanceMonitoring = true;
        public int resourceUpdateInterval = 250; // Ticks
        public int abilityCheckInterval = 60; // Ticks
        public int environmentalCheckInterval = 250; // Ticks
    }
}