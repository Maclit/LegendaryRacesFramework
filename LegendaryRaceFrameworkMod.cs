using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace LegendaryRacesFramework
{
    public class LegendaryRacesFrameworkMod : Mod
    {
        private static Dictionary<string, Type> registeredRaceExtensions = new Dictionary<string, Type>();
        private static Dictionary<string, Type> registeredAbilityTypes = new Dictionary<string, Type>();
        
        private static Harmony harmony;
        public static LegendaryRacesFrameworkMod Instance { get; private set; }
        
        public LegendaryRacesFrameworkMod(ModContentPack content) : base(content)
        {
            Instance = this;
            harmony = new Harmony("LegendaryRacesFramework");
            
            // Apply patches
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            // Log startup information
            Log.Message("Legendary Races Framework initializing...");
            
            // Register comp properties
            RegisterCompProperties();
            
            // Register default implementations
            RegisterDefaultImplementations();
            
            // Check for VEF
            if (!ModsConfig.IsActive("OskarPotocki.VanillaExpandedFramework"))
            {
                Log.Error("Legendary Races Framework requires Vanilla Expanded Framework (VEF) to function correctly. Please enable VEF.");
            }
            
            // Optional integrations
            CheckOptionalModIntegrations();
            
            Log.Message("Legendary Races Framework initialized successfully!");
        }
        
        private void RegisterCompProperties()
        {
            // Register comp properties for use in XML
            DefGenerator.AddImpliedDef(new CompProperties_LegendaryRace());
            DefGenerator.AddImpliedDef(new CompProperties_LegendaryCharacter());
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
}