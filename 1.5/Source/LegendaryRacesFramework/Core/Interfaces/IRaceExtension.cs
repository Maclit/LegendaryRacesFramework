using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using VFECore;

namespace LegendaryRacesFramework
{
    /// <summary>
    /// Base interface for race-specific implementations in the Legendary Races Framework.
    /// </summary>
    public interface IRaceExtension
    {
        /// <summary>
        /// Gets the unique identifier for this race.
        /// </summary>
        string RaceID { get; }
        
        /// <summary>
        /// Gets the display name of the race.
        /// </summary>
        string RaceName { get; }

        /// <summary>
        /// Gets the description of the race.
        /// </summary>
        string RaceDescription { get; }

        /// <summary>
        /// Gets the core gene def that defines this race.
        /// </summary>
        GeneDef CoreGeneDef { get; }

        /// <summary>
        /// Gets a list of secondary gene defs that further define this race.
        /// </summary>
        List<GeneDef> SecondaryGeneDefs { get; }

        /// <summary>
        /// Gets the resource manager for this race, if applicable.
        /// </summary>
        IResourceManager ResourceManager { get; }

        /// <summary>
        /// Gets all abilities available to this race.
        /// </summary>
        List<ISpecialAbility> RaceAbilities { get; }

        /// <summary>
        /// Gets the life cycle manager for this race.
        /// </summary>
        IRaceLifeCycle LifeCycleManager { get; }

        /// <summary>
        /// Gets or sets references to legendary characters for this race.
        /// </summary>
        List<ILegendaryCharacter> LegendaryCharacters { get; set; }

        /// <summary>
        /// Gets the balance metrics tracking for this race.
        /// </summary>
        IBalanceMetrics BalanceMetrics { get; }

        /// <summary>
        /// Initializes the race extension with any required setup.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Performs post-load initialization for the race extension.
        /// </summary>
        void PostLoadInit();

        /// <summary>
        /// Checks if a Pawn belongs to this race.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the pawn belongs to this race</returns>
        bool IsMemberOfRace(Pawn pawn);

        /// <summary>
        /// Gets custom stats for a pawn of this race.
        /// </summary>
        /// <param name="pawn">The pawn to get stats for</param>
        /// <returns>Dictionary of custom stat values</returns>
        Dictionary<string, float> GetCustomRaceStats(Pawn pawn);
    }

    /// <summary>
    /// Interface for managing race-specific resources.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gets the ID of the race that this resource manager belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets all resources managed by this manager.
        /// </summary>
        List<RaceResource> ManagedResources { get; }

        /// <summary>
        /// Gets the current value of a resource for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <param name="resourceDefName">The resource def name</param>
        /// <returns>Current resource value</returns>
        float GetResourceValue(Pawn pawn, string resourceDefName);

        /// <summary>
        /// Sets the current value of a resource for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        /// <param name="resourceDefName">The resource def name</param>
        /// <param name="value">The new value</param>
        void SetResourceValue(Pawn pawn, string resourceDefName, float value);

        /// <summary>
        /// Adjusts the current value of a resource for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        /// <param name="resourceDefName">The resource def name</param>
        /// <param name="delta">The change in value</param>
        void AdjustResourceValue(Pawn pawn, string resourceDefName, float delta);

        /// <summary>
        /// Registers a new resource to be managed.
        /// </summary>
        /// <param name="resource">The resource to register</param>
        void RegisterResource(RaceResource resource);

        /// <summary>
        /// Gets hediffs associated with resource levels for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>List of resource-related hediffs</returns>
        List<Hediff> GetResourceHediffs(Pawn pawn);
    }

    /// <summary>
    /// Interface for defining race-specific special abilities.
    /// </summary>
    public interface ISpecialAbility
    {
        /// <summary>
        /// Gets the unique identifier for this ability.
        /// </summary>
        string AbilityID { get; }

        /// <summary>
        /// Gets the display name of the ability.
        /// </summary>
        string AbilityName { get; }

        /// <summary>
        /// Gets the description of the ability.
        /// </summary>
        string AbilityDescription { get; }

        /// <summary>
        /// Gets the icon texture for the ability.
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// Gets the cooldown in ticks for the ability.
        /// </summary>
        int CooldownTicks { get; }

        /// <summary>
        /// Gets whether the ability is passive (automatic) or active (player-triggered).
        /// </summary>
        bool IsPassive { get; }

        /// <summary>
        /// Gets the resource costs for activating this ability.
        /// </summary>
        Dictionary<string, float> ResourceCosts { get; }

        /// <summary>
        /// Gets the stat requirements for using this ability.
        /// </summary>
        Dictionary<StatDef, float> StatRequirements { get; }

        /// <summary>
        /// Checks if a pawn can use this ability.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the ability can be used</returns>
        bool CanUseAbility(Pawn pawn);

        /// <summary>
        /// Performs the ability for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn using the ability</param>
        /// <param name="target">The target, if any</param>
        /// <returns>True if the ability was used successfully</returns>
        bool UseAbility(Pawn pawn, LocalTargetInfo target);

        /// <summary>
        /// Gets the current cooldown for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>Remaining cooldown in ticks, or 0 if ready</returns>
        int GetCurrentCooldown(Pawn pawn);
    }

    /// <summary>
    /// Interface for handling race life cycle, aging, and growth stages.
    /// </summary>
    public interface IRaceLifeCycle
    {
        /// <summary>
        /// Gets the ID of the race that this life cycle belongs to.
        /// </summary>
        string RaceID { get; }
        
        /// <summary>
        /// Gets the defined life stages for this race.
        /// </summary>
        List<RaceLifeStage> LifeStages { get; }

        /// <summary>
        /// Gets the average lifespan for this race in years.
        /// </summary>
        float AverageLifespanYears { get; }

        /// <summary>
        /// Gets the maximum lifespan for this race in years.
        /// </summary>
        float MaximumLifespanYears { get; }

        /// <summary>
        /// Gets the developmental stage for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>Current life stage information</returns>
        RaceLifeStage GetCurrentLifeStage(Pawn pawn);

        /// <summary>
        /// Gets the biological age factor for a specific pawn.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>Age factor modifier</returns>
        float GetAgingFactor(Pawn pawn);

        /// <summary>
        /// Handles a life stage transition for a pawn.
        /// </summary>
        /// <param name="pawn">The pawn transitioning</param>
        /// <param name="fromStage">Previous stage</param>
        /// <param name="toStage">New stage</param>
        void HandleLifeStageTransition(Pawn pawn, RaceLifeStage fromStage, RaceLifeStage toStage);

        /// <summary>
        /// Applies life stage hediffs to a pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        /// <param name="lifeStage">The life stage</param>
        void ApplyLifeStageHediffs(Pawn pawn, RaceLifeStage lifeStage);
    }

    /// <summary>
    /// Interface for tracking race balance metrics.
    /// </summary>
    public interface IBalanceMetrics
    {
        /// <summary>
        /// Gets the ID of the race that these metrics belong to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets the current power level assessment for the race.
        /// </summary>
        float CurrentPowerLevel { get; }

        /// <summary>
        /// Gets the resource economy balance factor.
        /// </summary>
        float ResourceBalanceFactor { get; }

        /// <summary>
        /// Records a new metric data point.
        /// </summary>
        /// <param name="metricType">The type of metric</param>
        /// <param name="value">The value to record</param>
        void RecordMetric(string metricType, float value);

        /// <summary>
        /// Gets the average value for a specific metric.
        /// </summary>
        /// <param name="metricType">The type of metric</param>
        /// <returns>Average value</returns>
        float GetAverageMetric(string metricType);

        /// <summary>
        /// Computes the balance score for the race.
        /// </summary>
        /// <returns>Balance score between 0 and 1</returns>
        float ComputeBalanceScore();

        /// <summary>
        /// Gets all recorded metrics as a dictionary.
        /// </summary>
        Dictionary<string, List<float>> GetAllMetrics();
    }

    /// <summary>
    /// Interface for monitoring performance impact of race mechanics.
    /// </summary>
    public interface IPerformanceMonitor
    {
        /// <summary>
        /// Gets the ID of the race that this monitor belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Starts timing a specific operation.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        void StartTiming(string operationName);

        /// <summary>
        /// Stops timing a specific operation and records the duration.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        void StopTiming(string operationName);

        /// <summary>
        /// Gets the average execution time for a specific operation.
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Average execution time in milliseconds</returns>
        float GetAverageExecutionTime(string operationName);

        /// <summary>
        /// Gets performance metrics for all operations.
        /// </summary>
        Dictionary<string, float> GetAllPerformanceMetrics();

        /// <summary>
        /// Checks if the race's performance impact is within acceptable limits.
        /// </summary>
        /// <returns>True if performance is acceptable</returns>
        bool IsPerformanceAcceptable();

        /// <summary>
        /// Suggests optimizations based on recorded metrics.
        /// </summary>
        /// <returns>List of suggested optimizations</returns>
        List<string> SuggestOptimizations();
    }

    /// <summary>
    /// Interface for handling version migrations.
    /// </summary>
    public interface IMigrationHandler
    {
        /// <summary>
        /// Gets the ID of the race that this handler belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets the current version of the race mod.
        /// </summary>
        Version CurrentVersion { get; }

        /// <summary>
        /// Checks if migration is needed from a previous version.
        /// </summary>
        /// <param name="previousVersion">The previous version</param>
        /// <returns>True if migration is needed</returns>
        bool NeedsMigration(Version previousVersion);

        /// <summary>
        /// Performs migration from a previous version.
        /// </summary>
        /// <param name="previousVersion">The previous version</param>
        /// <param name="world">The game world</param>
        /// <returns>True if migration was successful</returns>
        bool PerformMigration(Version previousVersion, World world);

        /// <summary>
        /// Gets all migration steps available.
        /// </summary>
        Dictionary<Version, Action<World>> GetAllMigrationSteps();
    }

    /// <summary>
    /// Interface for handling Humanoid Alien Races (HAR) integration.
    /// </summary>
    public interface IHARBodyAdapter
    {
        /// <summary>
        /// Gets the ID of the race that this adapter belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Checks if HAR is available.
        /// </summary>
        bool IsHARAvailable { get; }

        /// <summary>
        /// Gets the HAR alien race def for this race, if HAR is available.
        /// </summary>
        object AlienRaceDef { get; }

        /// <summary>
        /// Gets the body types available for this race with HAR.
        /// </summary>
        List<string> AvailableBodyTypes { get; }

        /// <summary>
        /// Applies HAR-specific body customizations to a pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        void ApplyBodyCustomization(Pawn pawn);

        /// <summary>
        /// Gets fallback body settings when HAR is not available.
        /// </summary>
        Dictionary<string, object> GetFallbackBodySettings();
    }

    /// <summary>
    /// Interface for Combat Extended (CE) integration.
    /// </summary>
    public interface ICECompatibility
    {
        /// <summary>
        /// Gets the ID of the race that this compatibility belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Checks if CE is available.
        /// </summary>
        bool IsCEAvailable { get; }

        /// <summary>
        /// Gets CE-specific race stats.
        /// </summary>
        Dictionary<string, float> CEStats { get; }

        /// <summary>
        /// Applies CE-specific stats to a pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        void ApplyCEStats(Pawn pawn);

        /// <summary>
        /// Gets CE weapon adapters for race-specific weapons.
        /// </summary>
        List<ICEWeaponAdapter> WeaponAdapters { get; }
    }

    /// <summary>
    /// Interface for adapting race weapons to Combat Extended systems.
    /// </summary>
    public interface ICEWeaponAdapter
    {
        /// <summary>
        /// Gets the standard weapon def.
        /// </summary>
        ThingDef WeaponDef { get; }

        /// <summary>
        /// Gets the CE weapon properties.
        /// </summary>
        Dictionary<string, object> CEProperties { get; }

        /// <summary>
        /// Gets the ammunition types for this weapon.
        /// </summary>
        List<ThingDef> AmmoTypes { get; }

        /// <summary>
        /// Applies CE-specific properties to a weapon instance.
        /// </summary>
        /// <param name="weapon">The weapon to modify</param>
        void ApplyCEPropertiesToWeapon(Thing weapon);
    }

    /// <summary>
    /// Interface for legendary character implementation.
    /// </summary>
    public interface ILegendaryCharacter
    {
        /// <summary>
        /// Gets the unique identifier for this legendary character.
        /// </summary>
        string CharacterID { get; }

        /// <summary>
        /// Gets the race ID that this character belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets the display name of the character.
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// Gets the backstory of the character.
        /// </summary>
        string Backstory { get; }

        /// <summary>
        /// Gets the birth date or age of the character.
        /// </summary>
        long BirthTicks { get; }

        /// <summary>
        /// Gets the special abilities unique to this legendary character.
        /// </summary>
        List<ISpecialAbility> UniqueAbilities { get; }

        /// <summary>
        /// Gets the special traits unique to this legendary character.
        /// </summary>
        List<TraitDef> UniqueTraits { get; }

        /// <summary>
        /// Gets the hediffs that should be applied to this character.
        /// </summary>
        List<HediffDef> UniqueHediffs { get; }

        /// <summary>
        /// Applies legendary character traits and abilities to a pawn.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        void ApplyToCharacter(Pawn pawn);

        /// <summary>
        /// Gets the starting scenario for this legendary character.
        /// </summary>
        ScenarioDef GetStartingScenario();
    }

    /// <summary>
    /// Interface for handling legendary character origins.
    /// </summary>
    public interface ILegendaryOrigin
    {
        /// <summary>
        /// Gets the unique identifier for this origin.
        /// </summary>
        string OriginID { get; }

        /// <summary>
        /// Gets the race ID that this origin belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets the display name of the origin.
        /// </summary>
        string OriginName { get; }

        /// <summary>
        /// Gets the description of the origin.
        /// </summary>
        string OriginDescription { get; }

        /// <summary>
        /// Gets the legendary characters associated with this origin.
        /// </summary>
        List<ILegendaryCharacter> AssociatedCharacters { get; }

        /// <summary>
        /// Gets the starting pawns for this origin.
        /// </summary>
        /// <returns>List of starting pawns</returns>
        List<Pawn> GetStartingPawns();

        /// <summary>
        /// Gets the starting items for this origin.
        /// </summary>
        /// <returns>List of starting items</returns>
        List<Thing> GetStartingItems();

        /// <summary>
        /// Gets the starting map location for this origin.
        /// </summary>
        /// <returns>Preferred tile for starting</returns>
        int GetPreferredStartingTile();

        /// <summary>
        /// Gets special scenario parts for this origin.
        /// </summary>
        /// <returns>List of scenario parts</returns>
        List<ScenPart> GetScenarioParts();
    }

    /// <summary>
    /// Interface for amplified versions of race mechanics for legendary figures.
    /// </summary>
    public interface IEnhancedRaceMechanic
    {
        /// <summary>
        /// Gets the unique identifier for this enhanced mechanic.
        /// </summary>
        string MechanicID { get; }

        /// <summary>
        /// Gets the race ID that this mechanic belongs to.
        /// </summary>
        string RaceID { get; }

        /// <summary>
        /// Gets the base mechanic that this enhances.
        /// </summary>
        string BaseMechanicID { get; }

        /// <summary>
        /// Gets the power scaling factor compared to the base mechanic.
        /// </summary>
        float PowerScalingFactor { get; }

        /// <summary>
        /// Gets the description of the enhanced mechanic.
        /// </summary>
        string EnhancementDescription { get; }

        /// <summary>
        /// Applies the enhanced mechanic to a legendary character.
        /// </summary>
        /// <param name="pawn">The pawn to modify</param>
        void ApplyEnhancement(Pawn pawn);

        /// <summary>
        /// Gets stat modifications from this enhanced mechanic.
        /// </summary>
        Dictionary<StatDef, float> StatModifications { get; }

        /// <summary>
        /// Gets ability modifications from this enhanced mechanic.
        /// </summary>
        Dictionary<string, float> AbilityModifications { get; }
    }

    /// <summary>
    /// Represents a race resource type with related properties.
    /// </summary>
    public class RaceResource
    {
        /// <summary>
        /// Gets or sets the unique identifier for this resource.
        /// </summary>
        public string ResourceID { get; set; }

        /// <summary>
        /// Gets or sets the display name of the resource.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the description of the resource.
        /// </summary>
        public string ResourceDescription { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for this resource.
        /// </summary>
        public float MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for this resource.
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the default value for this resource.
        /// </summary>
        public float DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets whether this resource is visible to the player.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the natural regeneration rate per day.
        /// </summary>
        public float RegenerationRate { get; set; }

        /// <summary>
        /// Gets or sets the UI icon path for this resource.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Gets or sets the hediffs applied at different resource levels.
        /// </summary>
        public Dictionary<float, HediffDef> ResourceLevelHediffs { get; set; }

        /// <summary>
        /// Gets or sets the category of this resource.
        /// </summary>
        public string Category { get; set; }
    }

    /// <summary>
    /// Represents a life stage for a race with related properties.
    /// </summary>
    public class RaceLifeStage
    {
        /// <summary>
        /// Gets or sets the unique identifier for this life stage.
        /// </summary>
        public string StageID { get; set; }

        /// <summary>
        /// Gets or sets the display name of the life stage.
        /// </summary>
        public string StageName { get; set; }

        /// <summary>
        /// Gets or sets the minimum age in years for this stage.
        /// </summary>
        public float MinAgeYears { get; set; }

        /// <summary>
        /// Gets or sets the maximum age in years for this stage.
        /// </summary>
        public float MaxAgeYears { get; set; }

        /// <summary>
        /// Gets or sets the hediffs applied during this life stage.
        /// </summary>
        public List<HediffDef> StageHediffs { get; set; }

        /// <summary>
        /// Gets or sets the stat modifiers for this life stage.
        /// </summary>
        public Dictionary<StatDef, float> StatFactors { get; set; }

        /// <summary>
        /// Gets or sets whether abilities are unlocked in this stage.
        /// </summary>
        public bool AbilitiesUnlocked { get; set; }

        /// <summary>
        /// Gets or sets the body size factor for this life stage.
        /// </summary>
        public float BodySizeFactor { get; set; }

        /// <summary>
        /// Gets or sets the health scale factor for this life stage.
        /// </summary>
        public float HealthScaleFactor { get; set; }

        /// <summary>
        /// Gets or sets the resource gain factor for this life stage.
        /// </summary>
        public float ResourceGainFactor { get; set; }

        /// <summary>
        /// Gets or sets the texture path for distinctive visual elements.
        /// </summary>
        public string VisualTexturePath { get; set; }
    }
}