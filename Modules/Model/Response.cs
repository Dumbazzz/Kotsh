﻿using System.Collections.Specialized;

namespace Kotsh.Modules.Model
{
    /// <summary>
    /// Response class contains every details of check
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Response type (hit, free, fail, retry, etc...)
        /// </summary>
        public Type type;

        /// <summary>
        /// Combo used for the check
        /// </summary>
        public string combo;

        /// <summary>
        /// Captured elements for the combo
        /// </summary>
        public NameValueCollection capture = new NameValueCollection();
    }
}
