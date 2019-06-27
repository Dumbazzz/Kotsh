// System
using System.Collections.Specialized;

// Modules
using Kotsh.Modules.Program;
using Kotsh.Modules.Console;
using Kotsh.Modules.IO;
using Kotsh.Modules.Instance;
using Kotsh.Modules.Filter;
using Kotsh.Modules.Block;

// Kotsh namespace
namespace Kotsh
{
    // Instance manager
    public class Manager
    {
        /// <summary>
        /// Kotsh details
        /// </summary>
        public string version = "0.1";
        public string releaseMode = "DEV";

        /// <summary>
        /// Kotsh instances
        /// </summary>
        public Program Program;
        public Console Console;
        public Input Input;
        public FileHelper FileHelper;
        public Tasker Tasker;
        public Handler Handler;
        public Block Block;

        /// <summary>
        /// Status of the checker
        /// 
        /// 0 = Idle
        /// 1 = Running
        /// 2 = Ended
        /// </summary>
        public int status = 0;

        /// <summary>
        /// Settings used for the run
        /// </summary>
        public NameValueCollection runSettings = new NameValueCollection()
        {
            { "ProxyProtocol", "HTTP" }
        };

        /// <summary>
        /// Stats during the run
        /// </summary>
        public NameValueCollection runStats = new NameValueCollection()
        {
            // Default values
            { "cpm"      , "0" },
            { "checked"  , "0" },
            { "remaining", "0" },
            { "hits"     , "0" },
            { "free"     , "0" },
            { "custom"   , "0" },
            { "expired"  , "0" },
            { "fail"     , "0" },
            { "banned"   , "0" },
            { "retry"    , "0" },
            { "ignored"  , "0" }
        };

        /// <summary>
        /// Array with proxies
        /// </summary>
        public NameValueCollection Proxies = new NameValueCollection();

        /// <summary>
        /// Supported protocols for proxies
        /// </summary>
        public string[] ProxiesProtocols = new[]
        {
            "HTTP",
            "SOCKS4",
            "SOCKS4A",
            "SOCKS5"
        };

        /// <summary>
        /// Default settings
        /// </summary>
        public NameValueCollection settings = new NameValueCollection()
        {
            // File saving
            { "AutoSave"        , "true" }, // Enable auto saving hits
            { "FileSave_hit"    , "true" }, // Save hits into a single file
            { "FileSave_custom" , "true" }, // Save customs into a single file
            { "FileSave_free"   , "true" }, // Save free into a single file
            { "FileSave_expired", "true" }, // Save expired hits into a single file

            // Console
            { "UseProgression"  , "false" } // Display progression using ProgressBar
        };

        /// <summary>
        /// Initialize modules
        /// </summary>
        public Manager()
        {
            // Initialize modules
            this.Program = new Program(this);
            this.Console = new Console(this);
            this.Input = new Input(this);
            this.FileHelper = new FileHelper(this);
            this.Tasker = new Tasker(this);
            this.Handler = new Handler(this);
            this.Block = new Block(this);
        }
    }
}
