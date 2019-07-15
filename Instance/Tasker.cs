using Kotsh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
        private readonly Manager core;

        /// <summary>
        /// Regex for credentials (user:pass and email:pass)
        /// </summary>
        private readonly Regex CredentialsRegex = new Regex("^.*:.*$");

        /// <summary>
        /// List of rules
        /// </summary>
        private readonly List<Objects.Requirements> RequirementsList = new List<Objects.Requirements>();

        /// <summary>
        /// Requirement controller
        /// </summary>
        public RequirementsController RequirementsController = new RequirementsController();

        /// <summary>
        /// Stats updater task
        /// </summary>
        private Thread StatsUpdater;

        /// <summary>
        /// Runner thread
        /// </summary>
        private Thread Runner;

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
        /// Add requirements to the list
        /// </summary>
        /// <param name="requirements">Requirement Object</param>
        public void AddRequirements(Objects.Requirements requirements)
            => RequirementsList.Add(requirements);

        /// <summary>
        /// Check every combo using multi-threading
        /// </summary>
        /// <param name="function">Checking function</param>
        public void RunCombo(Func<string, Response, Response> function)
        {
            // Set on started
            core.status = 1;

            // Open file stream
            var stream = File.ReadLines(core.runSettings["combolist"]);

            // Store line 
            core.ProgramStatistics.SetCount(stream.Count());

            // Log CPM
            StartBackgroundStatsUpdater();

            // Start runner
            Runner = new Thread(() =>
            {
                // Start running line by line
                stream
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
                .WithDegreeOfParallelism(GetThreads())
                .ForAll((combo) => 
                {
                    // Check combo using regex
                    if (CredentialsRegex.IsMatch(combo))
                    {
                        // Check requirements
                        bool valid = true;
                        if (RequirementsList.Count != 0)
                        {
                            foreach (Objects.Requirements req in RequirementsList)
                            {
                                valid = RequirementsController.CheckRequirements(combo, req);
                            }
                        }

                        // Combo is valid, continue
                        if (valid)
                        {
                            // Execute combo
                            Response res = function.Invoke(combo, new Response(combo));

                            // Handle banned or retry
                            while (res.type == Models.Type.BANNED || res.type == Models.Type.RETRY)
                            {
                                // Relaunch check
                                res = function.Invoke(combo, new Response(combo));

                                // Call response handler
                                core.Handler.Check(res);
                            }

                            // Call response handler
                            core.Handler.Check(res);
                        }
                    }
                });
            })
            {

                // Set thread name
                Name = core.Program.name,
            };

            // Start thread
            Runner.Start();

            // Wait until execution is finished
            Runner.Join();

            // Set on finished
            core.status = 2;

            // Update title to pass as finished
            core.Program.UpdateTitle();

            // Abort stats updater
            StatsUpdater.Abort();
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
            // Set on started
            core.status = 1;

            // Store line 
            // TODO: Actually set on max int value
            core.ProgramStatistics.SetCount(int.MaxValue);

            // Log CPM
            StartBackgroundStatsUpdater();

            // Start runner
            Runner = new Thread(() =>
            {
                // Start running line by line
                Infinite()
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.AutoBuffered)
                .WithDegreeOfParallelism(GetThreads())
                .ForAll((combo) =>
                {
                    // Execute combo
                    Response res = function.Invoke(new Response());

                    // Handle banned or retry
                    while (res.type == Models.Type.BANNED || res.type == Models.Type.RETRY)
                    {
                        // Relaunch check
                        res = function.Invoke(new Response());

                        // Call response handler
                        core.Handler.Check(res);
                    }

                    // Call response handler
                    core.Handler.Check(res);
                });
            })
            {
                // Set thread name
                Name = core.Program.name,
            };

            // Start thread
            Runner.Start();

            // Wait until execution is finished
            Runner.Join();

            // Set on finished
            core.status = 2;

            // Update title to pass as finished
            core.Program.UpdateTitle();

            // Abort stats updater
            StatsUpdater.Abort();
        }

        /// <summary>
        /// Starts a thread to update statistics in background
        /// </summary>
        private void StartBackgroundStatsUpdater()
        {
            // Start stats updater
            StatsUpdater = new Thread(() =>
            {
                // While checking
                while (core.status == 1)
                {
                    // Update stats
                    core.ProgramStatistics.UpdateStats();
                }
            });

            // Set thread in background
            StatsUpdater.IsBackground = true;

            // Start stats updater
            StatsUpdater.Start();
        }
    }
}
