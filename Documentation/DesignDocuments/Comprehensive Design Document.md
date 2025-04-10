# Legendary Races Framework (LRF)
## Comprehensive Design Document

## 📜 Overview & Vision

The Legendary Races Framework (LRF) serves as a unified foundation for creating complex, legendary race mods that extend beyond standard race implementations. Each race will feature unique mechanics, buildings, abilities, and gameplay elements while maintaining consistency and compatibility. The framework builds upon Vanilla Expanded Framework (VEF) as its core dependency and offers optional integration with Humanoid Alien Races (HAR) for expanded body customization.

## 🎯 Core Design Goals

1. **Extensibility** - Provide clean extension points for each legendary race
2. **Consistency** - Ensure all legendary races follow similar patterns and quality standards
3. **Integration** - Seamless compatibility with vanilla game systems and popular mods
4. **Uniqueness** - Enable truly distinctive gameplay for each legendary race
5. **Maintainability** - Minimize redundant code across race mods
6. **Balance** - Provide tools and guidelines for maintaining gameplay balance
7. **Performance** - Ensure race mechanics don't adversely impact game performance

## 🏗️ Framework Architecture

### 1. Core Systems

#### Legendary Characters System
- **Unique Individual Definition** - Framework for creating special characters for each race
- **Enhanced Abilities** - Lore-based powers beyond standard race capabilities
- **Origin Stories** - Narrative elements connecting characters to race history
- **Playable Legends** - Systems to allow starting the game as legendary figures
- **Power Progression** - Mechanics for legendary character development

#### Race Definition System
- **Base race properties** - Inherent stat modifiers, lifespan, needs
- **Life cycle management** - Aging, growth stages, special conditions
- **Racial abilities** - Passive and active abilities available to the race
- **Genetic traits** - Hereditary features and their inheritance rules
- **VEF Extensions** - Utilizes and extends VEF gene systems when appropriate

#### Resource Management
- **Custom resources** - Framework for race-specific resources (like hemogen)
- **Resource generation/consumption** - Systems for gaining and using race-specific resources
- **Resource visualization** - UI elements for displaying race resources
- **Resource economy balancing** - Tools for maintaining economic balance
- **VEF Integration** - Leverages existing VEF resource systems when applicable

#### Special Mechanics Handler
- **Unique need trackers** - For race-specific needs beyond standard needs
- **Custom interaction system** - For race-specific interactions with objects/pawns
- **Event system** - For racial events, rituals, transformations
- **Ability cooldown management** - For handling timed abilities
- **VEF Hooks** - Uses VEF event and interaction frameworks as foundation

### 2. Infrastructure Components

#### Building System
- **Race-specific building templates** - For consistent building definitions
- **Special workbenches** - For race-specific crafting/research
- **Environmental structures** - Special structures that affect the environment
- **Building upgrade paths** - For structures that can evolve or improve
- **VEF Building Extensions** - Extends VEF building frameworks when available

#### Research System
- **Extended research tree** - For race-specific technologies
- **Special research requirements** - Custom prerequisites based on race
- **Technology unlocks** - Management of race-specific unlockable content
- **Knowledge sharing** - How research transfers between different races
- **VEF Research Integration** - Builds upon VEF research systems

### 3. Integration Layers

#### VEF Integration (Required)
- **Extension modules** - Direct extensions of VEF systems
- **Override handling** - Managing conflicts with VEF default behaviors
- **VEF hook utilization** - Leveraging existing VEF functionality
- **Update compatibility** - Handling VEF version changes
- **System reuse** - Using VEF's established systems instead of creating duplicates

#### HAR Integration (Optional)
- **Body type extensions** - Support for non-standard body structures
- **Custom race anatomy** - Special body parts and configurations
- **Visual adaptation layer** - Handling custom body rendering
- **Fallback system** - Default appearance when HAR is not present
- **Feature toggling** - Enabling/disabling HAR-dependent features

#### Combat Extended Integration (Optional)
- **Race-specific combat properties** - Custom CE stats for legendary races
- **Special weapon adaptation** - Integration of race weapons with CE systems
- **Ability compatibility layer** - Making race abilities work with CE mechanics
- **Custom ammunition types** - Race-specific ammunition when appropriate
- **Legendary character combat prowess** - Enhanced CE capabilities for legendary figures
- **Balance adjustments** - Ensuring races remain balanced within CE's combat model

#### Mod Compatibility
- **Major mods compatibility layer** - Specific patches for popular mods
- **General compatibility system** - For handling typical interaction patterns
- **Cross-race interaction API** - For handling how legendary races interact with each other
- **Plugin system** - For third-party mods to extend legendary races

### 4. Content Creation Tools

#### Legendary Character Development
- **Unique Advantages System** - Framework for creating race-specific enhancements
- **Lore-Based Abilities** - Tools for implementing story-driven powers
- **Advanced Gene Combinations** - Special genetic traits unique to legendary figures
- **Character-Specific Quest Lines** - Narrative arcs for each legendary individual
- **Starting Scenarios** - Custom game starts featuring legendary characters

#### Visual Assets Management
- **Race appearance system** - For handling custom body types, features
- **Animation extensions** - For race-specific animations
- **Special effects system** - Visual effects for abilities and actions
- **Style guide enforcement** - For maintaining visual consistency
- **HAR-dependent visuals** - Extended options when HAR is available

#### Storytelling Elements
- **Race-specific incidents** - Special events tied to legendary races
- **Faction integration** - How the race integrates with faction systems
- **Special character roles** - Leadership or special positions within the race
- **World impact** - How the race affects the game world over time

### 5. Gameplay Balance Framework

#### Power Scaling
- **Ability power curves** - Templates for balanced ability progression
- **Resource economy models** - Ensuring resource systems remain balanced
- **Difficulty scaling** - How race mechanics adapt to game difficulty
- **Comparative metrics** - Tools for comparing race power levels

#### Progression Design
- **Advancement paths** - Templates for race progression
- **Milestone systems** - Framework for unlocking race features
- **Challenge scaling** - Ensuring appropriate challenge throughout gameplay
- **Reward structures** - Guidelines for balanced rewards

### 6. Performance & Optimization

#### Performance Monitoring
- **Metric tracking** - Systems to monitor performance impact
- **Optimization guidelines** - Best practices for efficient race mechanics
- **Throttling mechanisms** - For limiting intensive processes when needed
- **LOD (Level of Detail) systems** - For scaling complexity based on hardware

#### Technical Stability
- **Error handling** - Robust error catching and reporting
- **Safe mode** - Fallback behaviors when errors occur
- **Version migration** - Tools for handling framework updates
- **Backward compatibility** - Supporting races built on earlier framework versions

### 7. Player Experience Systems

#### UI/UX Standards
- **Interface consistency** - Guidelines for race-specific UI elements
- **Notification framework** - Standard approach to alerting players
- **Onboarding systems** - Tools for introducing complex race mechanics
- **Accessibility features** - Ensuring race mechanics are accessible to all players

#### Documentation Generation
- **In-game documentation** - Systems for explaining race mechanics to players
- **Help systems** - Context-sensitive guidance for complex features
- **Tutorial frameworks** - For teaching race-specific gameplay

## 🔧 Implementation Strategy

1. **Core Framework Mod** - Contains all shared systems and APIs (requires VEF)
2. **Race-Specific Mods** - Each implements one legendary race using the framework
3. **HAR Integration Module** - Optional module for HAR-dependent features
4. **Optional Integration Packs** - For deeper integration with specific major mods
5. **Development Tools** - Separate tools for race creators

## 📋 Race Mod Template

Each legendary race mod should include:

1. **Race Definition**
   - Basic traits and attributes
   - Life stages and aging process
   - Inherent abilities
   - Genetic diversity options
   - HAR body type definitions (if using HAR integration)

2. **Unique Mechanics**
   - Custom resources (if any)
   - Special needs or conditions
   - Unique gameplay loops
   - Special abilities and powers

3. **Infrastructure**
   - Race-specific buildings
   - Special workbenches or stations
   - Environmental requirements
   - Territory mechanics

4. **Progression**
   - Research tree extensions
   - Ability unlocks and progression
   - Evolution or transformation paths
   - Achievement systems

5. **Assets**
   - Visual representations
   - Sound effects
   - Special animations
   - UI elements
   - Custom body assets (when using HAR)

6. **Balance Documentation**
   - Power level guidelines
   - Resource economy specs
   - Progression curve details
   - Comparative analysis

7. **Performance Considerations**
   - Optimization strategies used
   - Performance impact assessment
   - LOD implementation details
   - Recommended settings

8. **Legendary Characters**
   - Progenitor or historical figures for the race
   - Enhanced race-specific abilities and traits
   - Unique genetic advantages tied to race lore
   - Custom mechanics not available to regular race members
   - Starting scenario for playing as the legendary figure
   - Progression paths for unlocking full potential

## 🔄 Development Workflow

1. **Framework Development** - Create core systems first with VEF integration
2. **HAR Module Development** - Create optional HAR integration layer
3. **Development Tools** - Build tools to assist race creation
4. **Prototype Race** - Develop one complete legendary race as proof of concept
5. **Framework Refinement** - Update framework based on lessons learned
6. **Documentation Creation** - Comprehensive developer guides
7. **Additional Races** - Develop subsequent races using the refined framework
8. **Community Resources** - Provide resources for other modders

## 📊 Technical Specifications

### Dependencies
- **Required**: Vanilla Expanded Framework (VEF)
- **Optional**: Humanoid Alien Races (HAR)
- **Optional**: Combat Extended (CE)

### File Structure
```
LegendaryRacesFramework/
├── About/
│   ├── About.xml            # Includes VEF as required dependency
│   └── Manifest.xml
├── Assemblies/
│   └── LRF.dll
├── Defs/
│   ├── RaceDefs/
│   ├── ThingDefs/
│   ├── HediffDefs/
│   ├── AbilityDefs/
│   ├── LegendaryFigureDefs/ # New def type for legendary characters
│   ├── ScenarioDefs/        # Custom scenarios for legendary characters
│   └── ...
├── Languages/
├── Patches/
│   ├── CorePatches/
│   ├── VEFIntegration/
│   ├── HARIntegration/      # Conditionally loaded when HAR is present
│   ├── CEIntegration/       # Conditionally loaded when Combat Extended is present
│   └── ModCompatibility/
├── Systems/
│   ├── LegendaryOrigins/    # Systems for starting as legendary characters
│   └── LegendaryProgression/ # Progression systems for legendary characters
├── Textures/
├── Sounds/
└── DevTools/
    ├── Templates/
    ├── Validators/
    └── Documentation/
```

### Key Interfaces
- **IRaceExtension** - For race-specific implementations
- **IResourceManager** - For custom resource handling
- **ISpecialAbility** - For race abilities
- **IRaceLifeCycle** - For handling growth and aging
- **IBalanceMetrics** - For tracking balance statistics
- **IPerformanceMonitor** - For monitoring performance impact
- **IMigrationHandler** - For version migrations
- **IHARBodyAdapter** - For handling HAR body type integration (when available)
- **ICECompatibility** - For Combat Extended integration (when available)
- **ICEWeaponAdapter** - For adapting race weapons to CE systems
- **ILegendaryCharacter** - Base interface for legendary character implementation
- **ILegendaryOrigin** - For handling starting as legendary characters
- **IEnhancedRaceMechanic** - For amplified versions of race mechanics for legendary figures

## 🔍 Evaluation Criteria

A successful legendary race implementation should:
1. Provide unique gameplay that feels distinctly different
2. Integrate seamlessly with vanilla systems and VEF
3. Have balanced progression and challenges
4. Maintain performance without excessive overhead
5. Present consistent visual and thematic elements
6. Include comprehensive documentation
7. Follow established framework patterns
8. Pass all validation tests
9. Properly handle presence/absence of optional dependencies (like HAR)

## 🚀 Future Expansion Possibilities

- **Cross-race interactions** - Special interactions between different legendary races
- **Legendary factions** - Full faction implementations of legendary races
- **World-altering abilities** - Powers that can modify world map or environments
- **Legacy systems** - Multi-generational gameplay features
- **Environmental adaptation** - Race-specific responses to environments
- **Cultural systems** - Deeper social and cultural mechanics
- **Legendary creatures** - Extension of framework to non-humanoid entities
- **Special NPC classes** - Unique NPCs related to legendary races
- **Advanced HAR integrations** - Further body customization for complex race types
- **Legendary Character Rivalries** - Special interactions between different legendary figures
- **Dynasty Systems** - Mechanics for legendary characters to create successors
- **Artifact Systems** - Unique items tied to legendary character lore

## 📚 Developer Resources

- **Documentation Wiki** - Comprehensive guides and reference
- **Example Implementations** - Sample code for common patterns
- **Best Practices Guide** - Recommended approaches
- **Troubleshooting Guide** - Common issues and solutions
- **Performance Optimization Guide** - Tips for efficient implementation
- **Style Guide** - For maintaining visual and thematic consistency
- **Balance Guide** - For ensuring fair and fun gameplay
- **VEF Integration Guide** - How to properly leverage VEF systems
- **HAR Optional Features Guide** - How to implement HAR-dependent features
- **Legendary Character Creation Guide** - Templates and best practices for legendary figures
- **Lore-Based Ability Design** - Methodology for creating story-driven mechanics
- **Origin Scenario Design** - How to create compelling legendary character starts
