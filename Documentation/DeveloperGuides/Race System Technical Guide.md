# 🌟 Legendary Races Framework - Race System Technical Guide

## 📋 Overview

This technical guide explains how to implement and extend the Race Definition System within the Legendary Races Framework (LRF). The race system is the foundation of the framework, providing the structure for creating unique legendary races with distinctive mechanics, abilities, and life cycles.

## 🏗️ Architecture

The race system is built around these core components:

1. **Race Definition (XML)** - Defines properties of a legendary race
2. **Race Handler (C#)** - Implements the race functionality
3. **Resource Management** - Handles race-specific resources
4. **Life Cycle Management** - Manages growth, aging, and life stages
5. **Ability System** - Implements race-specific abilities
6. **Legendary Characters** - Special characters of a race with enhanced abilities
7. **Balance Metrics** - Tools for ensuring game balance
8. **Performance Monitoring** - Tools for optimizing performance

## 🔧 Creating a New Legendary Race

### Step 1: Create the Race Definition XML

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
  <LRF.LegendaryRaceDef ParentName="LegendaryRaceBase">
    <!-- Basic information -->
    <defName>MyLegendaryRace</defName>
    <label>my legendary race</label>
    <description>A description of my legendary race.</description>
    
    <!-- Race properties -->
    <raceProperties>
      <raceName>My Race</raceName>
      <raceDescription>Detailed description of the race, its origins, and notable features.</raceDescription>
      <raceIcon>UI/Races/MyRaceIcon</raceIcon>
      
      <!-- Associated Gene Definition -->
      <coreGeneDef>MyRaceCoreGene</coreGeneDef>
      <secondaryGeneDefs>
        <li>MyRaceSecondaryGene1</li>
        <li>MyRaceSecondaryGene2</li>
      </secondaryGeneDefs>
      
      <!-- Life Cycle -->
      <lifeSpanYears>120</lifeSpanYears>
      <maxLifeSpanYears>160</maxLifeSpanYears>
      <lifeStages>
        <li>
          <stageID>Child</stageID>
          <stageName>Child</stageName>
          <minAgeYears>0</minAgeYears>
          <maxAgeYears>12</maxAgeYears>
          <bodySizeFactor>0.6</bodySizeFactor>
          <healthScaleFactor>0.7</healthScaleFactor>
          <resourceGainFactor>0.6</resourceGainFactor>
          <abilitiesUnlocked>false</abilitiesUnlocked>
          <stageHediffs>
            <li>MyRaceChildHediff</li>
          </stageHediffs>
          <statFactors>
            <li>
              <statDef>MoveSpeed</statDef>
              <value>0.85</value>
            </li>
          </statFactors>
        </li>
        <!-- Additional life stages... -->
      </lifeStages>
    </raceProperties>
    
    <!-- Race Resources -->
    <raceResources>
      <li>
        <resourceID>MyRaceResource</resourceID>
        <resourceName>Race Resource</resourceName>
        <resourceDescription>Description of the race resource.</resourceDescription>
        <minValue>0</minValue>
        <maxValue>100</maxValue>
        <defaultValue>50</defaultValue>
        <isVisible>true</isVisible>
        <regenerationRate>1.0</regenerationRate>
        <iconPath>UI/Resources/MyResource</iconPath>
        <category>Power</category>
        <resourceLevelHediffs>
          <li>
            <level>20</level>
            <hediff>MyResourceLowHediff</hediff>
          </li>
          <li>
            <level>80</level>
            <hediff>MyResourceHighHediff</hediff>
          </li>
        </resourceLevelHediffs>
      </li>
    </raceResources>
    
    <!-- Race Abilities -->
    <raceAbilities>
      <li>
        <abilityID>MyRaceAbility</abilityID>
        <abilityName>Race Ability</abilityName>
        <abilityDescription>Description of the race ability.</abilityDescription>
        <iconPath>UI/Abilities/MyAbility</iconPath>
        <cooldownTicks>60000</cooldownTicks>
        <isPassive>false</isPassive>
        <resourceCosts>
          <li>
            <resourceID>MyRaceResource</resourceID>
            <cost>25</cost>
          </li>
        </resourceCosts>
        <statRequirements>
          <li>
            <statDef>PsychicSensitivity</statDef>
            <value>0.8</value>
          </li>
        </statRequirements>
        <hediffsApplied>
          <li>MyAbilityHediff</li>
        </hediffsApplied>
        <effectRadius>3.0</effectRadius>
        <targetingType>SingleTarget</targetingType>
        <abilityClass>MyMod.MyRaceAbility</abilityClass>
      </li>
    </raceAbilities>
    
    <!-- Special Race Mechanics -->
    <specialMechanics>
      <environmentalAdaptations>
        <li>
          <environmentType>Aquatic</environmentType>
          <statModifications>
            <li>
              <statDef>MoveSpeed</statDef>
              <value>1.2</value>
            </li>
          </statModifications>
          <needModifications>
            <li>
              <needDef>Food</needDef>
              <factor>0.8</factor>
            </li>
          </needModifications>
        </li>
      </environmentalAdaptations>
      <!-- Additional mechanics... -->
    </specialMechanics>
    
    <!-- Integration settings -->
    <vefIntegration>
      <isUsingVEF>true</isUsingVEF>
      <vefGeneExtension>
        <backgroundPathEndogenes>UI/Genes/MyRaceGene</backgroundPathEndogenes>
        <hediffToWholeBody>MyRaceEssence</hediffToWholeBody>
        <customBloodThingDef>MyRaceBlood</customBloodThingDef>
        <customBloodIcon>UI/Icons/MyRaceBlood</customBloodIcon>
        <diseaseProgressionFactor>0.75</diseaseProgressionFactor>
        <pregnancySpeedFactor>1.2</pregnancySpeedFactor>
      </vefGeneExtension>
      <!-- Additional VEF integration... -->
    </vefIntegration>
    
    <!-- HAR Integration (Optional) -->
    <harIntegration>
      <isUsingHAR>true</isUsingHAR>
      <bodyTypes>
        <li>MyRaceBodyType</li>
      </bodyTypes>
      <fallbackBodyType>Thin</fallbackBodyType>
      <!-- Additional HAR configuration... -->
    </harIntegration>
    
    <!-- Balance settings -->
    <balanceSettings>
      <powerLevel>3</powerLevel>
      <resourceMultiplier>1.0</resourceMultiplier>
      <abilityPowerMultiplier>1.0</abilityPowerMultiplier>
      <difficultyScaling>true</difficultyScaling>
    </balanceSettings>
    
    <!-- Performance settings -->
    <performanceSettings>
      <usePerformanceMonitoring>true</usePerformanceMonitoring>
      <resourceUpdateInterval>250</resourceUpdateInterval>
      <abilityCheckInterval>60</abilityCheckInterval>
      <environmentalCheckInterval>250</environmentalCheckInterval>
    </performanceSettings>
  </LRF.LegendaryRaceDef>
</Defs>
```

### Step 2: Create Supporting Defs

Your race will need supporting defs such as:

1. **Gene Definitions**
```xml
<GeneDef>
  <defName>MyRaceCoreGene</defName>
  <label>my race core gene</label>
  <description>The core gene of my legendary race.</description>
  <!-- Gene properties... -->
</GeneDef>
```

2. **Hediff Definitions**
```xml
<HediffDef>
  <defName>MyRaceEssence</defName>
  <label>race essence</label>
  <description>The fundamental biological nature of this race.</description>
  <!-- Hediff properties... -->
</HediffDef>
```

3. **Required Resources (Blood, etc.)**
```xml
<ThingDef ParentName="BaseFilth">
  <defName>MyRaceBlood</defName>
  <label>race blood</label>
  <!-- Blood properties... -->
</ThingDef>
```

### Step 3: Create a Custom Race Handler (Optional)

If the default race handler doesn't meet your needs, you can create a custom one:

```csharp
using System.Collections.Generic;
using RimWorld;
using Verse;
using LegendaryRacesFramework;

namespace MyMod
{
    public class MyRaceHandler : IRaceExtension
    {
        private readonly LegendaryRaceDef raceDef;
        // Other fields...
        
        public MyRaceHandler(LegendaryRaceDef raceDef)
        {
            this.raceDef = raceDef;
            // Initialize...
        }
        
        // Implement required properties and methods
        public string RaceID => raceDef.defName;
        public string RaceName => raceDef.raceProperties.raceName;
        // Other properties...
        
        public void Initialize()
        {
            // Custom initialization logic
        }
        
        public bool IsMemberOfRace(Pawn pawn)
        {
            // Custom race membership detection
            return false;
        }
        
        // Other methods...
    }
}
```

### Step 4: Register Your Custom Race Handler

Register your custom race handler in a postfix:

```csharp
using HarmonyLib;
using Verse;
using LegendaryRacesFramework;

namespace MyMod
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            // Register race handler
            LegendaryRacesFrameworkMod.RegisterRaceExtension("MyLegendaryRace", typeof(MyRaceHandler));
        }
    }
}
```

### Step 5: Create Custom Abilities (Optional)

If your race has unique abilities, you can create custom implementations:

```csharp
using System.Collections.Generic;
using RimWorld;
using Verse;
using LegendaryRacesFramework;

namespace MyMod
{
    public class MyRaceAbility : ISpecialAbility
    {
        private readonly RaceAbilityDef abilityDef;
        // Other fields...
        
        public MyRaceAbility(RaceAbilityDef abilityDef)
        {
            this.abilityDef = abilityDef;
        }
        
        // Implement required properties
        public string AbilityID => abilityDef.abilityID;
        public string AbilityName => abilityDef.abilityName;
        // Other properties...
        
        // Implement required methods
        public bool CanUseAbility(Pawn pawn)
        {
            // Custom ability requirements
            return false;
        }
        
        public bool UseAbility(Pawn pawn, LocalTargetInfo target)
        {
            // Custom ability effects
            return false;
        }
        
        // Other methods...
    }
}
```

### Step 6: Test Your Race

Create a testing scenario with your race to ensure it works correctly.

## 📊 Race System API Reference

### Core Interfaces

#### `IRaceExtension`
The main interface for race implementations.

```csharp
public interface IRaceExtension
{
    // Basic race information
    string RaceID { get; }
    string RaceName { get; }
    string RaceDescription { get; }
    
    // Race genetics
    GeneDef CoreGeneDef { get; }
    List<GeneDef> SecondaryGeneDefs { get; }
    
    // Sub-systems
    IResourceManager ResourceManager { get; }
    List<ISpecialAbility> RaceAbilities { get; }
    IRaceLifeCycle LifeCycleManager { get; }
    List<ILegendaryCharacter> LegendaryCharacters { get; set; }
    IBalanceMetrics BalanceMetrics { get; }
    
    // Core methods
    void Initialize();
    void PostLoadInit();
    bool IsMemberOfRace(Pawn pawn);
    Dictionary<string, float> GetCustomRaceStats(Pawn pawn);
}
```

#### `IResourceManager`
Manages race-specific resources.

```csharp
public interface IResourceManager
{
    string RaceID { get; }
    List<RaceResource> ManagedResources { get; }
    
    float GetResourceValue(Pawn pawn, string resourceDefName);
    void SetResourceValue(Pawn pawn, string resourceDefName, float value);
    void AdjustResourceValue(Pawn pawn, string resourceDefName, float delta);
    void RegisterResource(RaceResource resource);
    List<Hediff> GetResourceHediffs(Pawn pawn);
}
```

#### `IRaceLifeCycle`
Manages race growth, aging, and life stages.

```csharp
public interface IRaceLifeCycle
{
    string RaceID { get; }
    List<RaceLifeStage> LifeStages { get; }
    float AverageLifespanYears { get; }
    float MaximumLifespanYears { get; }
    
    RaceLifeStage GetCurrentLifeStage(Pawn pawn);
    float GetAgingFactor(Pawn pawn);
    void HandleLifeStageTransition(Pawn pawn, RaceLifeStage fromStage, RaceLifeStage toStage);
    void ApplyLifeStageHediffs(Pawn pawn, RaceLifeStage lifeStage);
}
```

#### `ISpecialAbility`
Defines race-specific abilities.

```csharp
public interface ISpecialAbility
{
    string AbilityID { get; }
    string AbilityName { get; }
    string AbilityDescription { get; }
    string IconPath { get; }
    int CooldownTicks { get; }
    bool IsPassive { get; }
    Dictionary<string, float> ResourceCosts { get; }
    Dictionary<StatDef, float> StatRequirements { get; }
    
    bool CanUseAbility(Pawn pawn);
    bool UseAbility(Pawn pawn, LocalTargetInfo target);
    int GetCurrentCooldown(Pawn pawn);
}
```

#### `ILegendaryCharacter`
Defines special characters of a race.

```csharp
public interface ILegendaryCharacter
{
    string CharacterID { get; }
    string RaceID { get; }
    string CharacterName { get; }
    string Backstory { get; }
    long BirthTicks { get; }
    List<ISpecialAbility> UniqueAbilities { get; }
    List<TraitDef> UniqueTraits { get; }
    List<HediffDef> UniqueHediffs { get; }
    
    void ApplyToCharacter(Pawn pawn);
    ScenarioDef GetStartingScenario();
}
```

### Supporting Classes

#### `RaceResource`
Defines a race-specific resource.

```csharp
public class RaceResource
{
    public string ResourceID { get; set; }
    public string ResourceName { get; set; }
    public string ResourceDescription { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float DefaultValue { get; set; }
    public bool IsVisible { get; set; }
    public float RegenerationRate { get; set; }
    public string IconPath { get; set; }
    public Dictionary<float, HediffDef> ResourceLevelHediffs { get; set; }
    public string Category { get; set; }
}
```

#### `RaceLifeStage`
Defines a life stage for a race.

```csharp
public class RaceLifeStage
{
    public string StageID { get; set; }
    public string StageName { get; set; }
    public float MinAgeYears { get; set; }
    public float MaxAgeYears { get; set; }
    public List<HediffDef> StageHediffs { get; set; }
    public Dictionary<StatDef, float> StatFactors { get; set; }
    public bool AbilitiesUnlocked { get; set; }
    public float BodySizeFactor { get; set; }
    public float HealthScaleFactor { get; set; }
    public float ResourceGainFactor { get; set; }
    public string VisualTexturePath { get; set; }
}
```

## 🧩 Integration with Other Systems

### VEF Integration

The race system integrates with Vanilla Expanded Framework through:

1. **Gene System Extensions** - Using VEF's gene framework for race properties
2. **Conditional Stat System** - For environmental adaptations
3. **Custom Blood Implementation** - For race-specific blood and wounds
4. **Backstory Definition** - For race-specific backgrounds

### HAR Integration (Optional)

When Humanoid Alien Races is available, the race system can:

1. **Customize Body Types** - Define race-specific body types
2. **Modify Anatomical Structure** - Change body part configurations
3. **Adjust Rendering** - Modify how pawns are drawn
4. **Implement Fallbacks** - Gracefully handle when HAR is not present

### CE Integration (Optional)

When Combat Extended is available, the race system can:

1. **Define Combat Properties** - Set race-specific combat stats
2. **Adapt Weapons** - Make race weapons work with CE
3. **Balance Abilities** - Ensure abilities are balanced in CE's combat model

## 🚀 Best Practices

### Performance Optimization

1. **Use Appropriate Update Intervals**
   - Don't update resources or check conditions every tick
   - Use the performance settings to control update frequency

2. **Limit Hediff Application**
   - Apply hediffs only when necessary
   - Remove unused hediffs

3. **Optimize Environmental Checks**
   - Cache environmental conditions when possible
   - Use the built-in performance monitoring to identify bottlenecks

### Balance Considerations

1. **Power Scaling**
   - Use the `powerLevel` setting to guide overall race strength
   - Test race abilities against vanilla pawns for balance

2. **Resource Economy**
   - Ensure resources regenerate at a balanced rate
   - Make ability costs meaningful but not frustrating

3. **Life Cycle Balance**
   - Balance life stages for a smooth progression
   - Child stages should have significant limitations

### Integration Guidelines

1. **VEF Integration**
   - Always use VEF systems when available instead of creating duplicates
   - Follow VEF patterns for consistent behavior

2. **Optional Dependencies**
   - Always check if optional mods are active before using their features
   - Provide reasonable fallbacks when optional mods are not present

## 🔍 Troubleshooting Common Issues

### Race Not Being Detected

1. Check that the core gene or hediffs are correctly defined
2. Ensure `IsMemberOfRace` is implemented correctly
3. Verify that the race def is properly loaded in the database

### Resources Not Working

1. Check resource definitions in XML
2. Verify resource regeneration rate is set appropriately
3. Ensure resource hediffs are correctly defined

### Abilities Not Functioning

1. Check ability class implementation
2. Verify cooldowns are being tracked correctly
3. Ensure resource costs are set appropriately

### Life Stages Not Transitioning

1. Check life stage age ranges for gaps or overlaps
2. Verify that the life stages are sorted by age
3. Ensure transition logic is working in `HandleLifeStageTransition`

## 📚 Additional Resources

- [VEF Integration Guide](../DesignDocuments/VEF%20Integration%20Guide%20for%20Legendary%20Races%20Framework.md)
- [Framework Architecture Reference](../DesignDocuments/Legendary%20Races%20Framework%20-%20Architecture%20Reference.md)
- [Race Example Implementation](../../DevTools/Templates/ExampleRace/)
- [Legendary Character Guide](./Legendary%20Character%20System%20Guide.md)