using Kotsh.Models;
using Leaf.xNet;
using System.Collections.Generic;

namespace Kotsh.Instance
{
    public class ProxyController
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// List of proxies
        /// </summary>
        private List<Proxy> Proxies = new List<Proxy>();

        /// <summary>
        /// Number of proxies
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// Proxy progression
        /// </summary>
        public int Progression = 0;

        /// <summary>
        /// Use proxy
        /// </summary>
        public bool UseProxy { get; set; } = false;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public ProxyController(Manager core)
        {
            // Store the core
            this.core = core;
        }

        /// <summary>
        /// Add a proxy to list
        /// </summary>
        /// <param name="proxy">Full proxy</param>
        /// <param name="type">Proxy Type</param>
        public void Add(string _proxy, string _type)
        {
            // Parse type
            ProxyType type;

            switch (_type.ToUpper())
            {
                default:
                case "HTTP":
                    type = ProxyType.HTTP;
                    break;

                case "SOCKS4":
                    type = ProxyType.Socks4;
                    break;

                case "SOCKS4A":
                    type = ProxyType.Socks4A;
                    break;

                case "SOCKS5":
                    type = ProxyType.Socks5;
                    break;
            }

            // Make proxy
            Proxy proxy = new Proxy(_proxy, type);

            // Add proxy into the list
            Proxies.Add(proxy);

            // Increment count
            Count = Proxies.Count;

            // Enable proxy usage
            UseProxy = true;
        }

        /// <summary>
        /// Return a proxy
        /// </summary>
        public ProxyClient Get()
        {
            // Check proxy are enabled
            if (UseProxy && Count > 0)
            {
                // Check progression
                if (Progression >= Count)
                {
                    // Reset progression
                    Progression = 0;
                }

                // Get a proxy
                Proxy proxy = Proxies[Progression];

                // Increment progression
                Progression++;

                // Return proxy
                return proxy.GetParsedProxy();
            }
            else
            {
                return null;
            }
        }

    }
}
