// System
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Modules
using Kotsh.Modules.Model;

namespace Kotsh.Modules.Instance
{
    /// <summary>
    /// Tasker is the main function used to run checks
    /// </summary>
    public class Tasker
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Progression of proxies
        /// </summary>
        private int proxy_i = 0;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Tasker(Manager core)
        {
            // Store the core
            this.core = core;
        }

        /// <summary>
        /// Get a proxy and increment progression
        /// </summary>
        /// <returns>Proxy as host:port</returns>
        public string GetProxy()
        {
            // Check proxies
            if (this.proxy_i >= core.Proxies.Count)
            {
                this.proxy_i = 0;
            }

            // Get proxy
            string proxy = core.Proxies.Keys[this.proxy_i] + ":" + core.Proxies[this.proxy_i];

            // Increment proxy
            this.proxy_i++;

            // Return proxy
            return proxy;
        }

        /// <summary>
        /// Check every combo using multi-threading
        /// </summary>
        /// <param name="function">Checking function</param>
        public void Run(Func<string, Response> function)
        {
            // Open file stream
            var stream = File.ReadLines(core.runSettings["combolist"]);

            // Get threads count
            int threads = int.Parse(core.runStats.Get("threads"));

            // Store line 
            core.runStats["count"] = stream.Count().ToString();

            // Set on started
            core.status = 1;

            // Start progression bar
            core.Console.StartRun();

            // Start CPM calculator thread
            new Thread(() =>
            {
                // While checking
                while (core.status == 1)
                {
                    // Get initial checks
                    int initial_check = int.Parse(core.runStats["checked"]);

                    // Wait 2 seconds
                    Thread.Sleep(2000);

                    // Get actual checks
                    int actual_check = int.Parse(core.runStats["checked"]);

                    // Calculate it
                    int cpm = (actual_check - initial_check) * 30;

                    // Assign CPM
                    core.runStats["cpm"] = cpm.ToString();
                }
            }).Start();

            // Assign threads
            Parallel.ForEach(
                // File stream
                stream,
                // Parallel Options
                new ParallelOptions
                {
                    // Max threads 
                    MaxDegreeOfParallelism = threads

                    // Combo => Line
                    // Controller => Parallel control variable
                    // Count => Number of lines
                }, (combo, controller, count) =>
                {
                    // Execute combo
                    Response res = function.Invoke(combo);

                    // Handle banned or retry
                    while (res.type == Model.Type.BANNED || res.type == Model.Type.RETRY)
                    {   
                        // Relaunch check
                        res = function.Invoke(combo);
                    }

                    // Call response handler
                    core.Handler.Check(res);

                    // Update title
                    core.Program.UpdateTitle();

                    // Update stats
                    core.Console.UpdateRunningConsole();
                }
            );

            // Set on finished
            core.status = 2;

            // Update title
            core.Program.UpdateTitle();
        }
    }
}
