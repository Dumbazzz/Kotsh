using Kotsh.Models;
using Kotsh.Window;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kotsh.Filter
{
    public class Handler
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Handler(Manager core)
        {
            // Store the core
            this.core = core;
        }

        /// <summary>
        /// Current directory
        /// </summary>
        private readonly string dir = Directory.GetCurrentDirectory();

        /// <summary>
        /// Write on Lock security (safe threading)
        /// </summary>
        private ReaderWriterLockSlim fileLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Check and makes folder to store results
        /// </summary>
        public void MakeFolders()
        {
            // Check result direct
            if (!Directory.Exists(dir + "\\results"))
            {
                // Make folder
                Directory.CreateDirectory(dir + "\\results");
            }

            // Check for session folder
            if (!Directory.Exists(dir + "\\results\\" + core.runSettings["session_folder"]))
            {
                // Make folder
                Directory.CreateDirectory(dir + "\\results\\" + core.runSettings["session_folder"]);
            }
        }

        private void StoreResult(string file, string line)
        {
            // File path
            string path = dir + "\\results\\" + core.runSettings["session_folder"] + "\\" + file + ".txt";

            // Lock files
            fileLock.EnterWriteLock();
            try
            {
                // Append into file
                using (StreamWriter sw = File.AppendText(path))
                {
                    // Write into the file
                    sw.WriteLine(line);

                    // Close the writer
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                fileLock.ExitWriteLock();
            }
        }

        public void Check(Response response)
        {
            // Increment stats
            core.RunStatistics.Increment(response.type);

            // Update title
            core.Program.UpdateTitle();

            // Increment checks and tries
            switch (response.type)
            {
                // Increment checks
                case Type.HIT:
                case Type.FREE:
                case Type.CUSTOM:
                case Type.EXPIRED:
                    core.ProgramStatistics.IncrementCheck();
                    break;

                // Increment tries
                case Type.BANNED:
                case Type.RETRY:
                    core.ProgramStatistics.IncrementCheck(true);
                    break;
            }

            // Render capture
            StringBuilder capture = new StringBuilder();

            // Add capture
            if (response.capture.Count > 0)
            {
                // Get last value
                var last = response.capture.Last();

                // Loop
                foreach (var name in response.capture.Keys)
                {
                    // Append
                    capture.Append(name.ToUpper() + "=" + response.capture[name]);

                    // Add separator
                    if (!name.Equals(last.Key))
                    {
                        capture.Append(" | ");
                    }
                }
            }

            // Render file
            string line = response.combo;

            // Add capture
            if (capture.Length > 1)
            {
                line += " | " + capture;
            }

            // Check type
            switch (response.type)
            {
                case Type.CUSTOM:
                case Type.EXPIRED:
                case Type.FREE:
                case Type.HIT:
                    // Convert type
                    string type = response.type.ToString().ToLower();

                    // Check settings
                    if (core.settings.Get("FileSave_" + type) != null && core.settings.Get("FileSave_" + type) == "true")
                    {
                        this.StoreResult(type, line);
                    }
                    break;
            }

            // Display stats
            switch (response.type)
            {
                case Type.HIT:
                    core.Console.Push(Level.SUCCESS, "HITS | " + line);
                    break;
                case Type.FREE:
                    core.Console.Push(Level.INFO, "FREE | " + line);
                    break;
                case Type.FAIL:
                    core.Console.Push(Level.WARNING, "FAIL | " + line);
                    break;
                case Type.CUSTOM:
                    core.Console.Push(Level.INFO, "CUSTOM | " + line);
                    break;
                case Type.EXPIRED:
                    core.Console.Push(Level.INFO, "EXPIRED | " + line);
                    break;
            }
        }
    }
}
