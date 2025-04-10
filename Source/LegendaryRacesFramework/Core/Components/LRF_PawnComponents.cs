// File path: Source/LegendaryRacesFramework/Core/Components/LRF_PawnComponents.cs
using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    /// <summary>
    /// Component for tracking race information on pawns
    /// </summary>
    public class Comp_LegendaryRace : ThingComp
    {
        private string raceDefName;
        private IRaceExtension raceHandler;
        private int lastRaceDetectionTick = -1;
        private const int RACE_DETECTION_INTERVAL = 250; // Ticks between race detection attempts
        
        public string RaceDefName
        {
            get => raceDefName;
            set
            {
                raceDefName = value;
                raceHandler = null; // Clear cached handler when race changes
            }
        }
        
        public IRaceExtension RaceHandler
        {
            get
            {
                if (raceHandler == null && !string.IsNullOrEmpty(raceDefName))
                {
                    // Look up race handler
                    LegendaryRaceDef raceDef = DefDatabase<LegendaryRaceDef>.GetNamed(raceDefName, false);
                    if (raceDef != null)
                    {
                        raceHandler = raceDef.GetRaceHandler();
                    }
                }
                return raceHandler;
            }
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref raceDefName, "raceDefName");
            Scribe_Values.Look(ref lastRaceDetectionTick, "lastRaceDetectionTick", -1);
        }
        
        public override void CompTick()
        {
            base.CompTick();
            
            int currentTick = Find.TickManager.TicksGame;
            
            // Auto-detect race if not set (with rate limiting for RimWorld 1.5 performance)
            if (string.IsNullOrEmpty(raceDefName) && parent is Pawn pawn && 
                (lastRaceDetectionTick < 0 || currentTick - lastRaceDetectionTick >= RACE_DETECTION_INTERVAL))
            {
                lastRaceDetectionTick = currentTick;
                DetectRace(pawn);
            }
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // Only show gizmos for player faction pawns
            if (parent is Pawn pawn && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                // Add race-specific gizmos based on abilities
                if (!string.IsNullOrEmpty(raceDefName) && RaceHandler != null)
                {
                    // Get available abilities
                    foreach (var ability in RaceHandler.RaceAbilities)
                    {
                        if (ability.IsPassive)
                            continue; // Skip passive abilities
                            
                        if (!ability.CanUseAbility(pawn))
                            continue; // Skip abilities that can't be used
                            
                        int cooldown = ability.GetCurrentCooldown(pawn);
                        
                        Command_Action command = new Command_Action
                        {
                            defaultLabel = ability.AbilityName,
                            defaultDesc = ability.AbilityDescription + (cooldown > 0 ? $"\n\nCooldown: {cooldown / 60f:F1}s" : ""),
                            icon = ContentFinder<Texture2D>.Get(ability.IconPath, false) ?? BaseContent.BadTex,
                            action = delegate { UseAbility(pawn, ability); }
                        };
                        
                        // Disable if on cooldown
                        if (cooldown > 0)
                        {
                            command.Disable($"On cooldown: {cooldown / 60f:F1}s");
                        }
                        
                        yield return command;
                    }
                }
            }
        }
        
        private void UseAbility(Pawn pawn, ISpecialAbility ability)
        {
            if (pawn == null || ability == null)
                return;
                
            // For abilities requiring a target
            if (ability.TargetingType != TargetingType.Self)
            {
                Find.Targeter.BeginTargeting(new TargetingParameters
                {
                    canTargetPawns = true,
                    canTargetBuildings = ability.TargetingType.HasFlag(TargetingType.Buildings),
                    canTargetItems = ability.TargetingType.HasFlag(TargetingType.Items),
                    canTargetLocations = ability.TargetingType.HasFlag(TargetingType.Locations)
                }, 
                delegate(LocalTargetInfo target) 
                {
                    // Once target is selected, use the ability
                    if (ability.UseAbility(pawn, target))
                    {
                        // Notify success
                        Messages.Message(
                            string.Format("LRF.AbilityActivated".Translate(), pawn.LabelCap, ability.AbilityName),
                            pawn,
                            MessageTypeDefOf.PositiveEvent);
                    }
                }, 
                null, // Mouse attachment texture 
                true); // Play sound
            }
            else
            {
                // Self-targeted ability
                if (ability.UseAbility(pawn, pawn))
                {
                    // Notify success
                    Messages.Message(
                        string.Format("LRF.AbilityActivated".Translate(), pawn.LabelCap, ability.AbilityName),
                        pawn,
                        MessageTypeDefOf.PositiveEvent);
                }
            }
        }
        
        private void DetectRace(Pawn pawn)
        {
            // Check each race definition to see if the pawn is a member
            foreach (LegendaryRaceDef raceDef in DefDatabase<LegendaryRaceDef>.AllDefs)
            {
                if (raceDef.Abstract) continue;
                
                IRaceExtension handler = raceDef.GetRaceHandler();
                if (handler != null && handler.IsMemberOfRace(pawn))
                {
                    RaceDefName = raceDef.defName;
                    
                    if (LegendaryRacesFrameworkMod.Settings.showDebugLogs)
                    {
                        Log.Message(string.Format("LRF.RaceDetected".Translate(), pawn.Name, raceDef.defName));
                    }
                    break;
                }
            }
        }
        
        // RimWorld 1.5 stat modification
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            
            // If this is a legendary race, record damage taken for balance metrics
            if (!string.IsNullOrEmpty(raceDefName) && RaceHandler != null)
            {
                var metrics = RaceHandler.BalanceMetrics;
                if (metrics != null)
                {
                    metrics.RecordMetric("DamageTaken", totalDamageDealt);
                }
            }
        }
        
        // Inspector string with race information
        public override string CompInspectStringExtra()
        {
            if (string.IsNullOrEmpty(raceDefName) || parent is not Pawn)
                return null;
                
            var handler = RaceHandler;
            if (handler == null)
                return null;
                
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Race: {handler.RaceName}");
            
            // Show resource levels if applicable
            if (handler.ResourceManager != null && handler.ResourceManager.ManagedResources.Count > 0)
            {
                sb.AppendLine("Resources:");
                
                foreach (var resource in handler.ResourceManager.ManagedResources)
                {
                    if (resource.IsVisible)
                    {
                        float value = handler.ResourceManager.GetResourceValue((Pawn)parent, resource.ResourceID);
                        sb.AppendLine($"  {resource.ResourceName}: {value:F0}/{resource.MaxValue:F0}");
                    }
                }
            }
            
            return sb.ToString().TrimEndNewlines();
        }
    }
    
    /// <summary>
    /// Component for tracking legendary character information on pawns
    /// </summary>
    public class Comp_LegendaryCharacter : ThingComp
    {
        private string characterID;
        
        public string CharacterID
        {
            get => characterID;
            set => characterID = value;
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref characterID, "characterID");
        }
        
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }
        
        public override string CompInspectStringExtra()
        {
            if (string.IsNullOrEmpty(characterID)) return null;
            
            ILegendaryCharacter character = LegendaryCharacterManager.GetLegendaryCharacter(characterID);
            if (character != null)
            {
                return $"Legendary character: {character.CharacterName}";
            }
            
            return null;
        }
        
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            
            // Special handling for legendary characters when damaged
            if (!string.IsNullOrEmpty(characterID) && parent is Pawn pawn)
            {
                ILegendaryCharacter character = LegendaryCharacterManager.GetLegendaryCharacter(characterID);
                if (character != null)
                {
                    // Specialized damage response for legendary characters could go here
                    // For example, triggering special abilities when health is low
                    
                    if (pawn.health.summaryHealth.SummaryHealthPercent < 0.3f)
                    {
                        // Could trigger emergency abilities, etc.
                    }
                }
            }
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (string.IsNullOrEmpty(characterID) || parent is not Pawn pawn || 
                pawn.Faction == null || !pawn.Faction.IsPlayer)
                yield break;
                
            ILegendaryCharacter character = LegendaryCharacterManager.GetLegendaryCharacter(characterID);
            if (character == null)
                yield break;
                
            // Add character-specific ability gizmos
            foreach (var ability in character.UniqueAbilities)
            {
                if (ability.IsPassive)
                    continue; // Skip passive abilities
                    
                int cooldown = ability.GetCurrentCooldown(pawn);
                
                Command_Action command = new Command_Action
                {
                    defaultLabel = ability.AbilityName,
                    defaultDesc = ability.AbilityDescription + (cooldown > 0 ? $"\n\nCooldown: {cooldown / 60f:F1}s" : ""),
                    icon = ContentFinder<Texture2D>.Get(ability.IconPath, false) ?? BaseContent.BadTex,
                    action = delegate { UseAbility(pawn, ability); }
                };
                
                // Disable if on cooldown
                if (cooldown > 0)
                {
                    command.Disable($"On cooldown: {cooldown / 60f:F1}s");
                }
                
                yield return command;
            }
        }
        
        private void UseAbility(Pawn pawn, ISpecialAbility ability)
        {
            if (pawn == null || ability == null)
                return;
                
            // For abilities requiring a target
            if (ability.TargetingType != TargetingType.Self)
            {
                Find.Targeter.BeginTargeting(new TargetingParameters
                {
                    canTargetPawns = true,
                    canTargetBuildings = ability.TargetingType.HasFlag(TargetingType.Buildings),
                    canTargetItems = ability.TargetingType.HasFlag(TargetingType.Items),
                    canTargetLocations = ability.TargetingType.HasFlag(TargetingType.Locations)
                }, 
                delegate(LocalTargetInfo target) 
                {
                    // Once target is selected, use the ability
                    if (ability.UseAbility(pawn, target))
                    {
                        // Notify success
                        Messages.Message(
                            string.Format("LRF.AbilityActivated".Translate(), pawn.LabelCap, ability.AbilityName),
                            pawn,
                            MessageTypeDefOf.PositiveEvent);
                    }
                }, 
                null, // Mouse attachment texture 
                true); // Play sound
            }
            else
            {
                // Self-targeted ability
                if (ability.UseAbility(pawn, pawn))
                {
                    // Notify success
                    Messages.Message(
                        string.Format("LRF.AbilityActivated".Translate(), pawn.LabelCap, ability.AbilityName),
                        pawn,
                        MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }
    
    /// <summary>
    /// Component properties for legendary race component
    /// </summary>
    public class CompProperties_LegendaryRace : CompProperties
    {
        public CompProperties_LegendaryRace()
        {
            compClass = typeof(Comp_LegendaryRace);
        }
    }
    
    /// <summary>
    /// Component properties for legendary character component
    /// </summary>
    public class CompProperties_LegendaryCharacter : CompProperties
    {
        public CompProperties_LegendaryCharacter()
        {
            compClass = typeof(Comp_LegendaryCharacter);
        }
    }
    
    /// <summary>
    /// Targeting types for abilities
    /// </summary>
    [Flags]
    public enum TargetingType
    {
        None = 0,
        Self = 1,
        Pawns = 2,
        Buildings = 4,
        Items = 8,
        Locations = 16,
        
        // Combinations
        SingleTarget = Pawns | Buildings | Items,
        MultiTarget = Pawns | Buildings | Items | Locations,
        AoE = Locations | Pawns | Buildings | Items
    }
}