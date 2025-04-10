# 🌟 Enhanced VEF Integration Guide for Legendary Races Framework

## 📋 Overview

This comprehensive document extends the existing VEF Integration Reference, providing additional integration points and components from the Vanilla Expanded Framework (VEF) that are valuable for the Legendary Races Framework (LRF). When developing race mods or framework components, refer to this guide for proper VEF implementation.

## 🧬 Core Gene Systems

### Basic VEF Gene Integration

The `GeneExtension` from VEF offers extensive customization options for race genes. All of these options can be utilized in LRF race implementations.

**Implementation**:
```xml
<modExtensions>
    <li Class="VanillaGenesExpanded.GeneExtension">
        <!-- Gene customization options -->
    </li>
</modExtensions>
```

**Key Customization Options**:

| Feature | Description | Example |
|---------|-------------|---------|
| `backgroundPath*` | Custom gene backgrounds | `<backgroundPathEndogenes>UI/Genes/RaceGene</backgroundPathEndogenes>` |
| `hediffsToBodyParts` | Apply hediffs to specific body parts | `<hediffsToBodyParts><li><hediff>RaceTrait</hediff><bodyParts><li>Brain</li></bodyParts></li></hediffsToBodyParts>` |
| `hediffToWholeBody` | Apply hediff to whole body | `<hediffToWholeBody>RaceEssence</hediffToWholeBody>` |
| `useSkinColorForFur` | Makes fur use skin color | `<useSkinColorForFur>true</useSkinColorForFur>` |
| `dontColourFur` | Keeps fur untinted | `<dontColourFur>true</dontColourFur>` |
| `useMaskForFur` | Use CutoutComplex shader for fur | `<useMaskForFur>true</useMaskForFur>` |
| `furHidesBody` | Hides body under fur | `<furHidesBody>true</furHidesBody>` |
| `customBlood*` | Race-specific blood | `<customBloodThingDef>RaceBlood</customBloodThingDef>` |
| `forcedBodyType` | Specific body type | `<forcedBodyType>RaceBody</forcedBodyType>` |
| `thingSetMaker` | Starting equipment | `<thingSetMaker>RaceStartingItems</thingSetMaker>` |
| `force*` | Gender controls | `<forceMale>true</forceMale>` |
| `diseaseProgressionFactor` | Disease progression speed | `<diseaseProgressionFactor>0.5</diseaseProgressionFactor>` |
| `pregnancySpeedFactor` | Pregnancy speed | `<pregnancySpeedFactor>1.5</pregnancySpeedFactor>` |
| `hideGene` | Hide from xenotype creator | `<hideGene>true</hideGene>` |

**Complete Implementation Example**:
```xml
<modExtensions>
    <li Class="VanillaGenesExpanded.GeneExtension">
        <backgroundPathEndogenes>UI/Genes/MyRaceGene</backgroundPathEndogenes>
        <hediffToWholeBody>RaceEssence</hediffToWholeBody>
        <forcedBodyType>RaceBody</forcedBodyType>
        <customBloodThingDef>RaceBlood</customBloodThingDef>
        <customBloodIcon>UI/Icons/RaceBlood</customBloodIcon>
        <diseaseProgressionFactor>0.75</diseaseProgressionFactor>
        <pregnancySpeedFactor>1.2</pregnancySpeedFactor>
    </li>
    <li Class="LRF.LegendaryRaceGeneExtension">
        <isLegendaryTrait>true</isLegendaryTrait>
        <raceName>MyRaceName</raceName>
        <raceAbilities>
            <li>RaceAbility1</li>
        </raceAbilities>
    </li>
</modExtensions>
```

### 💉 Custom Blood System

VEF provides a dedicated component for custom blood implementation via `HediffCompProperties_CustomBlood`. This is essential for creating visually distinctive races.

**Implementation**:
```xml
<comps>
    <li Class="VFECore.HediffCompProperties_CustomBlood">
        <!-- Custom blood properties -->
    </li>
</comps>
```

**Key Features**:

| Feature | Description | Example |
|---------|-------------|---------|
| `customBloodThingDef` | Custom blood filth | `<customBloodThingDef>RaceBlood</customBloodThingDef>` |
| `customBloodIcon` | Blood icon for health UI | `<customBloodIcon>UI/Icons/Blood</customBloodIcon>` |
| `customBloodEffect` | Visual effect when damaged | `<customBloodEffect>RaceBloodEffect</customBloodEffect>` |
| `customWoundsFromFleshtype` | Custom wound appearance | `<customWoundsFromFleshtype>RaceFlesh</customWoundsFromFleshtype>` |

**Example Implementation**:
```xml
<HediffDef>
    <defName>RaceEssence</defName>
    <label>race essence</label>
    <description>The fundamental biological nature of this race.</description>
    <comps>
        <li Class="VFECore.HediffCompProperties_CustomBlood">
            <customBloodThingDef>RaceBlood</customBloodThingDef>
            <customBloodIcon>UI/Icons/RaceBlood</customBloodIcon>
            <customBloodEffect>RaceBloodEffect</customBloodEffect>
            <customWoundsFromFleshtype>RaceFlesh</customWoundsFromFleshtype>
        </li>
    </comps>
</HediffDef>

<ThingDef ParentName="BaseFilth">
    <defName>RaceBlood</defName>
    <label>race blood</label>
    <graphicData>
        <texPath>Things/Filth/RaceBlood</texPath>
        <color>(0.5, 0.7, 1.0)</color>
    </graphicData>
    <!-- Filth properties -->
</ThingDef>
```

## 🧠 Thought & Social Systems

### 💭 Thought Extensions

VEF's `ThoughtExtensions` allows customization of thought behavior, which is crucial for race-specific mental states and interactions.

**Implementation**:
```xml
<modExtensions>
    <li Class="VFECore.ThoughtExtensions">
        <removeThoughtsWhenAdded>
            <li>ThoughtToRemove</li>
        </removeThoughtsWhenAdded>
    </li>
</modExtensions>
```

**Usage Example**:
```xml
<ThoughtDef>
    <defName>RaceWaterAffinity</defName>
    <stages>
        <li>
            <label>water affinity</label>
            <description>I feel at home in water.</description>
            <baseMoodEffect>4</baseMoodEffect>
        </li>
    </stages>
    <modExtensions>
        <li Class="VFECore.ThoughtExtensions">
            <removeThoughtsWhenAdded>
                <li>SoakingWet</li>
                <li>WetClothes</li>
            </removeThoughtsWhenAdded>
        </li>
        <li Class="LRF.RaceThoughtExtension">
            <raceName>MyRaceName</raceName>
            <isLegendaryRaceThought>true</isLegendaryRaceThought>
            <triggerConditions>
                <li>InWater</li>
            </triggerConditions>
        </li>
    </modExtensions>
</ThoughtDef>
```

### 📜 Backstory Definition

VEF's `BackstoryDef` system allows creating custom backstories with race-specific traits, skills, and restrictions.

**Implementation**:
```xml
<VFECore.BackstoryDef>
    <defName>RaceBackstory</defName>
    <title>Race Title</title>
    <titleShort>Short</titleShort>
    <baseDescription>Description with [PAWN_nameDef] placeholders.</baseDescription>
    <!-- Additional properties -->
</VFECore.BackstoryDef>
```

**Key Features**:

| Feature | Description | Example |
|---------|-------------|---------|
| `slot` | Backstory slot (Childhood/Adulthood) | `<slot>Childhood</slot>` |
| `spawnCategories` | Categories for filtering | `<spawnCategories><li>RaceCategory</li></spawnCategories>` |
| `skillGains` | Skill bonuses | `<skillGains><Melee>4</Melee></skillGains>` |
| `maleCommonality` | Gender restriction | `<maleCommonality>40</maleCommonality>` |
| `chronologicalAgeRestriction` | Age restriction | `<chronologicalAgeRestriction>18~60</chronologicalAgeRestriction>` |
| `biologicalAgeRestriction` | Biological age restriction | `<biologicalAgeRestriction>18~60</biologicalAgeRestriction>` |
| `forcedTraits` | Traits always applied | `<forcedTraits><li><defName>Trait</defName><degree>1</degree></li></forcedTraits>` |
| `disallowedTraits` | Traits never applied | `<disallowedTraits><li><defName>Trait</defName></li></disallowedTraits>` |
| `workDisables` | Disabled work types | `<workDisables><li>Intellectual</li></workDisables>` |
| `shuffleable` | Can be randomly selected | `<shuffleable>true</shuffleable>` |

**Complete Implementation Example**:
```xml
<VFECore.BackstoryDef>
    <defName>LegendaryRaceChildhood</defName>
    <title>Legendary Youngling</title>
    <titleShort>Youngling</titleShort>
    <baseDescription>[PAWN_nameDef] was born with the ancient power of the race flowing through [PAWN_possessive] veins.</baseDescription>
    <slot>Childhood</slot>
    <spawnCategories>
        <li>LegendaryRaceCategory</li>
    </spawnCategories>
    <skillGains>
        <Melee>3</Melee>
        <Intellectual>2</Intellectual>
    </skillGains>
    <forcedTraits>
        <li>
            <defName>RaceTrait</defName>
            <chance>0.9</chance>
        </li>
    </forcedTraits>
    <disallowedTraits>
        <li>
            <defName>Wimp</defName>
        </li>
    </disallowedTraits>
    <shuffleable>true</shuffleable>
    <modExtensions>
        <li Class="LRF.RaceBackstoryExtension">
            <raceName>MyRaceName</raceName>
            <raceRequirement>true</raceRequirement>
            <frequency>15</frequency>
        </li>
    </modExtensions>
</VFECore.BackstoryDef>
```

## 💪 Environmental Adaptations

### 🌍 Conditional Stat System

VEF provides a powerful `ConditionalStatAffecter_*` system that applies stat modifiers based on specific conditions. This is essential for creating races with environmental adaptations.

**Implementation**:
```xml
<conditionalStatAffecters>
    <li Class="VanillaGenesExpanded.ConditionalStatAffecter_[ConditionType]">
        <statFactors>
            <Stat>Value</Stat>
        </statFactors>
    </li>
</conditionalStatAffecters>
```

**Available Condition Types**:

| Condition | Description | When Active |
|-----------|-------------|------------|
| `Darkness` | In low light | Light level < 0.5 |
| `InLight` | In bright areas | Light level > 0.5 |
| `Water` | On water terrain | On water terrain |
| `DryLand` | On non-water terrain | On non-water terrain |
| `Indoors` | Under roof | In roofed area |
| `Outdoors` | No roof | In unroofed area |
| `OverFortyDegrees` | Hot environment | Temperature > 40°C |
| `BelowZero` | Cold environment | Temperature < 0°C |
| `InPain` | Pawn in pain | Any pain level |
| `InPollution` | In polluted area | On polluted tile |
| `AnyLightSensitivity` | Any light | Light level > 0.11 |
| `NoSunlight` | No direct sunlight | Not under sun |
| `InColonyMap` | In player base | Map is player base |
| `OutsideColonyMap` | Not in player base | Map is not player base |
| `Armor` | Wearing armor | Metal apparel, etc. |

**Example Implementation**:
```xml
<GeneDef>
    <defName>RaceAdaptation</defName>
    <!-- Gene properties -->
    <conditionalStatAffecters>
        <li Class="VanillaGenesExpanded.ConditionalStatAffecter_Water">
            <statFactors>
                <MoveSpeed>1.5</MoveSpeed>
                <SwimSpeed>2.0</SwimSpeed>
            </statFactors>
        </li>
        <li Class="VanillaGenesExpanded.ConditionalStatAffecter_Darkness">
            <statFactors>
                <MoveSpeed>1.25</MoveSpeed>
            </statFactors>
            <statOffsets>
                <NightVisionEfficiency>0.3</NightVisionEfficiency>
            </statOffsets>
        </li>
    </conditionalStatAffecters>
</GeneDef>
```

### 🌤️ Weather Effects Integration

VEF's weather systems can be used to create race-specific environmental interactions, such as races that thrive or suffer in specific weather conditions.

**Weather Letter Extension**:
```xml
<li Class="VFECore.WeatherLetterExtensions">
    <letterDef>NeutralEvent</letterDef>
    <letterTitle>Ancestral Mist</letterTitle>
    <letterText>A mystical mist has formed, empowering [RaceName] inhabitants.</letterText>
</li>
```

**Weather Effects Extension**:
```xml
<li Class="VFECore.WeatherEffectsExtension">
    <ticksInterval>1000</ticksInterval>
    <hediffsToApply>
        <li>
            <hediff>RaceEmpowerment</hediff>
            <severityOffset>0.010</severityOffset>
            <effectMultiplyingStat>PsychicSensitivity</effectMultiplyingStat>
        </li>
    </hediffsToApply>
    <activeOnWeatherPerceived>RaceMist</activeOnWeatherPerceived>
</li>
```

**Custom Weather Overlay**:
```xml
<WeatherDef>
    <defName>RaceMist</defName>
    <label>ancestral mist</label>
    <!-- Weather properties -->
    <overlayClasses>
        <li>VFECore.WeatherOverlay_Custom</li>
    </overlayClasses>
    <modExtensions>
        <li Class="VFECore.WeatherOverlayExtension">
            <overlayPath>Weather/RaceMistOverlay</overlayPath>
            <copyPropertiesFrom>Weather/FogOverlayWorld</copyPropertiesFrom>
            <worldPanDir1>(1, 0.5)</worldPanDir1>
            <worldOverlayPanSpeed1>0.0008</worldOverlayPanSpeed1>
            <worldPanDir2>(0.5, 1)</worldPanDir2>
            <worldOverlayPanSpeed2>0.0006</worldOverlayPanSpeed2>
        </li>
    </modExtensions>
</WeatherDef>
```

## 🧪 Special Race Mechanics

### 💫 Hediff Giver

VEF's `CompProperties_HediffGiver` can be used to create race-specific environmental effects, like auras or radiation.

**Implementation**:
```xml
<li Class="VFECore.CompProperties_HediffGiver">
    <hediffDef>RaceAura</hediffDef>
    <severityIncrease>0.1</severityIncrease>
    <radius>3</radius>
    <stats>
        <li>PsychicSensitivity</li>
    </stats>
</li>
```

### 🛡️ Shield Bubble

VEF's `CompProperties_ShieldBubble` can be used for race-specific shield abilities or protective gear.

**Implementation**:
```xml
<li Class="VFECore.CompProperties_ShieldBubble">
    <EnergyShieldRechargeRate>0.15</EnergyShieldRechargeRate>
    <EnergyShieldEnergyMax>150</EnergyShieldEnergyMax>
    <blockRangedAttack>true</blockRangedAttack>
    <blockMeleeAttack>false</blockMeleeAttack>
    <shieldTexPath>Things/RaceShield</shieldTexPath>
    <showWhenDrafted>true</showWhenDrafted>
    <maxShieldSize>1.8</maxShieldSize>
    <minShieldSize>1.4</minShieldSize>
    <shieldColor>(0.8, 0.6, 1.0, 1.0)</shieldColor>
</li>
```

### 🔥 Expandable Projectiles

VEF's `ExpandableProjectileDef` can be used for race-specific ranged attacks or abilities.

**Implementation**:
```xml
<VFECore.ExpandableProjectileDef ParentName="BaseBullet">
    <defName>RaceEnergyBlast</defName>
    <label>energy blast</label>
    <thingClass>VFECore.FlamethrowProjectile</thingClass>
    <graphicData>
        <texPath>RaceEnergy</texPath>
        <texPathFadeOut>RaceEnergyFadeOut</texPathFadeOut>
        <shaderType>MoteGlow</shaderType>
        <color>(0.6, 0.8, 1.0, 0.8)</color>
    </graphicData>
    <projectile>
        <damageDef>RaceEnergy</damageDef>
        <speed>30</speed>
        <damageAmountBase>8</damageAmountBase>
    </projectile>
    <drawOffscreen>true</drawOffscreen>
    <lifeTimeDuration>60</lifeTimeDuration>
    <widthScaleFactor>0.8</widthScaleFactor>
    <heightScaleFactor>1.1</heightScaleFactor>
    <tickFrameRate>3</tickFrameRate>
    <finalTickFrameRate>6</finalTickFrameRate>
    <tickDamageRate>2</tickDamageRate>
</VFECore.ExpandableProjectileDef>
```

### 🎯 Homing Projectiles

VEF's `CompProperties_HomingProjectile` can be used for race-specific special attacks that track targets.

**Implementation**:
```xml
<comps>
    <li Class="VFECore.CompProperties_HomingProjectile">
        <homingDistanceFractionPassed>0.6</homingDistanceFractionPassed>
        <homingCorrectionTickRate>4</homingCorrectionTickRate>
        <initialDispersionFromTarget>2.0</initialDispersionFromTarget>
        <hitSound>RaceProjectileHit</hitSound>
    </li>
</comps>
```

### 👕 Apparel Hediffs

VEF's `CompProperties_ApparelHediffs` allows race-specific apparel to grant special effects when worn.

**Implementation**:
```xml
<comps>
    <li Class="VFECore.CompProperties_ApparelHediffs">
        <hediffDefnames>
            <li>RaceArmorBonus</li>
        </hediffDefnames>
    </li>
</comps>
```

## 🌐 World and Faction Integration

### 🏙️ Faction Extensions

VEF's `FactionDefExtension` provides configuration options for race-specific factions, including settlement placement, relations with other factions, and more.

**Implementation**:
```xml
<modExtensions>
    <li Class="VFECore.FactionDefExtension">
        <!-- Faction customization options -->
    </li>
</modExtensions>
```

**Key Features**:

| Feature | Description | Example |
|---------|-------------|---------|
| `hasCities` | Whether faction has cities | `<hasCities>true</hasCities>` |
| `startingGoodwillByFactionDefs` | Initial relations with other factions | `<startingGoodwillByFactionDefs><OtherFaction>-100~-80</OtherFaction></startingGoodwillByFactionDefs>` |
| `allowedBiomes` | Biomes where settlements can spawn | `<allowedBiomes><li>Tundra</li></allowedBiomes>` |
| `disallowedBiomes` | Biomes settlements avoid | `<disallowedBiomes><li>Desert</li></disallowedBiomes>` |
| `spawnOnCoastalTilesOnly` | Only settle on coasts | `<spawnOnCoastalTilesOnly>true</spawnOnCoastalTilesOnly>` |
| `shouldHaveLeader` | Whether faction has leader | `<shouldHaveLeader>true</shouldHaveLeader>` |
| `excludeFromCommConsole` | Hide from comm console | `<excludeFromCommConsole>false</excludeFromCommConsole>` |
| `allowedStrategies` | Raid strategies used | `<allowedStrategies><li>RaceStrategy</li></allowedStrategies>` |

**Example Implementation**:
```xml
<FactionDef>
    <defName>RaceFaction</defName>
    <!-- Faction properties -->
    <modExtensions>
        <li Class="VFECore.FactionDefExtension">
            <allowedBiomes>
                <li>Tundra</li>
                <li>IceSheet</li>
                <li>SeaIce</li>
            </allowedBiomes>
            <startingGoodwillByFactionDefs>
                <PlayerColony>-20~20</PlayerColony>
                <PlayerTribe>30~50</PlayerTribe>
                <RivalRaceFaction>-100~-80</RivalRaceFaction>
            </startingGoodwillByFactionDefs>
            <allowedStrategies>
                <li>RaceSpecificStrategy</li>
                <li>ImmediateAttack</li>
            </allowedStrategies>
        </li>
    </modExtensions>
</FactionDef>
```

### 🧩 Object Spawns

VEF's `ObjectSpawnsDef` system can be used to add race-specific objects or pawns to maps during generation.

**Implementation**:
```xml
<VFECore.ObjectSpawnsDef>
    <defName>RaceRuins</defName>
    <thingDef>RaceRuinsPillar</thingDef>
    <allowOnWater>false</allowOnWater>
    <numberToSpawn>5~12</numberToSpawn>
    <allowedTerrains>
        <li>RaceSpecificTerrain</li>
        <li>Soil</li>
    </allowedTerrains>
    <allowedBiomes>
        <li>TemperateForest</li>
        <li>TropicalRainforest</li>
    </allowedBiomes>
</VFECore.ObjectSpawnsDef>
```

**Guardian Spawn Example**:
```xml
<VFECore.ObjectSpawnsDef>
    <defName>RaceGuardian</defName>
    <pawnKindDef>RaceAncientGuardian</pawnKindDef>
    <factionDef>RaceAncientGuardians</factionDef>
    <allowOnWater>false</allowOnWater>
    <numberToSpawn>1~2</numberToSpawn>
    <allowedBiomes>
        <li>TemperateForest</li>
    </allowedBiomes>
</VFECore.ObjectSpawnsDef>
```

## 🌍 Biome Integration

### 🏞️ Biome Extensions

VEF's `BiomeExtension` allows customization of biomes associated with specific races, including terrain swapping, rock type manipulation, and more.

**Implementation**:
```xml
<modExtensions>
    <li Class="VFECore.BiomeExtension">
        <!-- Biome customization options -->
    </li>
</modExtensions>
```

**Key Features**:

| Feature | Description | Example |
|---------|-------------|---------|
| `terrainsToSwap` | Replace terrain types | `<terrainsToSwap><li><from>Gravel</from><to>RaceSoil</to></li></terrainsToSwap>` |
| `skipGenSteps` | Skip generation steps | `<skipGenSteps><li>AnimaTrees</li></skipGenSteps>` |
| `fogColor` | Fog of war color | `<fogColor>(0.6, 0.4, 0.8)</fogColor>` |
| `uniqueRockTypes` | Exclusive rock types | `<uniqueRockTypes><li>RaceStone</li></uniqueRockTypes>` |
| `forceRockTypes` | Always spawn rock types | `<forceRockTypes><li>Marble</li></forceRockTypes>` |
| `disallowRockTypes` | Never spawn rock types | `<disallowRockTypes><li>Slate</li></disallowRockTypes>` |

**Example Implementation**:
```xml
<BiomeDef>
    <defName>RaceHomeland</defName>
    <!-- Biome properties -->
    <modExtensions>
        <li Class="VFECore.BiomeExtension">
            <terrainsToSwap>
                <li>
                    <from>Soil</from>
                    <to>RaceSacredSoil</to>
                </li>
            </terrainsToSwap>
            <skipGenSteps>
                <li>ScatterRoadDebris</li>
                <li>AncientJunkClusters</li>
            </skipGenSteps>
            <fogColor>(0.5, 0.7, 0.9)</fogColor>
            <forceRockTypes>
                <li>RaceStone</li>
                <li>Marble</li>
            </forceRockTypes>
            <onlyAllowForcedRockTypes>true</onlyAllowForcedRockTypes>
        </li>
    </modExtensions>
</BiomeDef>
```

## 🛠️ Advanced Integration Components

### Recipe Inheritance

VEF's `RecipeInheritance.ThingDefExtension` allows race-specific workbenches to inherit recipes from standard workbenches.

**Implementation**:
```xml
<modExtensions>
    <li Class="RecipeInheritance.ThingDefExtension">
        <inheritRecipesFrom>
            <li>TableToInheritFrom</li>
        </inheritRecipesFrom>
        <!-- Optional filters -->
    </li>
</modExtensions>
```

**Example Implementation**:
```xml
<ThingDef ParentName="BenchBase">
    <defName>RaceWorkbench</defName>
    <!-- Workbench properties -->
    <modExtensions>
        <li Class="RecipeInheritance.ThingDefExtension">
            <inheritRecipesFrom>
                <li>TableMachining</li>
                <li>FabricationBench</li>
            </inheritRecipesFrom>
            <allowedProductFilter>
                <categories>
                    <li>Weapons</li>
                    <li>Manufactured</li>
                </categories>
            </allowedProductFilter>
            <disallowedRecipes>
                <li>Make_Gun_Revolver</li>
            </disallowedRecipes>
        </li>
    </modExtensions>
</ThingDef>
```

### Toggable Patches

VEF's `PatchOperationToggableSequence` allows creating optional patches for race mods that can be toggled in settings.

**Implementation**:
```xml
<Operation Class="VFECore.PatchOperationToggableSequence">
    <enabled>true</enabled>
    <label>Race Balance Adjustment:</label>
    <operations>
        <!-- Patch operations -->
    </operations>
</Operation>
```

**Example Implementation**:
```xml
<Operation Class="VFECore.PatchOperationToggableSequence">
    <enabled>true</enabled>
    <label>Race Compatibility with Other Mods:</label>
    <operations>
        <li Class="PatchOperationAdd">
            <xpath>/Defs/ThingDef[defName="SpecialItem"]/comps</xpath>
            <value>
                <li Class="VFECore.CompProperties_ShieldBubble">
                    <EnergyShieldRechargeRate>0.1</EnergyShieldRechargeRate>
                    <EnergyShieldEnergyMax>100</EnergyShieldEnergyMax>
                </li>
            </value>
        </li>
    </operations>
</Operation>
```

### Special Quest Integration

VEF's `QuestNode_GetFaction` allows creating race-specific quests that target particular factions.

**Implementation**:
```xml
<li Class="VFECore.QuestNode_GetFaction">
    <allowEnemy>true</allowEnemy>
    <mustBePermanentEnemy>true</mustBePermanentEnemy>
    <storeAs>enemyFaction</storeAs>
    <factionDef>RaceFaction</factionDef>
</li>
```

## 🔄 Material and Visual Integration

### 🎭 Apparel and Weapon Customization

VEF's `ThingDefExtension` provides customization options for race-specific weapons and apparel.

**Implementation**:
```xml
<modExtensions>
    <li Class="VFECore.ThingDefExtension">
        <!-- Customization options -->
    </li>
</modExtensions>
```

**Key Features**:

| Feature | Description | Example |
|---------|-------------|---------|
| `usableWithShields` | Can be used with shields | `<usableWithShields>true</usableWithShields>` |
| `weaponCarryDrawOffsets` | Offsets carried weapon | `<weaponCarryDrawOffsets><north><drawOffset>(0,0,-0.2)</drawOffset></north></weaponCarryDrawOffsets>` |
| `useFactionColourForPawnKinds` | Use faction colors | `<useFactionColourForPawnKinds><li>RacePawnKind</li></useFactionColourForPawnKinds>` |
| `deepColor` | Color of deep resources | `<deepColor>(0.7, 0.3, 0.3)</deepColor>` |

**Example Implementation**:
```xml
<ThingDef ParentName="BaseMeleeWeapon_Sharp">
    <defName>RaceWeapon</defName>
    <!-- Weapon properties -->
    <modExtensions>
        <li Class="VFECore.ThingDefExtension">
            <usableWithShields>true</usableWithShields>
            <weaponDraftedDrawOffsets>
                <north>
                    <drawOffset>(0.25, 0, 0.36)</drawOffset>
                    <angleOffset>-143</angleOffset>
                </north>
                <east>
                    <drawOffset>(0.1, 0, 0.42)</drawOffset>
                    <angleOffset>-122</angleOffset>
                </east>
                <south>
                    <drawOffset>(-0.25, 0, 0.47)</drawOffset>
                    <angleOffset>-143</angleOffset>
                </south>
                <west>
                    <drawOffset>(-0.1, 0, 0.42)</drawOffset>
                    <angleOffset>122</angleOffset>
                </west>
            </weaponDraftedDrawOffsets>
        </li>
    </modExtensions>
</ThingDef>
```

### 🧵 Stuff Extensions

VEF's `StuffExtension` allows control over the generation of race-specific materials.

**Implementation**:
```xml
<modExtensions>
    <li Class="VFECore.StuffExtension">
        <!-- Material generation options -->
    </li>
</modExtensions>
```

**Example Implementation**:
```xml
<ThingDef ParentName="ResourceBase">
    <defName>RaceMaterial</defName>
    <!-- Material properties -->
    <modExtensions>
        <li Class="VFECore.StuffExtension">
            <structureGenerationCommonalityOffset>100</structureGenerationCommonalityOffset>
            <weaponGenerationCommonalityOffset>50</weaponGenerationCommonalityOffset>
            <apparelGenerationCommonalityOffset>75</apparelGenerationCommonalityOffset>
            <structureGenerationCommonalityFactor>2</structureGenerationCommonalityFactor>
            <weaponGenerationCommonalityFactor>1.5</weaponGenerationCommonalityFactor>
            <apparelGenerationCommonalityFactor>1.8</apparelGenerationCommonalityFactor>
        </li>
    </modExtensions>
</ThingDef>
```

## 📋 Implementation Checklist

When implementing a new legendary race using VEF components, follow this checklist for completeness:

### Core Race Definition
- [ ] Base gene definition with `GeneExtension`
- [ ] Custom blood system via `HediffCompProperties_CustomBlood`
- [ ] Race-specific environmental adaptations with `ConditionalStatAffecter_*`
- [ ] Unique thought patterns with `ThoughtExtensions`
- [ ] Race backstories via `BackstoryDef`

### Race Mechanics
- [ ] Special abilities and powers
- [ ] Environmental interactions
- [ ] Unique resources and needs
- [ ] Visual distinctiveness

### Faction and World Integration
- [ ] Race-specific faction with `FactionDefExtension`
- [ ] Settlement placement rules
- [ ] Relations with other factions
- [ ] Race-specific biomes with `BiomeExtension`
- [ ] Environmental objects with `ObjectSpawnsDef`

### Equipment and Material
- [ ] Race-specific equipment with `ThingDefExtension`
- [ ] Unique materials with `StuffExtension`
- [ ] Special effects for apparel with `CompProperties_ApparelHediffs`
- [ ] Distinctive weapons with special projectiles

### Legendary Character Implementation
- [ ] Unique legendary character backstories
- [ ] Enhanced genetic traits
- [ ] Special abilities and powers
- [ ] Origin stories and progression paths

## 🔍 Best Practices

1. **Modularity** - Design components to be modular and reusable
2. **Layered Integration** - Build race features in layers, from core VEF to LRF extensions
3. **Performance Monitoring** - Test race mechanics for performance impact
4. **Documentation** - Document all VEF integrations for maintainability
5. **Compatibility** - Ensure compatibility with optional integration modules (HAR, CE)
6. **Fallback Systems** - Implement graceful fallbacks when optional dependencies are missing
7. **Balance Testing** - Test race mechanics against vanilla and other modded races for balance

## 📚 Resources and References

- **VEF Documentation Wiki**: [Link placeholder]
- **Example Race Implementations**: [Link placeholder]
- **LRF GitHub Repository**: [Link placeholder]
- **Troubleshooting Guide**: [Link placeholder]
