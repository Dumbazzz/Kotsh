using Kotsh.Blocks;
using Leaf.xNet;

namespace Kotsh.Objects
{
    public class Source
    {
        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public Source(Block block)
        {
            // Store instance
            this.Block = block;
        }

        public void Reset()
        {
            // Reset variables
            data = default;
            status = default;
            full = default;
        }

        /// <summary>
        /// Response data
        /// </summary>
        public string data { get; set; } = "";

        /// <summary>
        /// HTTP Status
        /// </summary>
        public string status { get; set; } = "";

        /// <summary>
        /// Full response from xNet
        /// </summary>
        public HttpResponse full { get; set; }
    }
}
