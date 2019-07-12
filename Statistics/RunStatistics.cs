using Kotsh.Models;
using System.Collections.Generic;

namespace Kotsh.Statistics
{
    public class RunStatistics
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private readonly Manager core;

        /// <summary>
        /// Type-Specific statistics
        /// </summary>
        private readonly Dictionary<Type, int> TypeStats = new Dictionary<Type, int>();

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public RunStatistics(Manager core)
        {
            // Store the core
            this.core = core;

            // Set default values for type-specific
            TypeStats.Add(Type.HIT, 0);
            TypeStats.Add(Type.FREE, 0);
            TypeStats.Add(Type.CUSTOM, 0);
            TypeStats.Add(Type.EXPIRED, 0);
            TypeStats.Add(Type.FAIL, 0);
            TypeStats.Add(Type.RETRY, 0);
            TypeStats.Add(Type.BANNED, 0);
        }

        /// <summary>
        /// Sets a value
        /// </summary>
        /// <param name="type">Key</param>
        /// <param name="value">Value</param>
        public void Set(Type type, int value)
            => TypeStats[type] = value;

        /// <summary>
        /// Increment value
        /// </summary>
        /// <param name="type">Key</param>
        public void Increment(Type type)
        {
            // Increment type
            TypeStats[type] = TypeStats[type] + 1;

            // Increment tries
            core.ProgramStatistics.Increment("tries");

            // Increment checks
            if (type != (Type.BANNED | Type.RETRY))
            {
                core.ProgramStatistics.Increment("checked");
            }
        }

        public int Get(Type type)
        {
            return TypeStats[type];
        }
    }
}
