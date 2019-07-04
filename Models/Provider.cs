using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotsh.Models
{
    /// <summary>
    /// Provider is every source that can be used for parsing and key checking
    /// </summary>
    public enum Provider
    {
        /// <summary>
        /// Source code from HTTP request
        /// </summary>
        SOURCE,

        /// <summary>
        /// Final URL from HTTP request
        /// </summary>
        ADDRESS
    }
}
