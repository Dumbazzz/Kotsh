using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kotsh.Blocks.Util
{
    public class Dictionary
    {
        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Dictionary with temporary variables
        /// </summary>
        private ConcurrentDictionary<string, string> variables = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public Dictionary(Block block)
        {
            // Store instance
            this.Block = block;
        }

        /// <summary>
        /// Add a value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, string value)
            => variables.TryAdd(key, value);

        /// <summary>
        /// Get a value using key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public string Get(string key)
        {
            if (variables.ContainsKey(key))
            {
                return variables[key];
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// Replace temporary variables in the text
        /// </summary>
        /// <param name="text">Original text</param>
        /// <returns>Generated text</returns>
        public string Replace(string text)
        {
            // Make list with keys
            List<string> keys = new List<string>(variables.Keys);

            // Make new text
            string new_text = text;

            // Replace variables
            foreach (string key in keys)
            {
                // Make selector
                string selector = "<" + key.ToUpper() + ">";

                // Replace
                new_text = new_text.Replace(selector, Get(key));
            }

            // Return generated text
            return new_text;
        }
    }
}
