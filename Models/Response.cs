using System.Collections.Specialized;

namespace Kotsh.Models
{
    /// <summary>
    /// Response class contains every details of check
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Response type (hit, free, fail, retry, etc...)
        /// </summary>
        public Type type { get; set; }

        /// <summary>
        /// Combo used for the check
        /// </summary>
        public string combo { get; set; }

        /// <summary>
        /// Captured elements for the combo
        /// </summary>
        public NameValueCollection capture = new NameValueCollection();

        /// <summary>
        /// Constructor allow to set a combo or code
        /// </summary>
        /// <param name="combo">combo</param>
        public Response(string combo = "")
        {
            this.combo = combo;
        }
    }
}
