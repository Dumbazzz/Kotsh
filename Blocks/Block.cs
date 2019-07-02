using Kotsh.Blocks.Action;
using Kotsh.Blocks.Util;

namespace Kotsh.Blocks
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
        public Parse Parse;

        /// <summary>
        /// Utilities blocks
        /// </summary>
        public StringUtil StringUtil;
        public Source Source;

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
            this.Parse = new Parse(this);
            this.StringUtil = new StringUtil(this);
            this.Source = new Source(this);
        }
    }
}
