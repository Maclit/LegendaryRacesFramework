using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    public static class LegendaryCharacterManager
    {
        private static Dictionary<string, List<ILegendaryCharacter>> legendaryCharactersByRace = new Dictionary<string, List<ILegendaryCharacter>>();
        
        public static void Initialize()
        {
            legendaryCharactersByRace.Clear();
            
            // Load all legendary character defs
            foreach (LegendaryCharacterDef charDef in DefDatabase<LegendaryCharacterDef>.AllDefs)
            {
                if (charDef.Abstract) continue;
                
                try
                {
                    // Create legendary character instance
                    ILegendaryCharacter character = CreateLegendaryCharacter(charDef);
                    if (character != null)
                    {
                        // Add to race-specific list
                        string raceID = character.RaceID;
                        if (!legendaryCharactersByRace.TryGetValue(raceID, out List<ILegendaryCharacter> raceCharacters))
                        {
                            raceCharacters = new List<ILegendaryCharacter>();
                            legendaryCharactersByRace[raceID] = raceCharacters;
                        }
                        
                        raceCharacters.Add(character);
                        
                        Log.Message($"Loaded legendary character: {character.CharacterName} for race {raceID}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error creating legendary character from def {charDef.defName}: {ex}");
                }
            }
        }
        
        public static List<ILegendaryCharacter> GetLegendaryCharactersForRace(string raceID)
        {
            if (string.IsNullOrEmpty(raceID) || !legendaryCharactersByRace.TryGetValue(raceID, out List<ILegendaryCharacter> characters))
            {
                return new List<ILegendaryCharacter>();
            }
            
            return characters;
        }
        
        public static ILegendaryCharacter GetLegendaryCharacter(string characterID)
        {
            foreach (var characters in legendaryCharactersByRace.Values)
            {
                ILegendaryCharacter character = characters.FirstOrDefault(c => c.CharacterID == characterID);
                if (character != null)
                {
                    return character;
                }
            }
            
            return null;
        }
        
        public static bool IsLegendaryCharacter(Pawn pawn)
        {
            if (pawn == null) return false;
            
            // Check if the pawn has the legendary character comp
            var legendaryComp = pawn.GetComp<Comp_LegendaryCharacter>();
            return legendaryComp != null && !string.IsNullOrEmpty(legendaryComp.CharacterID);
        }
        
        public static ILegendaryCharacter GetPawnLegendaryCharacter(Pawn pawn)
        {
            if (pawn == null) return null;
            
            // Get character ID from comp
            var legendaryComp = pawn.GetComp<Comp_LegendaryCharacter>();
            if (legendaryComp == null || string.IsNullOrEmpty(legendaryComp.CharacterID))
            {
                return null;
            }
            
            return GetLegendaryCharacter(legendaryComp.CharacterID);
        }
        
        public static void ApplyLegendaryCharacterToPawn(Pawn pawn, ILegendaryCharacter character)
        {
            if (pawn == null || character == null) return;
            
            // Add legendary character comp if not present
            var legendaryComp = pawn.GetComp<Comp_LegendaryCharacter>();
            if (legendaryComp == null)
            {
                Log.Error($"Cannot apply legendary character to pawn {pawn.Name} without Comp_LegendaryCharacter");
                return;
            }
            
            // Set character ID
            legendaryComp.CharacterID = character.CharacterID;
            
            // Apply character traits and abilities
            character.ApplyToCharacter(pawn);
            
            // Apply name
            if (!pawn.Name.ToStringFull.Contains(character.CharacterName))
            {
                pawn.Name = new NameTriple(character.CharacterName, character.CharacterName, "");
            }
        }
        
        private static ILegendaryCharacter CreateLegendaryCharacter(LegendaryCharacterDef charDef)
        {
            return new DefaultLegendaryCharacter(charDef);
        }
        
        // DefaultLegendaryCharacter implementation
        public class DefaultLegendaryCharacter : ILegendaryCharacter
        {
            private readonly LegendaryCharacterDef charDef;
            
            public DefaultLegendaryCharacter(LegendaryCharacterDef charDef)
            {
                this.charDef = charDef;
            }
            
            public string CharacterID => charDef.characterProperties.characterID;
            
            public string RaceID => charDef.characterProperties.raceID;
            
            public string CharacterName => charDef.characterProperties.characterName;
            
            public string Backstory => charDef.characterProperties.backstory;
            
            public long BirthTicks => GetBirthTicks();
            
            public List<ISpecialAbility> UniqueAbilities
            {
                get
                {
                    List<ISpecialAbility> abilities = new List<ISpecialAbility>();
                    
                    if (charDef.uniqueAbilities != null)
                    {
                        foreach (var abilityDef in charDef.uniqueAbilities)
                        {
                            abilities.Add(new DefaultAbility(abilityDef));
                        }
                    }
                    
                    return abilities;
                }
            }
            
            public List<TraitDef> UniqueTraits
            {
                get
                {
                    List<TraitDef> traits = new List<TraitDef>();
                    
                    if (charDef.traitProperties?.forcedTraits != null)
                    {
                        foreach (var trait in charDef.traitProperties.forcedTraits)
                        {
                            traits.Add(trait.def);
                        }
                    }
                    
                    return traits;
                }
            }
            
            public List<HediffDef> UniqueHediffs
            {
                get
                {
                    List<HediffDef> hediffs = new List<HediffDef>();
                    
                    if (charDef.uniqueHediffs != null)
                    {
                        foreach (var hediffEntry in charDef.uniqueHediffs)
                        {
                            hediffs.Add(hediffEntry.hediffDef);
                        }
                    }
                    
                    return hediffs;
                }
            }
            
            public void ApplyToCharacter(Pawn pawn)
            {
                if (pawn == null) return;
                
                // Apply traits
                if (charDef.traitProperties?.forcedTraits != null)
                {
                    foreach (var traitEntry in charDef.traitProperties.forcedTraits)
                    {
                        // Skip if pawn already has this trait
                        if (pawn.story.traits.HasTrait(traitEntry.def))
                            continue;
                        
                        // Check chance
                        if (traitEntry.chance < 1f && Rand.Value > traitEntry.chance)
                            continue;
                        
                        // Add trait
                        pawn.story.traits.GainTrait(new Trait(traitEntry.def, traitEntry.degree));
                    }
                }
                
                // Apply skills
                if (charDef.traitProperties?.skillSettings != null)
                {
                    foreach (var skillEntry in charDef.traitProperties.skillSettings)
                    {
                        SkillRecord skill = pawn.skills.GetSkill(skillEntry.def);
                        if (skill != null)
                        {
                            skill.Level = skillEntry.value;
                            
                            if (skillEntry.passion == "Major")
                            {
                                skill.passion = Passion.Major;
                            }
                            else if (skillEntry.passion == "Minor")
                            {
                                skill.passion = Passion.Minor;
                            }
                            else
                            {
                                skill.passion = Passion.None;
                            }
                        }
                    }
                }
                
                // Apply hediffs
                if (charDef.uniqueHediffs != null)
                {
                    foreach (var hediffEntry in charDef.uniqueHediffs)
                    {
                        // Find body part if specified
                        BodyPartRecord bodyPart = null;
                        if (!string.IsNullOrEmpty(hediffEntry.bodyPartLabel))
                        {
                            bodyPart = pawn.health.hediffSet.GetNotMissingParts()
                                .FirstOrDefault(p => p.def.label == hediffEntry.bodyPartLabel);
                        }
                        
                        // Create and add hediff
                        Hediff hediff = HediffMaker.MakeHediff(hediffEntry.hediffDef, pawn, bodyPart);
                        hediff.Severity = hediffEntry.severity;
                        pawn.health.AddHediff(hediff);
                    }
                }
                
                // Give starting equipment if spawning
                if (charDef.uniqueEquipment != null && pawn.inventory != null && pawn.equipment != null)
                {
                    foreach (var equipmentEntry in charDef.uniqueEquipment)
                    {
                        for (int i = 0; i < equipmentEntry.count; i++)
                        {
                            Thing thing = ThingMaker.MakeThing(equipmentEntry.thingDef);
                            
                            // Set quality if applicable
                            if (equipmentEntry.quality != QualityCategory.Awful && thing.TryGetComp<CompQuality>() is CompQuality qualityComp)
                            {
                                qualityComp.SetQuality(equipmentEntry.quality, ArtGenerationContext.Outsider);
                            }
                            
                            // Add to equipment or inventory
                            if (thing.def.IsApparel)
                            {
                                Apparel apparel = thing as Apparel;
                                pawn.apparel.Wear(apparel);
                            }
                            else if (thing.def.IsWeapon)
                            {
                                pawn.equipment.AddEquipment(thing as ThingWithComps);
                            }
                            else
                            {
                                pawn.inventory.innerContainer.TryAdd(thing);
                            }
                        }
                    }
                }
                
                // Apply enhanced race mechanics
                if (charDef.enhancedMechanics != null)
                {
                    foreach (var mechanicEntry in charDef.enhancedMechanics)
                    {
                        // Apply stat modifications
                        if (mechanicEntry.statModifications != null)
                        {
                            // Enhanced stats would be applied through a hediff or comp
                            // For now, we'll log that these would be applied
                            Log.Message($"Applying enhanced mechanics for {CharacterName}: {mechanicEntry.enhancementDescription}");
                        }
                    }
                }
            }
            
            public ScenarioDef GetStartingScenario()
            {
                // This would return a custom scenario if defined
                // For now, return null to use default scenario
                return null;
            }
            
            private long GetBirthTicks()
            {
                // Calculate birth ticks based on relative days from game start
                if (charDef.characterProperties.birthDateRelative < 0)
                {
                    return GenTicks.ConfiguredTicksAbsAtGameStart + (charDef.characterProperties.birthDateRelative * GenDate.TicksPerDay);
                }
                else
                {
                    // Default to 40 years before game start
                    return GenTicks.ConfiguredTicksAbsAtGameStart - (40 * GenDate.TicksPerYear);
                }
            }
        }
    }
}