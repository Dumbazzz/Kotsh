using System.Collections.Generic;
using System.Threading;

namespace Kotsh.Statistics
{
    public class ProgramStatistics
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Running statistics
        /// </summary>
        protected Dictionary<string, int> Stats = new Dictionary<string, int>();

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public ProgramStatistics(Manager core)
        {
            // Store the core
            this.core = core;

            // Set default values for default stats
            Stats.Add("count", 0); // Number of lines to check
            Stats.Add("checked", 0); // Checked lines
            Stats.Add("remaining", 0); // Remaining lines
            Stats.Add("tries", 0); // Every tries, include bans/retries
            Stats.Add("cpm", 0); // Checked per minute
            Stats.Add("rpm", 0); // Tries per minute
            Stats.Add("percent", 0); // Progression percentage
        }

        /// <summary>
        /// Sets a value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Set(string key, int value)
            => Stats[key] = value;

        /// <summary>
        /// Set count and remaining lines
        /// </summary>
        /// <param name="lines">Lines to check</param>
        public void SetCount(int lines)
        {
            // Set count and remaining lines
            Set("count", lines);
            Set("remaining", lines);
        }

        /// <summary>
        /// Increment value
        /// </summary>
        /// <param name="key">Key</param>
        public void Increment(string key)
            => Stats[key] = Stats[key] + 1;

        /// <summary>
        /// Increment value
        /// </summary>
        /// <param name="key">Key</param>
        public void Decrement(string key)
            => Stats[key] = Stats[key] - 1;

        /// <summary>
        /// Increment checked value, decrement remaining lines
        /// </summary>
        /// <param name="is_try">If true, it will increment tries too</param>
        public void IncrementCheck(bool is_try = false)
        {
            // Increment checked
            Increment("checked");

            // Increment tries
            if (is_try)
                Increment("tries");

            // Downgrade demaining
            Decrement("remaining");
        }

        /// <summary>
        /// Get value according to the key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        public int Get(string key)
        {
            return Stats[key];
        }

        /// <summary>
        /// Update CPM, RPM and progression percentage
        /// </summary>
        public void UpdateStats()
        {
            // Update percentage
            GetPercentage();

            // Update CPM
            GetCPM();

            // Update RPM
            GetRPM();
        }

        private int GetPercentage()
        {
            // Get total lines
            int lines = Get("count");

            // Get checked lines
            int check = Get("checked");

            // Calculate percentage
            int percent = (check * 100) / lines;

            // Save percentage
            Set("percent", percent);

            // Return percentage
            return percent;
        }

        /// <summary>
        /// Calculate CPM
        /// </summary>
        /// <returns>CPM</returns>
        private int GetCPM()
        {
            // Get initial checks
            int start = Get("checked");

            // Wait 3 seconds
            Thread.Sleep(3000);

            // Get actual checks
            int end = Get("checked");

            // Calculate CPM
            int cpm = (end - start) * 20;

            // Save value
            Set("cpm", cpm);

            // Return value
            return cpm;
        }

        /// <summary>
        /// Calculate RPM
        /// </summary>
        /// <returns>RPM</returns>
        private int GetRPM()
        {
            // Get initial checks
            int start = Get("tries");

            // Wait 3 seconds
            Thread.Sleep(3000);

            // Get actual checks
            int end = Get("tries");

            // Calculate RPM
            int rpm = (end - start) * 20;

            // Save value
            Set("rpm", rpm);

            // Return RPM
            return rpm;
        }
    }
}
