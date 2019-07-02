using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kotsh.Program
{
    public class ProgramManager
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Program definitions
        /// </summary>
        public string name, author, version;

        /// <summary>
        /// Console Window titles to use on differents steps
        /// </summary>
        public NameValueCollection titles = new NameValueCollection()
        {
            { "idleTitle", "%name% by %author% | v%version% | Idling" },
            { "runningTitle", "%name% | Hits: %hits% - CPM: %cpm% - Fail: %fail% - Bans: %banned% | Running" },
            { "endTitle", "%name% | Hits: %hits% - CPM: %cpm% - Fail: %fail% - Bans: %banned% | Finished" }
        };

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public ProgramManager(Manager core)
        {
            // Store the core
            this.core = core;
        }

        /// <summary>
        /// Generate a random ID (0.001% duplicate in 100M)
        /// </summary>
        /// <returns>random ID</returns>
        public string MakeId()
        {
            // Start builder
            StringBuilder builder = new StringBuilder();

            // Make random ID
            Enumerable
               .Range(65, 26)
                .Select(e => ((char) e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));

            // Return generated ID
            return builder.ToString();
        }

        /// <summary>
        /// Define program name, author and version
        /// </summary>
        /// <param name="name">Name of the Checker</param>
        /// <param name="author">Author of the checker</param>
        /// <param name="version">Version of the checker</param>
        public void Initialize(string name, string author, string version)
        {
            // Store definitions
            this.name = name;
            this.author = author;
            this.version = version;

            // Show title
            core.Console.DisplayTitle();

            // Make unique session identifier
            core.runSettings["session_id"] = this.MakeId();

            // Make unique session folder
            core.runSettings["session_folder"] = 
                DateTime.Now.ToString().Replace("/", "-").Replace(":", "-")
                + " - " 
                + core.runSettings["session_id"];

            // Update title
            this.UpdateTitle();

            // Check and make session folders
            core.Handler.MakeFolders();
        }

        /// <summary>
        /// Replace input with dynamic variables (e.g.: %name%) to fixed value
        /// </summary>
        /// <param name="input">Text with dynamic variables</param>
        /// <returns>Text with fixed value</returns>
        public string ReplaceVariables(string input)
        {
            return input
                // Program informations
                .Replace("%name%", this.name)
                .Replace("%author%", this.author)
                .Replace("%version%", this.version)

                // Stats
                .Replace("%cpm%", core.runStats.Get("cpm"))
                .Replace("%checked%", core.runStats.Get("checked"))
                .Replace("%remaining%", core.runStats.Get("remaining"))

                // Targets
                .Replace("%hits%", core.runStats.Get("hits"))
                .Replace("%free%", core.runStats.Get("free"))
                .Replace("%custom%", core.runStats.Get("custom"))
                .Replace("%expired%", core.runStats.Get("expired"))
                .Replace("%fail%", core.runStats.Get("fail"))
                .Replace("%banned%", core.runStats.Get("banned"))
                .Replace("%retry%", core.runStats.Get("retry"));
        }

        /// <summary>
        /// Update window title
        /// </summary>
        public void UpdateTitle()
        {
            // Select title according to the situation
            string title = titles[core.status];

            // Display title
            System.Console.Title = ReplaceVariables(title);
        }
    }
}
