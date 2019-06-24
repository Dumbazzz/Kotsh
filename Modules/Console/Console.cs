﻿using System;
using System.Drawing;
using System.Threading;
using ColorConsole = Colorful.Console;

namespace Kotsh.Modules.Console
{
    /// <summary>
    /// Console class allows to push messages in the console
    /// </summary>
    public class Console
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Console(Manager core)
        {
            // Store the core
            this.core = core;

            // Set default color
            ColorConsole.ForegroundColor = Color.WhiteSmoke;
        }

        /// <summary>
        /// Progress bar used for the check
        /// </summary>
        private Konsole.ProgressBar pbar;

        /// <summary>
        /// Write on Lock security (safe threading)
        /// </summary>
        private ReaderWriterLockSlim consoleLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Push a specific message with a certain level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Push(Level level, String message)
        {
            // Lock before pushing
            consoleLock.EnterWriteLock();

            try
            {
                // Use right method according to level
                switch (level)
                {
                    default:
                    case Level.INFO:
                        this.Info(message);
                        break;
                    case Level.WARNING:
                        this.Warning(message);
                        break;
                    case Level.DANGER:
                        this.Danger(message);
                        break;
                    case Level.SUCCESS:
                        this.Success(message);
                        break;
                }
            }
            finally
            {
                // Release lock
                consoleLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Create an message header
        /// </summary>
        /// <param name="level">Debugging type</param>
        /// <returns>Formatted header with type and date</returns>
        public String GetHeader(String level)
        {
            // Get date and format it
            String date = DateTime.Now.ToString("HH:mm:ss");

            // Return header
            return "[" + date + " | " + level + "] ";
        }

        /// <summary>
        /// Show informative message
        /// </summary>
        /// <param name="message">message to display</param>
        private void Info(String message)
        {
            // Write header
            ColorConsole.Write(GetHeader("INFO"), Color.Blue);

            // Write message
            ColorConsole.WriteLine(message, Color.WhiteSmoke);
        }

        /// <summary>
        /// Show warning message
        /// </summary>
        /// <param name="message">message to display</param>
        private void Warning(String message)
        {
            // Write header
            ColorConsole.Write(GetHeader("WARNING"), Color.Orange);

            // Write message
            ColorConsole.WriteLine(message, Color.WhiteSmoke);
        }

        /// <summary>
        /// Show danger message
        /// </summary>
        /// <param name="message">message to display</param>
        private void Danger(String message)
        {
            // Write header
            ColorConsole.Write(GetHeader("ERROR"), Color.Red);

            // Write message
            ColorConsole.WriteLine(message, Color.WhiteSmoke);
        }

        /// <summary>
        /// Show success message
        /// </summary>
        /// <param name="message">message to display</param>
        private void Success(String message)
        {
            // Write header
            ColorConsole.Write(GetHeader("SUCCESS"), Color.Green);

            // Write message
            ColorConsole.WriteLine(message, Color.WhiteSmoke);
        }

        /// <summary>
        /// Display formatted program title, version and author
        /// </summary>
        public void DisplayTitle()
        {
            // Break line
            ColorConsole.WriteLine();

            // Center message
            ColorConsole.SetCursorPosition((System.Console.WindowWidth - core.Program.name.Length) / 2, System.Console.CursorTop);

            // Write program name
            ColorConsole.WriteLine(core.Program.name, Color.Orange);
            
            // Break line
            ColorConsole.WriteLine();

            // Format author and version
            string subtitle = string.Format("Version: {0} | Author: {1}", core.Program.version, core.Program.author);

            // Center message
            ColorConsole.SetCursorPosition((System.Console.WindowWidth - subtitle.Length) / 2, System.Console.CursorTop);

            // Display author and version
            ColorConsole.WriteLine(subtitle, Color.Lime);

            // Break line
            ColorConsole.WriteLine();
        }

        /// <summary>
        /// Prepare display mode while checking
        /// </summary>
        public void StartRun()
        {
            // Clear console
            System.Console.Clear();

            // Display title
            this.DisplayTitle();

            // Instance progress bar
            this.pbar = new Konsole.ProgressBar(Konsole.PbStyle.DoubleLine, int.Parse(core.runStats["count"]));
        }

        /// <summary>
        /// Display advanced stats during the check
        /// </summary>
        public void UpdateRunningConsole()
        {
            // Make status text
            string status = string.Format(@"
 Hits   : {0}
 Free   : {1}
 Custom : {2}
 Expired: {3}
 Fail   : {4}
 Banned : {5}
 Retry  : {6}
 CPM    : {7}
            ", new string[] { // Replacement values
                core.runStats["hits"],
                core.runStats["free"],
                core.runStats["custom"],
                core.runStats["expired"],
                core.runStats["fail"],
                core.runStats["banned"],
                core.runStats["retry"],
                core.runStats["cpm"]
            });

            // Make progression
            this.pbar.Refresh(int.Parse(core.runStats["checked"]), status);
        }
    }
}
