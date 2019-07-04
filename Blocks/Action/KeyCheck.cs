using Kotsh.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kotsh.Blocks.Action
{
    public class KeyCheck
    {
        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Every keys
        /// </summary>
        private Dictionary<string, Type> Keys = new Dictionary<string, Type>();

        /// <summary>
        /// Set as BAN if no key match is found
        /// </summary>
        private bool BanIfNotFound = true;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public KeyCheck(Block block)
        {
            // Store instance
            this.Block = block;
        }

        /// <summary>
        /// Disable ban if keys are not found
        /// </summary>
        /// <returns>KeyCheck instance</returns>
        public KeyCheck DisableBan()
        {
            // Disable banning
            BanIfNotFound = false;

            // Continue method
            return this;
        }

        /// <summary>
        /// Enable ban if keys are not found
        /// </summary>
        /// <returns>KeyCheck instance</returns>
        public KeyCheck EnableBan()
        {
            // Enable banning
            BanIfNotFound = true;

            // Continue method
            return this;
        }

        /// <summary>
        /// Add key matches
        /// </summary>
        /// <param name="type">Key type</param>
        /// <param name="keys">Match</param>
        /// <returns>KeyCheck instance</returns>
        public KeyCheck SetKeys(Type type, string[] keys)
        {
            // For each keys
            foreach (string key in keys)
            {
                // Add key
                Keys.Add(key, type);
            }

            // Continue instance
            return this;
        }

        /// <summary>
        /// Check and assign response
        /// </summary>
        /// <param name="response">Response type</param>
        public Response Check(Response response)
        {
            // Get data
            string data = Block.Source.Data;

            // Temporary variables
            bool found = false;

            // Check every keys
            foreach (string key in Keys.Keys)
            {
                // Check match
                if (!found && data.Contains(key))
                {
                    // Set as found
                    found = true;

                    // Save type as response
                    response.type = Keys[key];
                }
            }

            // Ban if not found
            if (BanIfNotFound && !found)
            {
                // Set as banned
                response.type = Type.BANNED;
            }

            // Stop the block on fail, ban or retry
            if (response.type == Type.BANNED || response.type == Type.RETRY || response.type == Type.FAIL)
            {
                // Stop block execution
                Block.Stop();
            }

            // Reset ban once checked
            BanIfNotFound = true;

            // Reset dictionary
            Keys.Clear();

            // Send response
            return response;
        }
    }
}