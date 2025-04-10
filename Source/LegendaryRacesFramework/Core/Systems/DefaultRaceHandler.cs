using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

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
            
            // Create performance monitor
            performanceMonitor = new DefaultPerformanceMonitor(RaceID, raceDef.performanceSettings);
            
            // Log successful initialization
            Log.Message($"Initialized race handler for {RaceName} ({RaceID})");
        }
        
        public void PostLoadInit()
        {
            // Any post-load initialization
            
            // Find legendary characters for this race
            LegendaryCharacters = LegendaryCharacterManager.GetLegendaryCharactersForRace(RaceID);
        }
        
        public bool IsMemberOfRace(Pawn pawn)
        {
            if (pawn == null) return false;
            
            // Check for core gene
            if (ModsConfig.BiotechActive && pawn.genes != null && CoreGeneDef != null)
            {
                if (pawn.genes.HasGene(CoreGeneDef))
                {
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
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
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
            // Implement environment condition checking
            switch (environmentType)
            {
                case "Aquatic":
                    return pawn.Position.GetTerrain(pawn.Map)?.defName?.Contains("Water") ?? false;
                case "Darkness":
                    return pawn.Map?.glowGrid.GameGlowAt(pawn.Position) < 0.5f;
                case "Light":
                    return pawn.Map?.glowGrid.GameGlowAt(pawn.Position) > 0.5f;
                default:
                    return false;
            }
        }
        
        private ISpecialAbility CreateAbilityInstance(RaceAbilityDef abilityDef)
        {
            if (string.IsNullOrEmpty(abilityDef.abilityClass)) return null;
            
            try
            {
                // Try to create an instance of the specified ability class
                Type abilityType = Type.GetType(abilityDef.abilityClass);
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
    }
}