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

        public void Add(string key, string value)
            => variables.TryAdd(key, value);

        public string Get(string key)
        {
            return variables[key];
        }

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
