using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    public class DefaultLifeCycleManager : IRaceLifeCycle
    {
        private readonly string raceID;
        private readonly RaceProperties raceProperties;
        private readonly List<RaceLifeStage> lifeStages = new List<RaceLifeStage>();
        private readonly Dictionary<Pawn, string> currentLifeStages = new Dictionary<Pawn, string>();
        
        public string RaceID => raceID;
        
        public List<RaceLifeStage> LifeStages => lifeStages;
        
        public float AverageLifespanYears => raceProperties?.lifeSpanYears ?? 80f;
        
        public float MaximumLifespanYears => raceProperties?.maxLifeSpanYears ?? 100f;
        
        public DefaultLifeCycleManager(string raceID, RaceProperties raceProperties)
        {
            this.raceID = raceID;
            this.raceProperties = raceProperties;
            
            // Convert life stage defs to runtime life stages
            if (raceProperties?.lifeStages != null)
            {
                foreach (var stageDef in raceProperties.lifeStages)
                {
                    var stage = new RaceLifeStage
                    {
                        StageID = stageDef.stageID,
                        StageName = stageDef.stageName,
                        MinAgeYears = stageDef.minAgeYears,
                        MaxAgeYears = stageDef.maxAgeYears,
                        BodySizeFactor = stageDef.bodySizeFactor,
                        HealthScaleFactor = stageDef.healthScaleFactor,
                        ResourceGainFactor = stageDef.resourceGainFactor,
                        AbilitiesUnlocked = stageDef.abilitiesUnlocked,
                        StageHediffs = stageDef.stageHediffs?.ToList() ?? new List<HediffDef>(),
                        StatFactors = stageDef.statFactors?.ToDictionary(
                            x => x.statDef,
                            x => x.value
                        ) ?? new Dictionary<StatDef, float>()
                    };
                    
                    lifeStages.Add(stage);
                }
            }
            
            // Sort life stages by age
            lifeStages = lifeStages.OrderBy(x => x.MinAgeYears).ToList();
            
            // Register for game tick to update life stages
            LRF_GameComponent.RegisterForTick(OnTick);
        }
        
        private void OnTick()
        {
            // Check for life stage transitions periodically (once per day)
            if (Find.TickManager.TicksGame % GenDate.TicksPerDay != 0)
                return;
            
            // Cache pawns that need their life stage tracked
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists)
            {
                // Skip if pawn is not of this race
                if (!IsRaceMember(pawn))
                    continue;
                
                RaceLifeStage currentStage = GetCurrentLifeStage(pawn);
                if (currentStage == null)
                    continue;
                
                // Store current stage ID
                string previousStageID = null;
                currentLifeStages.TryGetValue(pawn, out previousStageID);
                
                // Check for life stage transition
                if (previousStageID != currentStage.StageID)
                {
                    RaceLifeStage previousStage = null;
                    if (!string.IsNullOrEmpty(previousStageID))
                    {
                        previousStage = lifeStages.FirstOrDefault(s => s.StageID == previousStageID);
                    }
                    
                    // Handle life stage transition
                    HandleLifeStageTransition(pawn, previousStage, currentStage);
                    
                    // Update stored stage
                    currentLifeStages[pawn] = currentStage.StageID;
                }
            }
        }
        
        private bool IsRaceMember(Pawn pawn)
        {
            if (pawn == null)
                return false;
            
            // Look up the race handler for this race
            var handler = DefDatabase<LegendaryRaceDef>.GetNamed(raceID)?.GetRaceHandler();
            if (handler == null)
                return false;
            
            return handler.IsMemberOfRace(pawn);
        }
        
        public RaceLifeStage GetCurrentLifeStage(Pawn pawn)
        {
            if (pawn == null || lifeStages.Count == 0)
                return null;
            
            // Calculate pawn's age in years
            float ageYears = pawn.ageTracker.AgeBiologicalYearsFloat;
            
            // Find the life stage for this age
            for (int i = lifeStages.Count - 1; i >= 0; i--)
            {
                var stage = lifeStages[i];
                if (ageYears >= stage.MinAgeYears)
                {
                    return stage;
                }
            }
            
            // Default to first life stage if none match
            return lifeStages.FirstOrDefault();
        }
        
        public float GetAgingFactor(Pawn pawn)
        {
            if (pawn == null)
                return 1f;
            
            // Get current life stage for aging factor
            RaceLifeStage currentStage = GetCurrentLifeStage(pawn);
            if (currentStage == null)
                return 1f;
            
            // Apply aging factor based on age relative to race's lifespan
            float ageYears = pawn.ageTracker.AgeBiologicalYearsFloat;
            float lifespanYears = AverageLifespanYears;
            
            // Age more slowly in early years, and more rapidly as pawn approaches maximum lifespan
            if (ageYears < lifespanYears * 0.8f)
            {
                return 0.9f;
            }
            else if (ageYears > lifespanYears)
            {
                return 1.5f; // Age more rapidly past normal lifespan
            }
            
            return 1f;
        }
        
        public void HandleLifeStageTransition(Pawn pawn, RaceLifeStage fromStage, RaceLifeStage toStage)
        {
            if (pawn == null || toStage == null)
                return;
            
            // Send notification of life stage transition
            if (pawn.Faction != null && pawn.Faction.IsPlayer && fromStage != null)
            {
                Messages.Message($"{pawn.Name} has entered the {toStage.StageName} stage of life.", 
                    pawn, MessageTypeDefOf.PositiveEvent);
            }
            
            // Remove hediffs from previous stage
            if (fromStage != null && fromStage.StageHediffs != null)
            {
                foreach (var hediffDef in fromStage.StageHediffs)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (hediff != null)
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }
            }
            
            // Apply hediffs for new stage
            ApplyLifeStageHediffs(pawn, toStage);
        }
        
        public void ApplyLifeStageHediffs(Pawn pawn, RaceLifeStage lifeStage)
        {
            if (pawn == null || lifeStage == null || lifeStage.StageHediffs == null)
                return;
            
            foreach (var hediffDef in lifeStage.StageHediffs)
            {
                if (!pawn.health.hediffSet.HasHediff(hediffDef))
                {
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
                    pawn.health.AddHediff(hediff);
                }
            }
        }
    }
}