// System
using Kotsh.Blocks;
using Kotsh.Filter;
using Kotsh.Instance;
using Kotsh.IO;
// Modules
using Kotsh.Program;
using Kotsh.Statistics;
using Kotsh.Window;
using System.Collections.Specialized;

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
        public ProgramManager Program;
        public Console Console;
        public Input Input;
        public FileHelper FileHelper;
        public Tasker Tasker;
        public Handler Handler;
        public Block Block;

        /// <summary>
        /// Non-user accessibles instances
        /// </summary>
        public RunStatistics RunStatistics;
        public ProgramStatistics ProgramStatistics;

        /// <summary>
        /// Status of the checker
        /// 
        /// 0 = Idle
        /// 1 = Running
        /// 2 = Ended
        /// </summary>
        public int status = 0;

        /// <summary>
        /// Thread count
        /// </summary>
        public int threads = 1;

        /// <summary>
        /// Settings used for the run
        /// </summary>
        public NameValueCollection runSettings = new NameValueCollection()
        {
            { "ProxyProtocol", "HTTP" }
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
            { "FileSave_expired", "true" }  // Save expired hits into a single file
        };

        /// <summary>
        /// Initialize modules
        /// </summary>
        public Manager(System.Action<Manager, ProgramManager, Console, Input, FileHelper, Tasker, Handler, Block> execution)
        {
            // Initialize modules
            this.Program = new ProgramManager(this);
            this.Console = new Console(this);
            this.Input = new Input(this);
            this.FileHelper = new FileHelper(this);
            this.Tasker = new Tasker(this);
            this.Handler = new Handler(this);
            this.Block = new Block(this);
            this.RunStatistics = new RunStatistics(this);
            this.ProgramStatistics = new ProgramStatistics(this);

            // Execution
            execution.Invoke(this, this.Program, this.Console, this.Input, this.FileHelper, this.Tasker, this.Handler, this.Block);
        }
    }
}
