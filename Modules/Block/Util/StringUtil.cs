using System.Linq;

namespace Kotsh.Modules.Block.Util
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
        /// <returns></returns>
        public string RandomString(string chars, int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
