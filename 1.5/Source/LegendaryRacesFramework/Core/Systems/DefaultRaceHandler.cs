using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    public class DefaultRaceHandler : IRaceExtension
    {
        private readonly LegendaryRaceDef raceDef;
        private IResourceManager resourceManager;
        private IRaceLifeCycle lifeCycleManager;
        private List<ISpecialAbility> abilities = new List<ISpecialAbility>();
        private IBalanceMetrics balanceMetrics;
        private IPerformanceMonitor performanceMonitor;
        
        // Cache for race membership lookups (RimWorld 1.5 optimization)
        private Dictionary<int, bool> raceMembershipCache = new Dictionary<int, bool>();
        private int raceMembershipCacheTick = 0;
        private const int CACHE_VALIDITY_TICKS = 2500; // Cache validity period
        
        public DefaultRaceHandler(LegendaryRaceDef raceDef)
        {
            this.raceDef = raceDef;
        }
        
        public string RaceID => raceDef.defName;
        
        public string RaceName => raceDef.raceProperties.raceName;
        
        public string RaceDescription => raceDef.raceProperties.raceDescription;
        
        public GeneDef CoreGeneDef => raceDef.raceProperties.coreGeneDef;
        
        public List<GeneDef> SecondaryGeneDefs => raceDef.raceProperties.secondaryGeneDefs ?? new List<GeneDef>();
        
        public IResourceManager ResourceManager => resourceManager;
        
        public List<ISpecialAbility> RaceAbilities => abilities;
        
        public IRaceLifeCycle LifeCycleManager => lifeCycleManager;
        
        public List<ILegendaryCharacter> LegendaryCharacters { get; set; } = new List<ILegendaryCharacter>();
        
        public IBalanceMetrics BalanceMetrics => balanceMetrics;
        
        public void Initialize()
        {
            // Start performance monitoring for initialization
            if (LegendaryRacesFrameworkMod.Settings.enablePerformanceMonitoring)
            {
                performanceMonitor = new DefaultPerformanceMonitor(RaceID, raceDef.performanceSettings);
                performanceMonitor.StartTiming("Initialization");
            }
            
            try
            {
                // Create resource manager
                resourceManager = new DefaultResourceManager(RaceID, raceDef.raceResources);
                
                // Create life cycle manager
                lifeCycleManager = new DefaultLifeCycleManager(RaceID, raceDef.raceProperties);
                
                // Create abilities
                if (raceDef.raceAbilities != null)
                {
                    foreach (var abilityDef in raceDef.raceAbilities)
                    {
                        ISpecialAbility ability = CreateAbilityInstance(abilityDef);
                        if (ability != null)
                        {
                            abilities.Add(ability);
                        }
                    }
                }
                
                // Create balance metrics
                balanceMetrics = new DefaultBalanceMetrics(RaceID, raceDef.balanceSettings);
                
                // Log successful initialization
                Log.Message($"Initialized race handler for {RaceName} ({RaceID})");
            }
            catch (Exception ex)
            {
                Log.Error($"Error initializing race handler for {RaceID}: {ex}");
            }
            finally
            {
                // Stop performance timing
                if (performanceMonitor != null)
                {
                    performanceMonitor.StopTiming("Initialization");
                }
            }
            
            // Register for game init complete
            LRF_GameComponent.RegisterForSlowTick(OnSlowTick);
        }
        
        public void PostLoadInit()
        {
            // Any post-load initialization
            
            // Find legendary characters for this race
            LegendaryCharacters = LegendaryCharacterManager.GetLegendaryCharactersForRace(RaceID);
            
            // Initialize VEF integration if configured
            if (raceDef.vefIntegration?.isUsingVEF == true && ModsConfig.IsActive("OskarPotocki.VanillaExpandedFramework"))
            {
                InitializeVEFIntegration();
            }
            
            // Initialize HAR integration if configured
            if (raceDef.harIntegration?.isUsingHAR == true && ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
            {
                InitializeHARIntegration();
            }
            
            // Initialize CE integration if configured
            if (raceDef.ceIntegration?.isUsingCE == true && ModsConfig.IsActive("CETeam.CombatExtended"))
            {
                InitializeCEIntegration();
            }
            
            // Clear caches on load
            ClearCaches();
        }
        
        private void OnSlowTick()
        {
            // Clear the membership cache periodically
            int currentTick = Find.TickManager.TicksGame;
            if (currentTick - raceMembershipCacheTick > CACHE_VALIDITY_TICKS)
            {
                ClearCaches();
                raceMembershipCacheTick = currentTick;
            }
        }
        
        private void ClearCaches()
        {
            raceMembershipCache.Clear();
        }
        
        public bool IsMemberOfRace(Pawn pawn)
        {
            if (pawn == null) return false;
            
            int pawnID = pawn.thingIDNumber;
            
            // Check cache first
            if (raceMembershipCache.TryGetValue(pawnID, out bool isMember))
            {
                return isMember;
            }
            
            // Race membership verification with timing
            if (performanceMonitor != null)
            {
                performanceMonitor.StartTiming("IsMemberOfRace");
            }
            
            try
            {
                // First, check for the race component which is definitive
                Comp_LegendaryRace raceComp = pawn.GetComp<Comp_LegendaryRace>();
                if (raceComp != null && raceComp.RaceDefName == RaceID)
                {
                    raceMembershipCache[pawnID] = true;
                    return true;
                }
                
                // Check for core gene
                if (ModsConfig.BiotechActive && pawn.genes != null && CoreGeneDef != null)
                {
                    if (pawn.genes.HasGene(CoreGeneDef))
                    {
                        raceMembershipCache[pawnID] = true;
                        return true;
                    }
                }
                
                // Check for race-specific hediffs
                if (raceDef.raceProperties?.lifeStages != null)
                {
                    foreach (var stage in raceDef.raceProperties.lifeStages)
                    {
                        if (stage.stageHediffs != null)
                        {
                            foreach (var hediffDef in stage.stageHediffs)
                            {
                                if (pawn.health.hediffSet.HasHediff(hediffDef))
                                {
                                    raceMembershipCache[pawnID] = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
                
                // Not a member of this race
                raceMembershipCache[pawnID] = false;
                return false;
            }
            finally
            {
                if (performanceMonitor != null)
                {
                    performanceMonitor.StopTiming("IsMemberOfRace");
                }
            }
        }
        
        public Dictionary<string, float> GetCustomRaceStats(Pawn pawn)
        {
            var stats = new Dictionary<string, float>();
            
            // Check if pawn is of this race
            if (!IsMemberOfRace(pawn)) return stats;
            
            // Get life stage modifiers
            var lifeStage = lifeCycleManager.GetCurrentLifeStage(pawn);
            if (lifeStage != null && lifeStage.StatFactors != null)
            {
                foreach (var statFactor in lifeStage.StatFactors)
                {
                    stats[statFactor.Key.defName] = statFactor.Value;
                }
            }
            
            // Get environmental adaptations
            if (raceDef.specialMechanics?.environmentalAdaptations != null)
            {
                foreach (var adaptation in raceDef.specialMechanics.environmentalAdaptations)
                {
                    // Check if environmental condition is met
                    if (IsEnvironmentalConditionMet(pawn, adaptation.environmentType))
                    {
                        foreach (var statMod in adaptation.statModifications)
                        {
                            string statDefName = statMod.statDef.defName;
                            if (stats.ContainsKey(statDefName))
                            {
                                stats[statDefName] *= statMod.value;
                            }
                            else
                            {
                                stats[statDefName] = statMod.value;
                            }
                        }
                    }
                }
            }
            
            return stats;
        }
        
        private bool IsEnvironmentalConditionMet(Pawn pawn, string environmentType)
        {
            if (pawn?.Map == null || string.IsNullOrEmpty(environmentType))
                return false;
            
            // Time the check for performance monitoring
            if (performanceMonitor != null)
            {
                performanceMonitor.StartTiming("EnvironmentalCheck");
            }
            
            try
            {
                // Check for various environmental conditions
                switch (environmentType.ToLowerInvariant())
                {
                    case "aquatic":
                        return pawn.Position.GetTerrain(pawn.Map)?.defName?.ToLowerInvariant().Contains("water") ?? false;
                        
                    case "darkness":
                        return pawn.Map.glowGrid.GameGlowAt(pawn.Position) < 0.5f;
                        
                    case "light":
                        return pawn.Map.glowGrid.GameGlowAt(pawn.Position) > 0.5f;
                        
                    case "indoors":
                        return pawn.GetRoom()?.PsychologicallyOutdoors == false;
                        
                    case "outdoors":
                        return pawn.GetRoom()?.PsychologicallyOutdoors == true;
                        
                    case "hot":
                        return pawn.AmbientTemperature > 40f;
                        
                    case "cold":
                        return pawn.AmbientTemperature < 0f;
                        
                    case "toxic":
                        return pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout);
                        
                    case "radiation":
                        return pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout);
                        
                    case "rain":
                        return pawn.Map.weatherManager.RainRate > 0.1f;
                        
                    case "snow":
                        return pawn.Map.weatherManager.SnowRate > 0.1f;
                        
                    default:
                        return false;
                }
            }
            finally
            {
                if (performanceMonitor != null)
                {
                    performanceMonitor.StopTiming("EnvironmentalCheck");
                }
            }
        }
        
        private ISpecialAbility CreateAbilityInstance(RaceAbilityDef abilityDef)
        {
            if (string.IsNullOrEmpty(abilityDef.abilityClass)) return null;
            
            try
            {
                // Try to get a registered ability type
                Type abilityType = LegendaryRacesFrameworkMod.GetRegisteredAbilityType(abilityDef.abilityClass);
                
                // If not registered, try to get by full type name
                if (abilityType == null)
                {
                    abilityType = Type.GetType(abilityDef.abilityClass);
                }
                
                // Create instance if type is valid
                if (abilityType != null && typeof(ISpecialAbility).IsAssignableFrom(abilityType))
                {
                    return (ISpecialAbility)Activator.CreateInstance(abilityType, abilityDef);
                }
                else
                {
                    Log.Error($"Could not create ability instance for {abilityDef.abilityName}: Type {abilityDef.abilityClass} not found or not implementing ISpecialAbility");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating ability instance for {abilityDef.abilityName}: {ex}");
            }
            
            // Fallback to default ability implementation
            return new DefaultAbility(abilityDef);
        }
        
        // New methods for RimWorld 1.5 mod integration
        private void InitializeVEFIntegration()
        {
            // Implementation will depend on VEF specifics
            if (LegendaryRacesFrameworkMod.Settings.showDebugLogs)
            {
                Log.Message($"Initializing VEF integration for race {RaceID}");
            }
        }
        
        private void InitializeHARIntegration()
        {
            // Implementation will depend on HAR specifics
            if (LegendaryRacesFrameworkMod.Settings.showDebugLogs)
            {
                Log.Message($"Initializing HAR integration for race {RaceID}");
            }
        }
        
        private void InitializeCEIntegration()
        {
            // Implementation will depend on CE specifics
            if (LegendaryRacesFrameworkMod.Settings.showDebugLogs)
            {
                Log.Message($"Initializing CE integration for race {RaceID}");
            }
        }
    }
}