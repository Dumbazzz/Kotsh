using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Kotsh.Modules.IO
{
    public class FileHelper
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Execution directory
        /// </summary>
        private readonly string directory = Directory.GetCurrentDirectory();

        /// <summary>
        /// Default and last opened directory
        /// </summary>
        private string last_directory = Directory.GetCurrentDirectory();

        /// <summary>
        /// This variable contains last chosen file
        /// </summary>
        public string filepath;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public FileHelper(Manager core)
        {
            // Store the core
            this.core = core;
        }

        public void ChooseFile(string title, string fileType)
        {
            // Open file chooser instance
            OpenFileDialog ofd = new OpenFileDialog
            {
                // Setup file chooser
                InitialDirectory = last_directory,
                Filter = fileType,
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = title
            };

            // Handle file
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Get selected file and save it
                this.filepath = ofd.FileName;

                // Save directory used
                this.last_directory = Path.GetDirectoryName(this.filepath);
            }
            else
            {
                // Ask to continue
                string response = core.Input.AskChoice("You canceled file chooser, continue ?", new string[] { "Yes", "No" });

                // Check response
                if (response == "Yes")
                {
                    // Continue -> Recursive call
                    this.ChooseFile(title, fileType);
                }
                else
                {
                    // Exit application
                    System.Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Will execute a lambda function for each line of the file
        /// </summary>
        /// <param name="function">Lambda function</param>
        public void Execute(Func<string, int> function)
        {
            // Read file line by line
            var lines = File.ReadLines(this.filepath);
            foreach (string line in lines)
            {
                // Execute lambda function
                function.Invoke(line);
            }
        }

        /// <summary>
        /// Save a file in the session folder
        /// </summary>
        /// <param name="name">File name with extension</param>
        /// <param name="data">Data to push in the file</param>
        public void SaveSingleFile(string name, string data)
        {
            // Create file path
            string path = directory + "\\results\\" + core.runSettings["session_folder"] + "\\" + name + ".txt";

            // Write text file
            using (var sw = new StreamWriter(path, true))
            {
                // Write into the file
                sw.Write(data);

                // Close the writer
                sw.Close();
            }
        }

        /// <summary>
        /// Download a file from URL
        /// </summary>
        /// <param name="URL">Target URL</param>
        /// <param name="target">Target file with extension</param>
        public void DownloadFile(string URL, string target)
        {
            // Create file path
            string path = directory + "\\results\\" + core.runSettings["session_folder"] + "\\" + target;

            // Open web client
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(URL, path);
            }
        }
    }
}
