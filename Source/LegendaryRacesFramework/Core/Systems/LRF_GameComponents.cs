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
        private static LRF_GameComponent instance;
        
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
        }
        
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            
            // Initialize legendary characters
            LegendaryCharacterManager.Initialize();
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
        /// Unregister a previously registered tick action
        /// </summary>
        public static void UnregisterForTick(Action action)
        {
            if (action != null)
            {
                tickActions.Remove(action);
            }
        }
    }
}