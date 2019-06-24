using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kotsh.Modules.Program
{
    public class Program
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
        public Program(Manager core)
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

        public void UpdateTitle()
        {
            // Select title according to the situation
            string title = titles[core.status];

            // Replace program definitions
            title = title.Replace("%name%", this.name);
            title = title.Replace("%author%", this.author);
            title = title.Replace("%version%", this.version);

            // Replace program statistics
            title = title.Replace("%cpm%", core.runStats.Get("cpm"));
            title = title.Replace("%checked%", core.runStats.Get("checked"));
            title = title.Replace("%remaining%", core.runStats.Get("remaining"));
            title = title.Replace("%hits%", core.runStats.Get("hits"));
            title = title.Replace("%free%", core.runStats.Get("free"));
            title = title.Replace("%custom%", core.runStats.Get("custom"));
            title = title.Replace("%expired%", core.runStats.Get("expired"));
            title = title.Replace("%fail%", core.runStats.Get("fail"));
            title = title.Replace("%banned%", core.runStats.Get("banned"));
            title = title.Replace("%retry%", core.runStats.Get("retry"));

            // Display title
            System.Console.Title = title;
        }
    }
}
