using Kotsh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kotsh.Instance
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
        /// Regex for credentials (user:pass and email:pass)
        /// </summary>
        private readonly Regex CredentialsRegex = new Regex("^.*:.*$");


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
        /// Return threads in integer (if not set, it will set 1 thread)
        /// </summary>
        /// <returns>Thread Count</returns>
        private int GetThreads()
        {
            // Check if threads are set
            if (!Enumerable.Range(1, int.MaxValue).Contains(core.threads))
            {
                // Wrong thread number
                return 1; // Set on 1 thread
            }
            else
            {
                // Check lines
                int lines = core.ProgramStatistics.Get("count");

                // Check if thread are less than lines
                if (core.threads > lines)
                {
                    // Too much threads, set 1 thread
                    return 1;
                }
                else
                {
                    // Threads are set
                    return core.threads;
                }
            }
        }

        /// <summary>
        /// Check every combo using multi-threading
        /// </summary>
        /// <param name="function">Checking function</param>
        public void RunCombo(Func<string, Response, Response> function)
        {
            // Open file stream
            var stream = File.ReadLines(core.runSettings["combolist"]);

            // Store line 
            core.ProgramStatistics.SetCount(stream.Count());

            // Set on started
            core.status = 1;

            // Log CPM
            RegisterCPM();

            // Assign threads
            Parallel.ForEach(
                // File stream
                stream,
                // Parallel Options
                new ParallelOptions
                {
                    // Max threads 
                    MaxDegreeOfParallelism = GetThreads()

                    // Combo => Line
                    // Controller => Parallel control variable
                    // Count => Number of lines
                }, (combo, controller, count) =>
                {
                    // Check combo using regex
                    if (CredentialsRegex.IsMatch(combo))
                    {
                        // Execute combo
                        Response res = function.Invoke(combo, new Response(combo));

                        // Handle banned or retry
                        while (res.type == Models.Type.BANNED || res.type == Models.Type.RETRY)
                        {
                            // Relaunch check
                            res = function.Invoke(combo, new Response(combo));
                        }

                        // Call response handler
                        core.Handler.Check(res);
                    }
                }
            );

            // Set on finished
            core.status = 2;

            // Update title
            core.Program.UpdateTitle();
        }

        /// <summary>
        /// Support used for infinite loops
        /// </summary>
        /// <returns>Boolean</returns>
        private IEnumerable<bool> Infinite()
        {
            while (true)
            {
                yield return true;
            }
        }

        /// <summary>
        /// Execute function into a infinite loop
        /// </summary>
        /// <param name="function">Checking function</param>
        public void RunInfinite(Func<Response, Response> function)
        {
            // Store line 
            // TODO: Actually set on max int value
            core.ProgramStatistics.SetCount(int.MaxValue);

            // Set on started
            core.status = 1;

            // Log CPM
            RegisterCPM();

            // Assign threads
            Parallel.ForEach(
                // Infinite stream
                Infinite(),
                // Parallel Options
                new ParallelOptions
                {
                    // Max threads 
                    MaxDegreeOfParallelism = GetThreads()
                },
                // Arguments
                new Action<bool>((val) =>
                {
                    // Execute combo
                    Response res = function.Invoke(new Response());

                    // Handle banned or retry
                    while (res.type == Models.Type.BANNED || res.type == Models.Type.RETRY)
                    {
                        // Relaunch check
                        res = function.Invoke(new Response());
                    }

                    // Call response handler
                    core.Handler.Check(res);
                }
            ));

            // Set on finished
            core.status = 2;

            // Update title
            core.Program.UpdateTitle();
        }

        /// <summary>
        /// Starts a thread to log CPM
        /// </summary>
        private void RegisterCPM()
        {
            // Start CPM calculator thread
            Task.Run(() =>
            {
                // While checking
                while (core.status == 1)
                {
                    // Get CPM
                    core.ProgramStatistics.GetCPM();
                }
            });
        }
    }
}
