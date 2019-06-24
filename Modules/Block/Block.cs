using Kotsh.Modules.Block.Util;

namespace Kotsh.Modules.Block
{
    /// <summary>
    /// Block class
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Core instance
        /// </summary>
        public Manager core;

        /// <summary>
        /// Blocks availables
        /// </summary>
        public Request Request;

        /// <summary>
        /// Utilities blocks
        /// </summary>
        public StringUtil StringUtil;

        /// <summary>
        /// Store core instance and initialize blocks
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Block(Manager core)
        {
            // Store the core
            this.core = core;

            // Start instances
            this.Request = new Request(this);
            this.StringUtil = new StringUtil(this);
        }
    }
}
