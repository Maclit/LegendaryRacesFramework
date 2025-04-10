using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace LegendaryRacesFramework
{
    [StaticConstructorOnStartup]
    public class LegendaryRacesFrameworkMod : Mod
    {
        private static Dictionary<string, Type> registeredRaceExtensions = new Dictionary<string, Type>();
        private static Dictionary<string, Type> registeredAbilityTypes = new Dictionary<string, Type>();
        
        public static LegendaryRacesFrameworkSettings Settings { get; private set; }
        
        private static Harmony harmony;
        public static LegendaryRacesFrameworkMod Instance { get; private set; }
        
        // Flag to track if components have been registered
        private static bool componentsRegistered = false;

        // Static constructor - use this ONLY for things that require all defs to be loaded
        static LegendaryRacesFrameworkMod()
        {
            // Use LongEventHandler to ensure this runs at the right time
            LongEventHandler.ExecuteWhenFinished(RegisterCompPropertiesDelayed);
            
            Log.Message("Legendary Races Framework: Static initialization complete");
        }
        
        public LegendaryRacesFrameworkMod(ModContentPack content) : base(content)
        {
            Instance = this;
            harmony = new Harmony("JulienCastillejos.LegendaryRacesFramework");
            
            // Initialize settings
            Settings = GetSettings<LegendaryRacesFrameworkSettings>();
            
            // Apply patches
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            // Log startup information
            Log.Message("Legendary Races Framework initializing...");
            
            // Register default implementations
            RegisterDefaultImplementations();
            
            // Check for VEF (just a warning, not critical)
            if (!ModsConfig.IsActive("OskarPotocki.VanillaFactionsExpanded.Core"))
            {
                Log.Warning("Legendary Races Framework works best with Vanilla Expanded Framework (VEF). Some features may be limited without it.");
            }
            
            // Optional integrations
            CheckOptionalModIntegrations();
            
            Log.Message("Legendary Races Framework initialized successfully!");
        }
        
        // Add settings handling for RimWorld
        public override string SettingsCategory()
        {
            return "Legendary Races Framework";
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }
        
        // This is called after all defs are loaded via LongEventHandler
        private static void RegisterCompPropertiesDelayed()
        {
            if (componentsRegistered)
                return;
                
            try {
                // Find Human def using DefDatabase instead of ThingDefOf
                ThingDef pawnDef = DefDatabase<ThingDef>.GetNamed("Human");
                if (pawnDef == null) {
                    Log.Error("Failed to find Human ThingDef when registering components.");
                    return;
                }
                
                // Create comp properties instances
                var legendaryRaceProps = new CompProperties_LegendaryRace();
                var legendaryCharacterProps = new CompProperties_LegendaryCharacter();

                // Add comp properties to pawn def if they don't already exist
                if (!pawnDef.comps.Any(c => c is CompProperties_LegendaryRace))
                {
                    pawnDef.comps.Add(legendaryRaceProps);
                }
                
                if (!pawnDef.comps.Any(c => c is CompProperties_LegendaryCharacter))
                {
                    pawnDef.comps.Add(legendaryCharacterProps);
                }
                
                // Log component registration
                Log.Message("Legendary Races Framework: Registered pawn components");
                componentsRegistered = true;
            }
            catch (Exception ex) {
                Log.Error($"Error registering comp properties: {ex}");
            }
        }
        
        private void RegisterDefaultImplementations()
        {
            // No need to register default implementations as they are created automatically
            // This would be where custom implementations from other mods would register
        }
        
        private void CheckOptionalModIntegrations()
        {
            // Check for HAR
            if (ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
            {
                Log.Message("Legendary Races Framework: Humanoid Alien Races (HAR) detected, enabling HAR integration.");
                // Initialize HAR integration
            }
            
            // Check for CE
            if (ModsConfig.IsActive("CETeam.CombatExtended"))
            {
                Log.Message("Legendary Races Framework: Combat Extended (CE) detected, enabling CE integration.");
                // Initialize CE integration
            }
        }
        
        /// <summary>
        /// Register a custom race extension implementation
        /// </summary>
        public static void RegisterRaceExtension(string raceDefName, Type extensionType)
        {
            if (string.IsNullOrEmpty(raceDefName) || extensionType == null)
                return;
            
            if (!typeof(IRaceExtension).IsAssignableFrom(extensionType))
            {
                Log.Error($"Cannot register race extension type {extensionType.Name} as it does not implement IRaceExtension");
                return;
            }
            
            registeredRaceExtensions[raceDefName] = extensionType;
            Log.Message($"Registered custom race extension {extensionType.Name} for race {raceDefName}");
        }
        
        /// <summary>
        /// Get a registered race extension or null if not registered
        /// </summary>
        public static IRaceExtension GetRegisteredRaceExtension(string raceDefName)
        {
            if (string.IsNullOrEmpty(raceDefName) || !registeredRaceExtensions.TryGetValue(raceDefName, out Type extensionType))
                return null;
            
            try
            {
                // Get the race def
                LegendaryRaceDef raceDef = DefDatabase<LegendaryRaceDef>.GetNamed(raceDefName);
                if (raceDef == null)
                    return null;
                
                // Create instance
                return (IRaceExtension)Activator.CreateInstance(extensionType, raceDef);
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating race extension for {raceDefName}: {ex}");
                return null;
            }
        }
        
        /// <summary>
        /// Register a custom ability type
        /// </summary>
        public static void RegisterAbilityType(string abilityClassName, Type abilityType)
        {
            if (string.IsNullOrEmpty(abilityClassName) || abilityType == null)
                return;
            
            if (!typeof(ISpecialAbility).IsAssignableFrom(abilityType))
            {
                Log.Error($"Cannot register ability type {abilityType.Name} as it does not implement ISpecialAbility");
                return;
            }
            
            registeredAbilityTypes[abilityClassName] = abilityType;
            Log.Message($"Registered custom ability type {abilityType.Name} as {abilityClassName}");
        }
        
        /// <summary>
        /// Get a registered ability type or null if not registered
        /// </summary>
        public static Type GetRegisteredAbilityType(string abilityClassName)
        {
            if (string.IsNullOrEmpty(abilityClassName) || !registeredAbilityTypes.TryGetValue(abilityClassName, out Type abilityType))
                return null;
            
            return abilityType;
        }
    }
    
    public class LegendaryRacesFrameworkSettings : ModSettings
    {
        // Framework-wide settings
        public bool enablePerformanceMonitoring = true;
        public bool showDebugLogs = false;
        public bool enableBalancingTools = true;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enablePerformanceMonitoring, "enablePerformanceMonitoring", true);
            Scribe_Values.Look(ref showDebugLogs, "showDebugLogs", false);
            Scribe_Values.Look(ref enableBalancingTools, "enableBalancingTools", true);
        }
        
        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            
            listing.CheckboxLabeled("Enable performance monitoring", ref enablePerformanceMonitoring, 
                "Monitors performance of race mechanics to detect potential issues");
            
            listing.CheckboxLabeled("Show debug logs", ref showDebugLogs,
                "Shows additional debug information in the log");
            
            listing.CheckboxLabeled("Enable balancing tools", ref enableBalancingTools,
                "Enables tools for balancing race mechanics");
            
            listing.End();
        }
    }
}