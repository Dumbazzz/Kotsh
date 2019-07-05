using Kotsh.Models;
using System.Collections.Generic;

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
        /// Providing source
        /// </summary>
        private Provider Provider { get; set; } = Provider.SOURCE;

        /// <summary>
        /// Source to check according to the Provider
        /// </summary>
        private string Data = "";

        /// <summary>
        /// Use a callback on error
        /// </summary>
        private bool UseCallback = false;

        /// <summary>
        /// Callback on error
        /// </summary>
        private System.Action CallbackOnError;

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

        public KeyCheck Source(Provider provider = Provider.SOURCE)
        {
            // Switch provider
            switch (provider)
            {
                default:
                case Provider.SOURCE:
                    Data = Block.Source.Data;
                    break;

                case Provider.ADDRESS:
                    Data = Block.Source.URL;
                    break;
            }

            // Continue method
            return this;
        }

        /// <summary>
        /// This callback will get called if an error occur, like a FAIL, BAN or RETRY type
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public KeyCheck ActionOnError(System.Action callback)
        {
            // Save callback
            CallbackOnError = callback;

            // Enable callback
            UseCallback = true;

            // Continue method
            return this;
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
        /// Check if source contains keys
        /// </summary>
        /// <param name="response">Response type</param>
        public Response Contains(Response response)
        {
            // Temporary variables
            bool found = false;

            // Check every keys
            foreach (string key in Keys.Keys)
            {
                // Check match
                if (!found && Data.Contains(key))
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

                // Call the callback
                if (UseCallback)
                    CallbackOnError.Invoke();
            }

            // Reset ban once checked
            BanIfNotFound = true;

            // Reset dictionary
            Keys.Clear();

            // Send response
            return response;
        }

        /// <summary>
        /// Check if source equals keys
        /// </summary>
        /// <param name="response">Response type</param>
        public Response Equals(Response response)
        {
            // Temporary variables
            bool found = false;

            // Check every keys
            foreach (string key in Keys.Keys)
            {
                // Check match
                if (!found && Data.Equals(key))
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

                // Call the callback
                if (UseCallback)
                    CallbackOnError.Invoke();
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