using Kotsh.Models;
using System.Collections.Generic;

namespace Kotsh.Objects
{
    /// <summary>
    /// Object used for rule control
    /// </summary>
    public class Requirements
    {
        /// <summary>
        /// Part (user or pass)
        /// </summary>
        public string part { get; set; }

        /// <summary>
        /// Checking rules for the part
        /// </summary>
        public Dictionary<Rules, string> rules = new Dictionary<Rules, string>();
    }
}
