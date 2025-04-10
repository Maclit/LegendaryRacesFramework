# 🔧 Legendary Races Framework - Build Instructions

This document provides instructions for building and testing the Legendary Races Framework.

## 📋 Prerequisites

1. **Development Tools**:
   - Visual Studio 2022 or Visual Studio Code with C# extensions
   - .NET SDK 4.7.2+
   - Git for version control

2. **RimWorld Installed**:
   - A legal copy of RimWorld (current version 1.5)
   - Preferably a dedicated "development" installation for testing

3. **Required Mods**:
   - Harmony (installed in your Mods folder)
   - Vanilla Expanded Framework (VEF) (installed in your Mods folder)

## 🛠️ Building the Framework

### Using Visual Studio 2022

1. Open the solution file: `Source/LegendaryRacesFramework/LegendaryRacesFramework.sln`
2. Update library references if needed in the `.csproj` file:
   - Make sure paths to RimWorld assemblies are correct
   - Check the Harmony DLL path is correct
3. Build the solution (F6 or Build > Build Solution)
4. The compiled DLL should appear in the `Assemblies` folder

### Using Visual Studio Code

1. Open the project folder in VS Code
2. Make sure the C# extension is installed
3. Update references in `.csproj` file if needed
4. Run the build task (Ctrl+Shift+B or Terminal > Run Build Task)
5. Check that the DLL appears in the `Assemblies` folder

### Using Command Line

```bash
# Navigate to project directory
cd Source/LegendaryRacesFramework

# Build the project
dotnet build

# Copy the output DLL to Assemblies folder
cp bin/Debug/net472/LRF.dll ../../Assemblies/
```

## 📦 Installing for Testing

### Method 1: Symlink (Recommended for Development)

Create a symbolic link from your repository to the RimWorld Mods folder:

**Windows**:
```cmd
mklink /D "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\LegendaryRacesFramework" "C:\path\to\your\repository"
```

**macOS/Linux**:
```bash
ln -s /path/to/your/repository "/Applications/RimWorld/RimWorld.app/Mods/LegendaryRacesFramework"
```

### Method 2: Direct Copy

1. Copy the entire repository folder to your RimWorld Mods folder
2. Make sure you rebuild and recopy after any changes

## ⚠️ Common Build Issues

### Missing References
If you get missing reference errors, check:
- RimWorld installation path is correct in `.csproj`
- Required mods are installed (Harmony, VEF)
- File paths use the correct format for your OS

### Wrong .NET Version
The project targets .NET 4.7.2. If you have build issues, check:
- Your development environment has .NET 4.7.2 installed
- The project file has the correct target framework

### Assembly Loading Issues
If the mod doesn't load in RimWorld:
- Check output DLL is in the `Assemblies` folder
- Verify dependencies are loading in the correct order
- Look in the RimWorld logs (PlayerLog.txt) for error details

## 🧪 Testing in RimWorld

1. Start RimWorld
2. Make sure the mod shows up in the mod list and is enabled
3. Create a new game to test basic loading functionality
4. Use dev mode (Options > Development mode) for additional testing tools
5. Check logs for any errors or warnings

## 🐛 Debugging

### Visual Studio Debugging
1. Add a new Debug profile targeting RimWorld.exe
2. Set working directory to RimWorld installation
3. Start debugging (F5)

### Log-Based Debugging
1. Add `Log.Message()` calls for important checkpoints
2. Check the RimWorld log file for output
3. Consider using a mod like Dev Mode Extended for better logging

---

If you encounter persistent issues, please create an issue in the repository with details about the error and your environment.