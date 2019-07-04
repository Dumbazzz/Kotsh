using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kotsh.Blocks.Action
{
    public class Parse
    {
        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public Parse(Block block)
        {
            // Store instance
            this.Block = block;
        }

        /// <summary>
        /// Extract text between delimiters
        /// </summary>
        /// <param name="variable">Dictionary key</param>
        /// <param name="left">Left delimiter</param>
        /// <param name="right">Right delimiter</param>
        /// <returns>Extracted text</returns>
        public string ByDelimiter(string variable, string left, string right)
        {
            // Substring it
            string value = Block.Source.Full.ToString().Substring(left, right);

            // Add to dictionary
            Block.Dictionary.Add(variable, value);

            // Return value
            return value;
        }

        /// <summary>
        /// Finds a JSON value by its key
        /// </summary>
        /// <param name="variable">Dictionary key</param>
        /// <param name="key">JSON key</param>
        /// <returns>JSON Value</returns>
        public string ByJSON(string variable, string key)
        {
            // Get last response
            string data = Block.Source.Data;

            // Find value
            string value = "";

            // Try
            try
            {
                // For an array
                if (data.StartsWith("["))
                {
                    // Parse and find
                    value = JArray.Parse(data).Children()[key].First().ToString();
                }
                // For an object
                else if (data.StartsWith("{"))
                {
                    // Parse and find
                    value = JObject.Parse(data)[key].ToString();
                }
            }
            catch (Exception)
            {
                // Error => Value stay at default
                value = "";
            }

            // Add to dictionary
            Block.Dictionary.Add(variable, value);

            // Return found value
            return value;
        }

        /// <summary>
        /// Get a value from a regex match
        /// </summary>
        /// <param name="variable">Dictionary key</param>
        /// <param name="pattern">Regex Pattern</param>
        /// <returns>Regex Match</returns>
        public string ByRegex(string variable, string pattern)
        {
            // Get last response
            string data = Block.Source.Data;

            // Apply regex
            Regex regex = new Regex(pattern);

            // Get capture
            string value = regex.Match(data).Groups[1].ToString();

            // Add to dictionary
            Block.Dictionary.Add(variable, value);

            // Return value
            return value;
        }
    }
}
