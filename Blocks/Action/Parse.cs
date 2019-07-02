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
        /// <param name="data">Original text</param>
        /// <param name="left">Left delimiter</param>
        /// <param name="right">Right delimiter</param>
        /// <returns>Extracted text</returns>
        public string ByDelimiter(string left, string right)
        {
            // Get last response
            string data = Block.Source.data;

            // Get lengths of left/right
            int pFrom = data.IndexOf(left) + left.Length;
            int pTo = data.LastIndexOf(right);

            // Substring it
            return data.Substring(pFrom, pTo - pFrom);
        }

        /// <summary>
        /// Finds a JSON value by its key
        /// </summary>
        /// <param name="key">JSON key</param>
        /// <returns>JSON Value</returns>
        public string ByJSON(string key)
        {
            // Get last response
            string data = Block.Source.data;

            // Find value
            string value = "";

            // For an array
            if (data.StartsWith("["))
            {
                try
                {
                    // Parse and find
                    value = JArray.Parse(data).Children()[key].First().ToString();
                } catch (Exception) { }
            }
            // For an object
            else if (data.StartsWith("{"))
            {
                try
                {
                    // Parse and find
                    value = JObject.Parse(data)[key].ToString();
                } catch (Exception) { }
            }

            // Return found value
            return value;
        }

        public string ByRegex(string pattern)
        {
            // Get last response
            string data = Block.Source.data;

            // Apply regex
            Regex regex = new Regex(pattern);

            // Get capture
            return regex.Match(data).Groups[1].ToString();
        }
    }
}
