using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Kotsh.IO
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

        /// <summary>
        /// Allow you to choose a file using Windows File Chooser
        /// </summary>
        /// <param name="title">Title of the Windows Frame</param>
        /// <param name="fileType">File type (example: "Text (.txt)|*.txt")</param>
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
                try
                {
                    webClient.DownloadFile(URL, path);
                }
                catch (Exception)
                {
                    // Ignore errors
                }
            }
        }

        /// <summary>
        /// Automatically loads a combolist
        /// </summary>
        public void LoadCombolist()
        {
            // Choose a file
            ChooseFile("Select a combolist", "Combolist (.txt)|*.txt");

            // Save file
            core.runSettings["combolist"] = filepath;
        }

        /// <summary>
        /// Automatically loads a combolist
        /// </summary>
        public void LoadProxylist()
        {
            // Choose the proxylist
            ChooseFile("Select a proxylist", "Proxylist (.txt)|*.txt");

            // Execute line by line
            Execute((line) =>
            {
                // Split line
                string host = line.Split(':')[0];
                string port = line.Split(':')[1];

                // Store proxy
                core.Proxies.Add(host, port);

                // Exit lambda
                return 0;
            });

            // Ask and save protocol
            core.runSettings["ProxyProtocol"] = core.Input.AskChoice("Choose proxy protocol", new string[] {
                "HTTP", "SOCKS4", "SOCKS4A", "SOCKS5"
            });
        }
    }
}
