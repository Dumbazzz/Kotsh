using System;
using System.IO;
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
        /// Default opening directory
        /// </summary>
        private String directory = Directory.GetCurrentDirectory();

        /// <summary>
        /// This variable contains last chosen file
        /// </summary>
        public String filepath;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public FileHelper(Manager core)
        {
            // Store the core
            this.core = core;
        }

        public void ChooseFile(String title, String fileType)
        {
            // Open file chooser instance
            OpenFileDialog ofd = new OpenFileDialog
            {
                // Setup file chooser
                InitialDirectory = directory,
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
                this.directory = Path.GetDirectoryName(this.filepath);
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
        public void Execute(Func<String, int> function)
        {
            // Read file line by line
            var lines = File.ReadLines(this.filepath);
            foreach (String line in lines)
            {
                // Execute lambda function
                function.Invoke(line);
            }
        }
    }
}
