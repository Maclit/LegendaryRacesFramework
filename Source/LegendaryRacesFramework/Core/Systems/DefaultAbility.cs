using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    public class DefaultAbility : ISpecialAbility
    {
        private readonly RaceAbilityDef abilityDef;
        private readonly Dictionary<Pawn, int> cooldowns = new Dictionary<Pawn, int>();
        
        public DefaultAbility(RaceAbilityDef abilityDef)
        {
            this.abilityDef = abilityDef;
            
            // Register for game tick to update cooldowns
            LRF_GameComponent.RegisterForTick(OnTick);
        }
        
        public string AbilityID => abilityDef.abilityID;
        
        public string AbilityName => abilityDef.abilityName;
        
        public string AbilityDescription => abilityDef.abilityDescription;
        
        public string IconPath => abilityDef.iconPath;
        
        public int CooldownTicks => abilityDef.cooldownTicks;
        
        public bool IsPassive => abilityDef.isPassive;
        
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
        
        private void OnTick()
        {
            // Update cooldowns
            foreach (var pawn in cooldowns.Keys.ToList())
            {
                if (pawn == null || pawn.Destroyed || pawn.Dead)
                {
                    cooldowns.Remove(pawn);
                    continue;
                }
                
                int currentCooldown = cooldowns[pawn];
                if (currentCooldown > 0)
                {
                    cooldowns[pawn] = currentCooldown - 1;
                }
            }
            
            // Handle passive abilities
            if (IsPassive)
            {
                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
                {
                    // Check if pawn can use this ability
                    if (CanUseAbility(pawn))
                    {
                        // For passive abilities, only try to use if not on cooldown
                        if (GetCurrentCooldown(pawn) <= 0)
                        {
                            UseAbility(pawn, pawn);
                        }
                    }
                }
            }
        }
        
        public bool CanUseAbility(Pawn pawn)
        {
            if (pawn == null || pawn.Dead || pawn.Downed)
                return false;
            
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
        
        public bool UseAbility(Pawn pawn, LocalTargetInfo target)
        {
            if (!CanUseAbility(pawn))
                return false;
            
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
            cooldowns[pawn] = CooldownTicks;
            
            return true;
        }
        
        public int GetCurrentCooldown(Pawn pawn)
        {
            if (pawn == null)
                return 0;
            
            if (cooldowns.TryGetValue(pawn, out int cooldown))
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
                
                switch (abilityDef.targetingType?.ToLowerInvariant())
                {
                    case "singletarget":
                        if (target.Thing is Pawn targetPawn)
                        {
                            targets.Add(targetPawn);
                        }
                        break;
                        
                    case "self":
                        targets.Add(pawn);
                        break;
                        
                    case "multitarget":
                    case "aoe":
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
                        
                    default:
                        targets.Add(pawn); // Default to self
                        break;
                }
                
                // Apply hediffs to targets
                foreach (Pawn targetPawn in targets)
                {
                    foreach (HediffDef hediffDef in abilityDef.hediffsApplied)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(hediffDef, targetPawn);
                        targetPawn.health.AddHediff(hediff);
                    }
                }
                
                // Create visual effect
                FleckMaker.ThrowLightningGlow(target.Cell.ToVector3Shifted(), pawn.Map, 1.5f);
            }
        }
    }
}