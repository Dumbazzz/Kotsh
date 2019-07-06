using Kotsh.Blocks;
using Leaf.xNet;
using System.Net;

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

        /// <summary>
        /// Last URL
        /// </summary>
        public string URL { get; set; } = "";

        /// <summary>
        /// Response data
        /// </summary>
        public string Data { get; set; } = "";

        /// <summary>
        /// HTTP Status
        /// </summary>
        public string Status { get; set; } = "";

        /// <summary>
        /// Full response from xNet
        /// </summary>
        public HttpResponse Full { get; set; }

        /// <summary>
        /// Reset all values
        /// </summary>
        public void Reset()
        {
            // Reset variables
            URL = default;
            Data = default;
            Status = default;
            Full = default;
        }

        /// <summary>
        /// Return header value if exists
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Value</returns>
        public string GetHeader(string key)
        {
            // Check if header is set
            if (Full.ContainsHeader(key))
            {
                return Full[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return a cookie
        /// </summary>
        /// <param name="key">Cookie name</param>
        /// <returns>Cookie array</returns>
        public Cookie GetCookie(string key)
        {
            // Check if cookie exists
            if (Full.ContainsCookie(key))
            {
                // Return cookie array
                return Block.Request.cookies.GetCookies(URL)[key];
            } else
            {
                // Does not exists
                return null;
            }
        }
    }
}
