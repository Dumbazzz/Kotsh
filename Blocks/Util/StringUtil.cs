using System.Linq;
using System.Text.RegularExpressions;

namespace Kotsh.Blocks.Util
{
    public class StringUtil
    {
        /// <summary>
        /// Initialize random instance
        /// </summary>
        private static System.Random random = new System.Random();

        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public StringUtil(Block block)
        {
            // Store instance
            this.Block = block;
        }

        /// <summary>
        /// Generate a random string with possible letters
        /// </summary>
        /// <param name="length">String length</param>
        /// <returns>Random String</returns>
        public string RandomString(string chars, int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generate a random integer between a minimum and a maximum value
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <returns>Random Int</returns>
        public int RandomInt(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Count occurrences in the source
        /// </summary>
        /// <param name="needle">String to check</param>
        /// <returns>Count as integer</returns>
        public int CountOccurrences(string needle)
        {
            // Get source
            string haystack = Block.Source.Data;
            
            // Return occurences count
            return new Regex(Regex.Escape(needle)).Matches(haystack).Count;
        }
    }
}
