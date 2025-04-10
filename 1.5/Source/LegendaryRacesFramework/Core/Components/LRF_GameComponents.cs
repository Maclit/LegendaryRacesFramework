using System;
using System.Collections.Generic;
using Verse;

namespace LegendaryRacesFramework
{
    /// <summary>
    /// Game component for centralized tick handling
    /// </summary>
    public class LRF_GameComponent : GameComponent
    {
        private static readonly List<Action> tickActions = new List<Action>();
        private static readonly List<Action> slowTickActions = new List<Action>();
        private static LRF_GameComponent instance;
        
        // RimWorld 1.5 tracking variables
        private int slowTickCounter = 0;
        private const int SLOW_TICK_INTERVAL = 60; // Every 60 ticks (1 second)
        
        public LRF_GameComponent(Game game) : base()
        {
            instance = this;
        }
        
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            
            // Execute all registered tick actions
            foreach (Action action in tickActions)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error in LRF tick action: {ex}");
                }
            }
            
            // Handle slow ticks for less frequent updates
            slowTickCounter++;
            if (slowTickCounter >= SLOW_TICK_INTERVAL)
            {
                slowTickCounter = 0;
                
                foreach (Action action in slowTickActions)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error in LRF slow tick action: {ex}");
                    }
                }
            }
        }
        
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            
            // Initialize legendary characters
            LegendaryCharacterManager.Initialize();
            
            // Initialize all race handlers
            foreach (LegendaryRaceDef raceDef in DefDatabase<LegendaryRaceDef>.AllDefs)
            {
                if (!raceDef.Abstract)
                {
                    try
                    {
                        IRaceExtension handler = raceDef.GetRaceHandler();
                        if (handler != null)
                        {
                            handler.PostLoadInit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error initializing race handler for {raceDef.defName}: {ex}");
                    }
                }
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            // Save any persistent data here
        }
        
        /// <summary>
        /// Register an action to be called every tick
        /// </summary>
        public static void RegisterForTick(Action action)
        {
            if (action != null && !tickActions.Contains(action))
            {
                tickActions.Add(action);
            }
        }
        
        /// <summary>
        /// Register an action to be called less frequently (every SLOW_TICK_INTERVAL ticks)
        /// </summary>
        public static void RegisterForSlowTick(Action action)
        {
            if (action != null && !slowTickActions.Contains(action))
            {
                slowTickActions.Add(action);
            }
        }
        
        /// <summary>
        /// Unregister a previously registered tick action
        /// </summary>
        public static void UnregisterForTick(Action action)
        {
            if (action != null)
            {
                tickActions.Remove(action);
            }
        }
        
        /// <summary>
        /// Unregister a previously registered slow tick action
        /// </summary>
        public static void UnregisterForSlowTick(Action action)
        {
            if (action != null)
            {
                slowTickActions.Remove(action);
            }
        }
        
        /// <summary>
        /// Get the singleton instance of this component
        /// </summary>
        public static LRF_GameComponent Instance => instance;
        
        /// <summary>
        /// Check if the framework is ready (instance exists)
        /// </summary>
        public static bool IsReady => instance != null;
    }
}