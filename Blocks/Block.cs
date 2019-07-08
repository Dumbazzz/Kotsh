using Kotsh.Blocks.Action;
using Kotsh.Blocks.Util;
using Kotsh.Models;
using Kotsh.Objects;
using System.Collections.Generic;

namespace Kotsh.Blocks
{
    /// <summary>
    /// Block class
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Kotsh instance
        /// </summary>
        public Manager core;

        /// <summary>
        /// Blocks availables
        /// </summary>
        public Request Request;
        public Parse Parse;
        public KeyCheck KeyCheck;
        public Selenium Selenium;

        /// <summary>
        /// Utilities blocks
        /// </summary>
        public StringUtil StringUtil;
        public Dictionary Dictionary;

        /// <summary>
        /// Objects
        /// </summary>
        public Source Source;

        /// <summary>
        /// Response
        /// </summary>
        public Response response;

        /// <summary>
        /// Can continue automation
        /// </summary>
        private bool running = true;

        /// <summary>
        /// Store core instance and initialize blocks
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Block(Manager core)
        {
            // Store instance
            this.core = core;

            // Start instances
            this.Request = new Request(this);
            this.Parse = new Parse(this);
            this.KeyCheck = new KeyCheck(this);
            this.Selenium = new Selenium(this);
            this.StringUtil = new StringUtil(this);
            this.Dictionary = new Dictionary(this);
            this.Source = new Source(this);
        }

        /// <summary>
        /// Execute a list of actions
        /// </summary>
        /// <param name="combo">Save combo for automatic usage</param>
        /// <param name="methods">List of actions</param>
        public Response Automate(string combo, Response response, List<System.Action> methods)
        {
            // Save response
            this.response = response;

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

            // Return response
            return this.response;
        }

        /// <summary>
        /// Set a combo and split it
        /// </summary>
        /// <param name="combo">Combo</param>
        private void SetCombo(string combo)
        {
            // Save full definition
            Dictionary.Add("combo", combo);

            // Split and save definitions
            Dictionary.Add("user", combo.Split(':')[0]);
            Dictionary.Add("pass", combo.Split(':')[1]);
        }

        /// <summary>
        /// Stop block running
        /// </summary>
        public void Stop()
            => running = false;
    }
}
