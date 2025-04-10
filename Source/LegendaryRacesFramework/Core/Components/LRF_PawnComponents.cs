using System;
using Verse;

namespace LegendaryRacesFramework
{
    /// <summary>
    /// Component for tracking race information on pawns
    /// </summary>
    public class Comp_LegendaryRace : ThingComp
    {
        private string raceDefName;
        private IRaceExtension raceHandler;
        
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
        }
        
        public override void CompTick()
        {
            base.CompTick();
            
            // Auto-detect race if not set
            if (string.IsNullOrEmpty(raceDefName) && parent is Pawn pawn)
            {
                DetectRace(pawn);
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
                    Log.Message($"Detected legendary race for {pawn.Name}: {raceDef.defName}");
                    break;
                }
            }
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
}