# 🌟 Legendary Races Framework - Architecture Reference

## 📋 Overview

The Legendary Races Framework (LRF) provides a unified foundation for creating complex, legendary race mods for RimWorld. This document serves as a technical reference for the framework architecture, explaining how the various components interact and how to implement race mods using the framework.

## 🏗️ Core Architecture

The framework is designed around a layered architecture:

```
┌─────────────────────────────────────────────────────┐
│                Race Mod Implementation               │
└───────────────────────────┬─────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────┐
│               Legendary Races Framework              │
│                                                      │
│  ┌─────────────┐  ┌────────────┐  ┌───────────────┐  │
│  │Core Systems │  │Infrastructure│  │Content Tools │  │
│  └─────────────┘  └────────────┘  └───────────────┘  │
│         │               │                │           │
│  ┌─────────────────────────────────────────────────┐ │
│  │               Integration Layer                  │ │
│  └─────────────────────────────────────────────────┘ │
└───────────────────────────┬─────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────┐
│              Vanilla Expanded Framework              │
└─────────────────────────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────┐
│                  RimWorld Base Game                  │
└─────────────────────────────────────────────────────┘
```

### 1. Core Systems

These form the foundation of the framework:

#### 1.1 Race Definition System

- **Purpose**: Defines the basic properties, lifecycle, and mechanics of a legendary race
- **Key Components**:
  - `LegendaryRaceDef`: XML definition for a race
  - `IRaceExtension`: Interface for implementing race functionality
  - `RaceLifeStage`: Defines developmental stages for the race
  - `IRaceLifeCycle`: Manages growth, aging, and life stage transitions

#### 1.2 Legendary Characters System

- **Purpose**: Creates and manages special, powerful characters for each race
- **Key Components**:
  - `LegendaryCharacterDef`: XML definition for legendary characters
  - `ILegendaryCharacter`: Interface for implementing legendary character functionality
  - `ILegendaryOrigin`: Handles starting scenarios for legendary characters
  - `IEnhancedRaceMechanic`: Implements amplified versions of race mechanics

#### 1.3 Resource Management

- **Purpose**: Handles race-specific resources and their interactions
- **Key Components**:
  - `RaceResource`: Defines attributes of a race-specific resource
  - `IResourceManager`: Interface for managing resource states and transitions
  - `ResourceHediffLink`: Connects resource levels to pawn effects

#### 1.4 Special Mechanics Handler

- **Purpose**: Implements unique race-specific gameplay mechanics
- **Key Components**:
  - `ISpecialAbility`: Interface for race-specific abilities
  - `EnvironmentalAdaptation`: Defines how races interact with their environment
  - `SpecialNeed`: Implements unique needs beyond vanilla needs

### 2. Infrastructure Components

These provide supporting systems for the core functionality:

#### 2.1 Building System

- **Purpose**: Manages race-specific structures and workstations
- **Key Components**:
  - Race-specific building templates
  - Special workbenches for race crafting
  - Environmental structures

#### 2.2 Research System

- **Purpose**: Handles race-specific technologies and progression
- **Key Components**:
  - Race-specific research trees
  - Technology unlocks tied to race mechanics
  - Knowledge sharing systems

### 3. Integration Layer

Provides compatibility with other mods and systems:

#### 3.1 VEF Integration (Required)

- **Purpose**: Leverages Vanilla Expanded Framework for core functionality
- **Key Components**:
  - Gene system extensions
  - Conditional stat system integration
  - Custom blood implementation
  - Backstory definition integration

#### 3.2 HAR Integration (Optional)

- **Purpose**: Provides enhanced body customization when available
- **Key Components**:
  - `IHARBodyAdapter`: Interface for HAR integration
  - Body type customization
  - Custom race anatomy
  - Fallback systems when HAR is not present

#### 3.3 CE Integration (Optional)

- **Purpose**: Ensures compatibility with Combat Extended
- **Key Components**:
  - `ICECompatibility`: Interface for CE integration
  - `ICEWeaponAdapter`: Adapts race weapons to CE

### 4. Content Creation Tools

Provide utilities for race creators:

- Templates for race implementation
- Validation tools for balance and performance
- Documentation generators

## 💾 Data Flow

```
┌───────────────┐     ┌─────────────────┐     ┌───────────────────┐
│               │     │                 │     │                   │
│    RaceDef    ├────►│  LRFController  ├────►│  IRaceExtension   │
│               │     │                 │     │   Implementation   │
└───────────────┘     └─────────────────┘     └───────────────────┘
                              │                          │
                              ▼                          ▼
┌───────────────┐     ┌─────────────────┐     ┌───────────────────┐
│               │     │                 │     │                   │
│ Legend.CharDef├────►│ LegendarySystem ├────►│ ILegendaryCharacter│
│               │     │                 │     │   Implementation   │
└───────────────┘     └─────────────────┘     └───────────────────┘
                              │                          │
                              ▼                          ▼
┌───────────────┐     ┌─────────────────┐     ┌───────────────────┐
│               │     │                 │     │                   │
│  Game Events  ├────►│ Event Dispatcher├────►│ Resource & Ability │
│               │     │                 │     │     Handlers       │
└───────────────┘     └─────────────────┘     └───────────────────┘
```

## 🔧 Implementation Guide

### Creating a New Legendary Race

1. **Define the Race**:
   - Create a new XML file in the `Defs/RaceDefs` folder
   - Extend the `LegendaryRaceBase` abstract definition
   - Fill in all required fields for your race

2. **Implement Race Functionality**:
   - Create a C# class that implements `IRaceExtension`
   - Register the class in the XML definition
   - Implement race-specific logic and mechanics

3. **Create Resources**:
   - Define race-specific resources in the race definition
   - Implement resource management logic
   - Create UI elements for resource visualization

4. **Define Abilities**:
   - Add race-specific abilities to the race definition
   - Implement ability logic in C# classes
   - Create visual effects for abilities

5. **Set Up Legendary Characters**:
   - Create legendary character definitions
   - Implement enhanced mechanics for these characters
   - Design origin scenarios for starting as legendary characters

6. **Implement VEF Integration**:
   - Use gene extensions for core race properties
   - Set up conditional stat modifiers for environmental adaptations
   - Configure custom blood and resource visualizations

7. **Add HAR/CE Support (Optional)**:
   - Implement HAR body customization if needed
   - Add CE compatibility through the integration interfaces
   - Create fallback behaviors for when these mods are not present

8. **Balance and Optimize**:
   - Use the balance metrics system to track race power levels
   - Optimize performance-critical code paths
   - Implement LOD (Level of Detail) for complex mechanics

## 📊 Core Interface Relationships

```
                ┌───────────────┐
                │ IRaceExtension│
                └───────┬───────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
┌───────▼───────┐ ┌─────▼─────┐ ┌───────▼───────┐
│IResourceManager│ │ISpecialAbility│ │IRaceLifeCycle │
└───────────────┘ └─────────────┘ └───────────────┘
        │               │               │
        └───────────────┼───────────────┘
                        │
                ┌───────▼───────┐
                │IBalanceMetrics │
                └───────────────┘
                        │
                ┌───────▼───────┐
                │IPerformanceMonitor│
                └───────────────┘
                        
                ┌───────────────┐
                │ILegendaryCharacter│
                └───────┬───────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
┌───────▼───────┐ ┌─────▼─────┐ ┌───────▼───────┐
│ILegendaryOrigin│ │IEnhancedRaceMechanic│ │ISpecialAbility │
└───────────────┘ └─────────────┘ └───────────────┘
```

## 🔄 Event Flow

The framework uses an event-driven architecture to handle race-specific events:

1. **Game Events**: Standard RimWorld events (tick, damage, etc.)
2. **Framework Events**: LRF-specific events (resource changes, ability usage)
3. **Race Events**: Race-specific custom events

Events flow through the event dispatcher, which routes them to the appropriate handlers.

## 🛠️ Utility Systems

### Performance Monitoring

- `IPerformanceMonitor`: Tracks execution time of race mechanics
- Provides optimization suggestions and throttling when needed
- Logs performance metrics for debugging

### Migration Handling

- `IMigrationHandler`: Manages version updates gracefully
- Ensures save game compatibility between versions
- Provides fallback behaviors for outdated race definitions

### Balance Testing

- `IBalanceMetrics`: Tracks race power levels and resource economy
- Provides comparative analysis against vanilla races
- Helps identify and address balance issues

## 🧩 XML Schema Reference

### LegendaryRaceDef

```xml
<LRF.LegendaryRaceDef>
  <!-- Basic information -->
  <defName>UniqueID</defName>
  <label>Display Name</label>
  <description>Description</description>
  
  <!-- Race properties -->
  <raceProperties>...</raceProperties>
  
  <!-- Resources -->
  <raceResources>...</raceResources>
  
  <!-- Abilities -->
  <raceAbilities>...</raceAbilities>
  
  <!-- Special mechanics -->
  <specialMechanics>...</specialMechanics>
  
  <!-- Integration settings -->
  <vefIntegration>...</vefIntegration>
  <harIntegration>...</harIntegration>
  <ceIntegration>...</ceIntegration>
  
  <!-- Balance/performance settings -->
  <balanceSettings>...</balanceSettings>
  <performanceSettings>...</performanceSettings>
</LRF.LegendaryRaceDef>
```

### LegendaryCharacterDef

```xml
<LRF.LegendaryCharacterDef>
  <!-- Basic information -->
  <defName>UniqueID</defName>
  <label>Display Name</label>
  <description>Description</description>
  
  <!-- Character properties -->
  <characterProperties>...</characterProperties>
  
  <!-- Traits and skills -->
  <traitProperties>...</traitProperties>
  
  <!-- Special abilities -->
  <uniqueAbilities>...</uniqueAbilities>
  
  <!-- Enhanced race mechanics -->
  <enhancedMechanics>...</enhancedMechanics>
  
  <!-- Equipment and hediffs -->
  <uniqueEquipment>...</uniqueEquipment>
  <uniqueHediffs>...</uniqueHediffs>
  
  <!-- Origin scenario -->
  <originScenario>...</originScenario>
  
  <!-- Progression and story -->
  <progressionPath>...</progressionPath>
  <storyElements>...</storyElements>
  <specialRelationships>...</specialRelationships>
</LRF.LegendaryCharacterDef>
```

## 🔄 VEF Integration Details

For detailed VEF integration reference, see the [VEF Integration Guide](../Documentation/DesignDocuments/VEF%20Integration%20Guide%20for%20Legendary%20Races%20Framework.md).

Key integration points include:

- **Gene Extensions**: Used for race core definition
- **Conditional Stat System**: Enables environmental adaptations
- **Custom Blood System**: Creates visually distinctive races
- **Backstory System**: Defines race-specific backgrounds
- **Weather Effects**: Implements race-specific environmental interactions
- **Shields and Projectiles**: Creates unique combat mechanics

## 📚 Further Resources

- [Core Interface Documentation](../Source/README.md)
- [XML Schema Reference](../DevTools/Documentation/XMLSchema.md)
- [Race Examples](../DevTools/Templates/ExampleRace.md)
- [Legendary Character Examples](../DevTools/Templates/ExampleLegendaryCharacter.md)
- [Balance Guide](../DevTools/Documentation/BalanceGuide.md)
- [Performance Optimization Guide](../DevTools/Documentation/PerformanceGuide.md)

## 📝 Implementation Checklist

When creating a new legendary race, ensure you have:

- [ ] Defined the race XML with all required properties
- [ ] Implemented the `IRaceExtension` interface
- [ ] Set up race resources and management
- [ ] Created unique abilities and mechanics
- [ ] Designed legendary characters
- [ ] Configured VEF integration
- [ ] Added optional HAR/CE support if needed
- [ ] Balanced and optimized race mechanics
- [ ] Tested with and without optional dependencies
- [ ] Documented race-specific features