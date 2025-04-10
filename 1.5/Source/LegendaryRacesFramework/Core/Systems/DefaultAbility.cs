using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using System;
using Verse.Sound;

namespace LegendaryRacesFramework
{
    public class DefaultAbility : ISpecialAbility
    {
        private readonly RaceAbilityDef abilityDef;
        private readonly Dictionary<int, int> cooldowns = new Dictionary<int, int>();
        private readonly Dictionary<int, int> lastUsedTicks = new Dictionary<int, int>();
        
        private IPerformanceMonitor performanceMonitor;
        
        public DefaultAbility(RaceAbilityDef abilityDef)
        {
            this.abilityDef = abilityDef;
            
            // Set up performance monitoring
            if (LegendaryRacesFrameworkMod.Settings.enablePerformanceMonitoring)
            {
                performanceMonitor = new DefaultPerformanceMonitor(abilityDef.abilityID, null);
            }
            
            // Register for game tick to update cooldowns
            LRF_GameComponent.RegisterForTick(OnTick);
        }
        
        public string AbilityID => abilityDef.abilityID;
        
        public string AbilityName => abilityDef.abilityName;
        
        public string AbilityDescription => abilityDef.abilityDescription;
        
        public string IconPath => abilityDef.iconPath;
        
        public int CooldownTicks => abilityDef.cooldownTicks;
        
        public bool IsPassive => abilityDef.isPassive;
        
        // Implement the TargetingType property required by the interface
        public TargetingType TargetingType => ParseTargetingType(abilityDef.targetingType);
        
        public Dictionary<string, float> ResourceCosts
        {
            get
            {
                Dictionary<string, float> costs = new Dictionary<string, float>();
                if (abilityDef.resourceCosts != null)
                {
                    foreach (var cost in abilityDef.resourceCosts)
                    {
                        costs[cost.resourceID] = cost.cost;
                    }
                }
                return costs;
            }
        }
        
        public Dictionary<StatDef, float> StatRequirements
        {
            get
            {
                Dictionary<StatDef, float> requirements = new Dictionary<StatDef, float>();
                if (abilityDef.statRequirements != null)
                {
                    foreach (var req in abilityDef.statRequirements)
                    {
                        requirements[req.statDef] = req.value;
                    }
                }
                return requirements;
            }
        }
        
        private TargetingType ParseTargetingType(string targetingTypeStr)
        {
            if (string.IsNullOrEmpty(targetingTypeStr))
                return TargetingType.Self;
                
            switch (targetingTypeStr.ToLowerInvariant())
            {
                case "self":
                    return TargetingType.Self;
                case "singletarget":
                    return TargetingType.SingleTarget;
                case "multitarget":
                    return TargetingType.MultiTarget;
                case "aoe":
                    return TargetingType.AoE;
                default:
                    return TargetingType.Self;
            }
        }
        
        private void OnTick()
        {
            // Update cooldowns
            foreach (var pawnId in cooldowns.Keys.ToList())
            {
                int currentCooldown = cooldowns[pawnId];
                if (currentCooldown > 0)
                {
                    cooldowns[pawnId] = currentCooldown - 1;
                }
            }
            
            // Handle passive abilities
            if (IsPassive)
            {
                int currentTick = Find.TickManager.TicksGame;
                
                foreach (Map map in Find.Maps)
                {
                    foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        // Skip if we've already handled this pawn recently
                        if (lastUsedTicks.TryGetValue(pawn.thingIDNumber, out int lastUsed) && 
                            currentTick - lastUsed < 250) // Only check every 250 ticks for passive abilities
                            continue;
                            
                        // Check if pawn can use this ability
                        if (CanUseAbility(pawn))
                        {
                            // For passive abilities, only try to use if not on cooldown
                            if (GetCurrentCooldown(pawn) <= 0)
                            {
                                UseAbility(pawn, pawn);
                                lastUsedTicks[pawn.thingIDNumber] = currentTick;
                            }
                        }
                    }
                }
            }
        }
        
        public bool CanUseAbility(Pawn pawn)
        {
            if (pawn == null || pawn.Dead || pawn.Downed)
                return false;
            
            // Start timing for performance monitoring
            if (performanceMonitor != null)
            {
                performanceMonitor.StartTiming("CanUseAbility");
            }
            
            try
            {
                // Check if pawn is of the correct race
                var raceComp = pawn.GetComp<Comp_LegendaryRace>();
                if (raceComp == null || raceComp.RaceHandler == null)
                    return false;
                
                // Check if pawn is in a life stage that can use abilities
                var lifeCycle = raceComp.RaceHandler.LifeCycleManager;
                var lifeStage = lifeCycle?.GetCurrentLifeStage(pawn);
                if (lifeStage == null || !lifeStage.AbilitiesUnlocked)
                    return false;
                
                // Check cooldown
                if (GetCurrentCooldown(pawn) > 0)
                    return false;
                
                // Check stat requirements
                foreach (var requirement in StatRequirements)
                {
                    float statValue = pawn.GetStatValue(requirement.Key);
                    if (statValue < requirement.Value)
                    {
                        return false;
                    }
                }
                
                // Check resource costs
                var resourceManager = raceComp.RaceHandler.ResourceManager;
                if (resourceManager != null)
                {
                    foreach (var cost in ResourceCosts)
                    {
                        float resourceValue = resourceManager.GetResourceValue(pawn, cost.Key);
                        if (resourceValue < cost.Value)
                        {
                            return false;
                        }
                    }
                }
                
                return true;
            }
            finally
            {
                // Stop timing for performance monitoring
                if (performanceMonitor != null)
                {
                    performanceMonitor.StopTiming("CanUseAbility");
                }
            }
        }
        
        public bool UseAbility(Pawn pawn, LocalTargetInfo target)
        {
            if (!CanUseAbility(pawn))
                return false;
            
            // Start timing for performance monitoring
            if (performanceMonitor != null)
            {
                performanceMonitor.StartTiming("UseAbility");
            }
            
            try
            {
                // Get race resource manager
                var raceComp = pawn.GetComp<Comp_LegendaryRace>();
                var resourceManager = raceComp?.RaceHandler?.ResourceManager;
                
                // Consume resources
                if (resourceManager != null)
                {
                    foreach (var cost in ResourceCosts)
                    {
                        resourceManager.AdjustResourceValue(pawn, cost.Key, -cost.Value);
                    }
                }
                
                // Apply ability effects
                ApplyAbilityEffects(pawn, target);
                
                // Set cooldown
                cooldowns[pawn.thingIDNumber] = CooldownTicks;
                
                // Record usage for balance metrics
                if (raceComp?.RaceHandler?.BalanceMetrics != null)
                {
                    raceComp.RaceHandler.BalanceMetrics.RecordMetric("AbilityUses", 1);
                }
                
                return true;
            }
            finally
            {
                // Stop timing for performance monitoring
                if (performanceMonitor != null)
                {
                    performanceMonitor.StopTiming("UseAbility");
                }
            }
        }
        
        public int GetCurrentCooldown(Pawn pawn)
        {
            if (pawn == null)
                return 0;
            
            int pawnId = pawn.thingIDNumber;
            
            if (cooldowns.TryGetValue(pawnId, out int cooldown))
            {
                return cooldown;
            }
            
            return 0;
        }
        
        protected virtual void ApplyAbilityEffects(Pawn pawn, LocalTargetInfo target)
        {
            // Apply hediffs if specified
            if (abilityDef.hediffsApplied != null && abilityDef.hediffsApplied.Count > 0)
            {
                // Determine targets based on targeting type
                List<Pawn> targets = new List<Pawn>();
                
                switch (TargetingType)
                {
                    case TargetingType.Self:
                        targets.Add(pawn);
                        break;
                        
                    case TargetingType.SingleTarget:
                        if (target.Thing is Pawn targetPawn)
                        {
                            targets.Add(targetPawn);
                        }
                        break;
                        
                    case TargetingType.MultiTarget:
                    case TargetingType.AoE:
                        if (abilityDef.effectRadius > 0f && target.Cell.IsValid && pawn.Map != null)
                        {
                            IEnumerable<Thing> thingsInRange = GenRadial.RadialDistinctThingsAround(
                                target.Cell, pawn.Map, abilityDef.effectRadius, true);
                            
                            foreach (Thing thing in thingsInRange)
                            {
                                if (thing is Pawn targetPawnInRange)
                                {
                                    targets.Add(targetPawnInRange);
                                }
                            }
                        }
                        break;
                }
                
                // Apply hediffs to targets
                foreach (Pawn targetPawn in targets)
                {
                    foreach (HediffDef hediffDef in abilityDef.hediffsApplied)
                    {
                        Hediff existingHediff = targetPawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                        
                        if (existingHediff != null)
                        {
                            // Increase severity of existing hediff
                            existingHediff.Severity += 0.2f;
                        }
                        else
                        {
                            // Add new hediff
                            Hediff hediff = HediffMaker.MakeHediff(hediffDef, targetPawn);
                            hediff.Severity = 0.5f; // Start at medium severity
                            targetPawn.health.AddHediff(hediff);
                        }
                    }
                }
                
                // Create visual effect
                if (target.Cell.IsValid && pawn.Map != null)
                {
                    // Effect type based on ability category (can be expanded)
                    if (abilityDef.category == "Healing")
                    {
                        FleckMaker.ThrowLightningGlow(target.Cell.ToVector3Shifted(), pawn.Map, 1.5f);
                        FleckMaker.AttachedOverlay(pawn, FleckDefOf.HealingCross, Vector3.zero);
                    }
                    else if (abilityDef.category == "Damage")
                    {
                        FleckMaker.Static(target.Cell.ToVector3Shifted(), pawn.Map, FleckDefOf.ExplosionFlash);
                    }
                    else
                    {
                        FleckMaker.ThrowDustPuff(target.Cell.ToVector3Shifted(), pawn.Map, 2f);
                    }
                }
                
                // Play sound if specified
                if (!string.IsNullOrEmpty(abilityDef.soundDefName))
                {
                    SoundDef soundDef = DefDatabase<SoundDef>.GetNamed(abilityDef.soundDefName, false);
                    if (soundDef != null)
                    {
                        soundDef.PlayOneShot(SoundInfo.InMap(new TargetInfo(target.Cell, pawn.Map)));
                    }
                }
            }
        }
    }
}