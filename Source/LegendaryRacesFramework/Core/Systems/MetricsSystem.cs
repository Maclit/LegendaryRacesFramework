using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace LegendaryRacesFramework
{
    // Balance metrics system
    public class DefaultBalanceMetrics : IBalanceMetrics
    {
        private readonly string raceID;
        private readonly BalanceSettings settings;
        private readonly Dictionary<string, List<float>> metricHistory = new Dictionary<string, List<float>>();
        private readonly int maxHistoryPoints = 100;
        
        public string RaceID => raceID;
        
        public float CurrentPowerLevel => settings?.powerLevel ?? 3f;
        
        public float ResourceBalanceFactor => settings?.resourceMultiplier ?? 1f;
        
        public DefaultBalanceMetrics(string raceID, BalanceSettings settings)
        {
            this.raceID = raceID;
            this.settings = settings;
            
            // Initialize with common metric types
            metricHistory["DamageDealt"] = new List<float>();
            metricHistory["DamageTaken"] = new List<float>();
            metricHistory["ResourceConsumption"] = new List<float>();
            metricHistory["AbilityUses"] = new List<float>();
            metricHistory["SurvivalTime"] = new List<float>();
        }
        
        public void RecordMetric(string metricType, float value)
        {
            if (string.IsNullOrEmpty(metricType))
                return;
            
            // Initialize metric history if needed
            if (!metricHistory.TryGetValue(metricType, out List<float> history))
            {
                history = new List<float>();
                metricHistory[metricType] = history;
            }
            
            // Add value to history
            history.Add(value);
            
            // Trim history if too long
            if (history.Count > maxHistoryPoints)
            {
                history.RemoveAt(0);
            }
        }
        
        public float GetAverageMetric(string metricType)
        {
            if (string.IsNullOrEmpty(metricType) || !metricHistory.TryGetValue(metricType, out List<float> history) || history.Count == 0)
                return 0f;
            
            return history.Average();
        }
        
        public float ComputeBalanceScore()
        {
            // Placeholder for a more comprehensive balance score calculation
            // In a full implementation, this would compare metrics to baseline values
            
            // For now, just return a normalized value based on power level
            return CurrentPowerLevel / 5f;
        }
        
        public Dictionary<string, List<float>> GetAllMetrics()
        {
            return metricHistory;
        }
    }
    
    // Performance monitoring system
    public class DefaultPerformanceMonitor : IPerformanceMonitor
    {
        private readonly string raceID;
        private readonly PerformanceSettings settings;
        private readonly Dictionary<string, List<float>> executionTimes = new Dictionary<string, List<float>>();
        private readonly Dictionary<string, Stopwatch> activeTimers = new Dictionary<string, Stopwatch>();
        private readonly int maxTimingPoints = 100;
        private readonly float acceptableThresholdMs = 0.5f; // 0.5ms threshold for operations
        
        public string RaceID => raceID;
        
        public DefaultPerformanceMonitor(string raceID, PerformanceSettings settings)
        {
            this.raceID = raceID;
            this.settings = settings;
        }
        
        public void StartTiming(string operationName)
        {
            if (string.IsNullOrEmpty(operationName) || !settings.usePerformanceMonitoring)
                return;
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            activeTimers[operationName] = sw;
        }
        
        public void StopTiming(string operationName)
        {
            if (string.IsNullOrEmpty(operationName) || !settings.usePerformanceMonitoring)
                return;
            
            if (activeTimers.TryGetValue(operationName, out Stopwatch sw))
            {
                sw.Stop();
                float elapsedMs = (float)sw.Elapsed.TotalMilliseconds;
                
                // Record execution time
                if (!executionTimes.TryGetValue(operationName, out List<float> times))
                {
                    times = new List<float>();
                    executionTimes[operationName] = times;
                }
                
                times.Add(elapsedMs);
                
                // Trim history if too long
                if (times.Count > maxTimingPoints)
                {
                    times.RemoveAt(0);
                }
                
                // Clean up active timer
                activeTimers.Remove(operationName);
                
                // Log warning if operation took too long
                if (elapsedMs > acceptableThresholdMs * 10)
                {
                    Log.Warning($"Performance warning: Operation '{operationName}' for race '{raceID}' took {elapsedMs}ms");
                }
            }
        }
        
        public float GetAverageExecutionTime(string operationName)
        {
            if (string.IsNullOrEmpty(operationName) || !executionTimes.TryGetValue(operationName, out List<float> times) || times.Count == 0)
                return 0f;
            
            return times.Average();
        }
        
        public Dictionary<string, float> GetAllPerformanceMetrics()
        {
            Dictionary<string, float> metrics = new Dictionary<string, float>();
            
            foreach (var kvp in executionTimes)
            {
                if (kvp.Value.Count > 0)
                {
                    metrics[kvp.Key] = kvp.Value.Average();
                }
                else
                {
                    metrics[kvp.Key] = 0f;
                }
            }
            
            return metrics;
        }
        
        public bool IsPerformanceAcceptable()
        {
            // Check if any operation exceeds acceptable threshold on average
            foreach (var kvp in executionTimes)
            {
                if (kvp.Value.Count > 0 && kvp.Value.Average() > acceptableThresholdMs)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public List<string> SuggestOptimizations()
        {
            List<string> suggestions = new List<string>();
            
            // Find most expensive operations
            var sortedOperations = executionTimes
                .Where(kvp => kvp.Value.Count > 0)
                .OrderByDescending(kvp => kvp.Value.Average())
                .Take(3)
                .ToList();
            
            // Generate suggestions for expensive operations
            foreach (var operation in sortedOperations)
            {
                float avgTime = operation.Value.Average();
                if (avgTime > acceptableThresholdMs)
                {
                    suggestions.Add($"Optimize '{operation.Key}' operation (avg {avgTime:F2}ms)");
                    
                    // Suggest specific optimizations based on operation name
                    if (operation.Key.Contains("Resource") && settings.resourceUpdateInterval < 250)
                    {
                        suggestions.Add($"Increase resourceUpdateInterval from {settings.resourceUpdateInterval} to {settings.resourceUpdateInterval * 2}");
                    }
                    else if (operation.Key.Contains("Ability") && settings.abilityCheckInterval < 60)
                    {
                        suggestions.Add($"Increase abilityCheckInterval from {settings.abilityCheckInterval} to {settings.abilityCheckInterval * 2}");
                    }
                    else if (operation.Key.Contains("Environmental") && settings.environmentalCheckInterval < 250)
                    {
                        suggestions.Add($"Increase environmentalCheckInterval from {settings.environmentalCheckInterval} to {settings.environmentalCheckInterval * 2}");
                    }
                }
            }
            
            // General suggestions
            if (!IsPerformanceAcceptable())
            {
                suggestions.Add("Consider disabling performance-intensive features for this race");
                suggestions.Add("Check for redundant calculations in race mechanics");
            }
            
            return suggestions;
        }
    }
}