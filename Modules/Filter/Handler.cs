using Kotsh.Modules.Model;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kotsh.Modules.Filter
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
            try {
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
            this.IncrementStats(response.type);

            // Get capture
            var items = response.capture.AllKeys.SelectMany(response.capture.GetValues, (k, v) => new { key = k, value = v });

            // Render capture
            StringBuilder capture = new StringBuilder();
            foreach (var item in items)
            {
                // Append
                capture.Append(item.key.ToUpper() + "=" + item.value);
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

            // Update title
            core.Program.UpdateTitle();

            // Display stats
            switch (response.type)
            {
                case Type.HIT:
                    core.Console.Push(Console.Level.SUCCESS, "HITS | " + line);
                    break;
                case Type.FREE:
                    core.Console.Push(Console.Level.INFO, "FREE | " + line);
                    break;
                case Type.FAIL:
                    core.Console.Push(Console.Level.WARNING, "FAIL | " +  line);
                    break;
                case Type.CUSTOM:
                    core.Console.Push(Console.Level.INFO, "CUSTOM | " + line);
                    break;
                case Type.EXPIRED:
                    core.Console.Push(Console.Level.INFO, "EXPIRED | " + line);
                    break;
            }
        }

        private void IncrementStats(Type type)
        {
            // Check count
            if (type != (Type.BANNED | Type.RETRY))
            {
                // Increment checked
                core.runStats["checked"] = (int.Parse(core.runStats["checked"]) + 1).ToString();
            }

            // Type specific
            switch (type)
            {
                case Type.HIT:
                    core.runStats["hits"] = (int.Parse(core.runStats["hits"]) + 1).ToString();
                    break;
                case Type.FREE:
                    core.runStats["free"] = (int.Parse(core.runStats["free"]) + 1).ToString();
                    break;
                case Type.FAIL:
                    core.runStats["fail"] = (int.Parse(core.runStats["fail"]) + 1).ToString();
                    break;
                case Type.CUSTOM:
                    core.runStats["custom"] = (int.Parse(core.runStats["custom"]) + 1).ToString();
                    break;
                case Type.EXPIRED:
                    core.runStats["expired"] = (int.Parse(core.runStats["expired"]) + 1).ToString();
                    break;
                case Type.BANNED:
                    core.runStats["banned"] = (int.Parse(core.runStats["banned"]) + 1).ToString();
                    break;
                case Type.RETRY:
                    core.runStats["retry"] = (int.Parse(core.runStats["retry"]) + 1).ToString();
                    break;
            }
        }
    }
}
