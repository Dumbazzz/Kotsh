using Kotsh.Blocks.Action;
using Kotsh.Blocks.Util;
using Kotsh.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kotsh.Blocks
{
    /// <summary>
    /// Block class
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Core instance
        /// </summary>
        public Manager core;

        /// <summary>
        /// Blocks availables
        /// </summary>
        public Request Request;
        public Parse Parse;

        /// <summary>
        /// Utilities blocks
        /// </summary>
        public StringUtil StringUtil;
        public Source Source;

        /// <summary>
        /// Combo
        /// </summary>
        public string combo;

        /// <summary>
        /// Username and Password
        /// </summary>
        public string username;
        public string password;

        /// <summary>
        /// Can continue automation
        /// </summary>
        public bool running = true;

        /// <summary>
        /// Store core instance and initialize blocks
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Block(Manager core)
        {
            // Store the core
            this.core = core;

            // Start instances
            this.Request = new Request(this);
            this.Parse = new Parse(this);
            this.StringUtil = new StringUtil(this);
            this.Source = new Source(this);
        }

        /// <summary>
        /// Execute a list of actions
        /// </summary>
        /// <param name="combo">Save combo for automatic usage</param>
        /// <param name="methods">List of actions</param>
        public void Automate(string combo, List<System.Action> methods)
        {
            // Reset running
            running = true;

            // Save combo
            if (combo != "")
                SetCombo(combo);

            // Execute every methods
            foreach (System.Action method in methods)
            {
                // Check if running is allowed
                if (running)
                {
                    // Invoke method
                    method.Invoke();
                }
            }
        }

        /// <summary>
        /// Set a combo and split it
        /// </summary>
        /// <param name="combo">Combo</param>
        private void SetCombo(string combo)
        {
            // Save full definition
            this.combo = combo;

            // Split and save definitions
            username = combo.Split(':')[0];
            password = combo.Split(':')[1];
        }

        /// <summary>
        /// Stop block running
        /// </summary>
        public void Stop()
            => running = false;
    }
}
