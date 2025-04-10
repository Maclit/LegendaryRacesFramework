using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    public class LegendaryCharacterDef : Def
    {
        // Character properties
        public CharacterProperties characterProperties;
        
        // Trait properties
        public TraitProperties traitProperties;
        
        // Abilities
        public List<RaceAbilityDef> uniqueAbilities;
        
        // Enhanced mechanics
        public List<EnhancedMechanicDef> enhancedMechanics;
        
        // Equipment
        public List<EquipmentEntry> uniqueEquipment;
        
        // Hediffs
        public List<HediffEntry> uniqueHediffs;
        
        // Origin scenario
        public OriginScenarioDef originScenario;
        
        // Story elements
        public List<StoryElementDef> storyElements;
        
        // Progression
        public ProgressionPathDef progressionPath;
        
        // Relationships
        public List<SpecialRelationshipDef> specialRelationships;

        public bool Abstract;
        
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            
            // Resolve references in character properties
            characterProperties?.ResolveReferences();
            
            // Resolve references in trait properties
            traitProperties?.ResolveReferences();
            
            // Resolve references in abilities
            if (uniqueAbilities != null)
            {
                foreach (var ability in uniqueAbilities)
                {
                    // Nothing to resolve here, as this is already handled in the race def
                }
            }
            
            // Resolve references in enhanced mechanics
            if (enhancedMechanics != null)
            {
                foreach (var mechanic in enhancedMechanics)
                {
                    mechanic.ResolveReferences();
                }
            }
            
            // Resolve references in equipment
            if (uniqueEquipment != null)
            {
                foreach (var equipment in uniqueEquipment)
                {
                    equipment.ResolveReferences();
                }
            }
            
            // Resolve references in hediffs
            if (uniqueHediffs != null)
            {
                foreach (var hediff in uniqueHediffs)
                {
                    hediff.ResolveReferences();
                }
            }
            
            // Resolve references in origin scenario
            originScenario?.ResolveReferences();
            
            // Resolve references in story elements
            if (storyElements != null)
            {
                foreach (var element in storyElements)
                {
                    element.ResolveReferences();
                }
            }
            
            // Resolve references in progression path
            progressionPath?.ResolveReferences();
            
            // Resolve references in relationships
            if (specialRelationships != null)
            {
                foreach (var relationship in specialRelationships)
                {
                    relationship.ResolveReferences();
                }
            }
        }
    }
    
    public class CharacterProperties
    {
        public string characterID;
        public string characterName;
        public string backstory;
        public string portraitPath;
        public string raceID;
        public long birthDateRelative = -4500; // Negative number of days from game start
        public string gender = "Male"; // Male, Female, None
        public string faction;
        
        public void ResolveReferences()
        {
            // Verify faction exists if specified
            if (!string.IsNullOrEmpty(faction))
            {
                FactionDef factionDef = DefDatabase<FactionDef>.GetNamedSilentFail(faction);
                if (factionDef == null)
                {
                    Log.Warning($"Could not find faction def {faction} for legendary character {characterName}");
                }
            }
        }
    }
    
    public class TraitProperties
    {
        public List<ForcedTrait> forcedTraits;
        public List<DisallowedTrait> disallowedTraits;
        public List<SkillSetting> skillSettings;
        
        public void ResolveReferences()
        {
            // Verify traits exist
            if (forcedTraits != null)
            {
                foreach (var trait in forcedTraits)
                {
                    if (trait.def == null)
                    {
                        Log.Error($"Null trait def in forced traits");
                    }
                }
            }
            
            // Verify disallowed traits exist
            if (disallowedTraits != null)
            {
                foreach (var trait in disallowedTraits)
                {
                    if (trait.def == null)
                    {
                        Log.Error($"Null trait def in disallowed traits");
                    }
                }
            }
            
            // Verify skills exist
            if (skillSettings != null)
            {
                foreach (var skill in skillSettings)
                {
                    if (skill.def == null)
                    {
                        Log.Error($"Null skill def in skill settings");
                    }
                }
            }
        }
    }
    
    public class ForcedTrait
    {
        public TraitDef def;
        public int degree;
        public float chance = 1f;
    }
    
    public class DisallowedTrait
    {
        public TraitDef def;
    }
    
    public class SkillSetting
    {
        public SkillDef def;
        public int value;
        public string passion = "None"; // None, Minor, Major
    }
    
    public class EnhancedMechanicDef
    {
        public string mechanicID;
        public string baseMechanicID;
        public float powerScalingFactor = 1f;
        public string enhancementDescription;
        public List<StatModification> statModifications;
        public List<AbilityModification> abilityModifications;
        
        public void ResolveReferences()
        {
            // Verify stat defs exist
            if (statModifications != null)
            {
                foreach (var stat in statModifications)
                {
                    if (stat.statDef == null)
                    {
                        Log.Error($"Null stat def in enhanced mechanic {mechanicID}");
                    }
                }
            }
        }
    }
    
    public class StatModification
    {
        public StatDef statDef;
        public float value;
    }
    
    public class AbilityModification
    {
        public string abilityID;
        public float powerMultiplier = 1f;
        public float cooldownFactor = 1f;
        public float resourceCostFactor = 1f;
    }
    
    public class EquipmentEntry
    {
        public ThingDef thingDef;
        public int count = 1;
        public QualityCategory quality = QualityCategory.Normal;
        
        public void ResolveReferences()
        {
            if (thingDef == null)
            {
                Log.Error($"Null thing def in equipment entry");
            }
        }
    }
    
    public class HediffEntry
    {
        public HediffDef hediffDef;
        public float severity = 1f;
        public string bodyPartLabel;
        
        public void ResolveReferences()
        {
            if (hediffDef == null)
            {
                Log.Error($"Null hediff def in hediff entry");
            }
        }
    }
    
    public class OriginScenarioDef
    {
        public string scenarioID;
        public string scenarioName;
        public string scenarioDescription;
        public int startingPawns = 1;
        public bool forcedStartingCharacter = true;
        public bool startWithCharacter = true;
        public List<ThingDefCount> additionalStartingItems;
        public List<ScenPart> specialScenParts;
        public string preferredStartingBiome;
        
        public void ResolveReferences()
        {
            // Verify items exist
            if (additionalStartingItems != null)
            {
                foreach (var item in additionalStartingItems)
                {
                    if (item.thingDef == null)
                    {
                        Log.Error($"Null thing def in additional starting items for scenario {scenarioID}");
                    }
                }
            }
            
            // Verify biome exists if specified
            if (!string.IsNullOrEmpty(preferredStartingBiome))
            {
                BiomeDef biomeDef = DefDatabase<BiomeDef>.GetNamedSilentFail(preferredStartingBiome);
                if (biomeDef == null)
                {
                    Log.Warning($"Could not find biome def {preferredStartingBiome} for scenario {scenarioID}");
                }
            }
        }
    }
    
    public class StoryElementDef
    {
        public string elementID;
        public string elementName;
        public string elementDescription;
        public List<string> triggerConditions;
        public List<ConsequenceOption> consequenceOptions;
        
        public void ResolveReferences()
        {
            // Nothing to resolve here
        }
    }
    
    public class ConsequenceOption
    {
        public string optionText;
        public string consequenceDescription;
        public string consequenceClass;
    }
    
    public class ProgressionPathDef
    {
        public string pathID;
        public string pathName;
        public string pathDescription;
        public List<ProgressionStage> stages;
        
        public void ResolveReferences()
        {
            if (stages != null)
            {
                foreach (var stage in stages)
                {
                    stage.ResolveReferences();
                }
            }
        }
    }
    
    public class ProgressionStage
    {
        public string stageID;
        public string stageName;
        public string stageDescription;
        public List<string> requirements;
        public List<ProgressionReward> rewards;
        public string questClass;
        
        public void ResolveReferences()
        {
            // Nothing to resolve here
        }
    }
    
    public class ProgressionReward
    {
        public string rewardType; // Ability, StatBoost, Item, etc.
        public string rewardID;
        public float value;
    }
    
    public class SpecialRelationshipDef
    {
        public string relationshipID;
        public string relationshipType; // Rivalry, Alliance, etc.
        public string targetCharacterID;
        public string relationshipDescription;
        public int initialOpinion;
        public List<string> specialInteractions;
        
        public void ResolveReferences()
        {
            // Nothing to resolve here
        }
    }
}