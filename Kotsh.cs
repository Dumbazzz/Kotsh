using Kotsh.Filter;
using Kotsh.Instance;
using Kotsh.IO;
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
        public string version = "0.3.1";
        public string releaseMode = "RC";

        /// <summary>
        /// Kotsh instances
        /// </summary>
        public ProgramManager Program;
        public Console Console;
        public Input Input;
        public FileHelper FileHelper;
        public Tasker Tasker;
        public Handler Handler;
        public ProxyController ProxyController;

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
        public NameValueCollection runSettings = new NameValueCollection();

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
        public Manager(System.Action<Manager, ProgramManager, Console, Input, FileHelper, Tasker, Handler> execution)
        {
            // Initialize modules
            this.Program = new ProgramManager(this);
            this.Console = new Console(this);
            this.Input = new Input(this);
            this.FileHelper = new FileHelper(this);
            this.Tasker = new Tasker(this);
            this.Handler = new Handler(this);

            // Not in method invocation
            this.RunStatistics = new RunStatistics(this);
            this.ProgramStatistics = new ProgramStatistics(this);
            this.ProxyController = new ProxyController(this);

            // Execution
            execution.Invoke(this, this.Program, this.Console, this.Input, this.FileHelper, this.Tasker, this.Handler);
        }
    }
}
