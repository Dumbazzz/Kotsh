using Leaf.xNet;
using System.Text;

namespace Kotsh.Models
{
    public class Proxy
    {
        /// <summary>
        /// Non-parsed proxy
        /// </summary>
        private string Full { get; set; } = "";

        /// <summary>
        /// Proxy host
        /// </summary>
        private string Host { get; set; } = "";

        /// <summary>
        /// Proxy port
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// Proxy username
        /// </summary>
        private string Username { get; set; } = "";

        /// <summary>
        /// Proxy password
        /// </summary>
        private string Password { get; set; } = "";

        /// <summary>
        /// Proxy Type, by default set on HTTP
        /// </summary>
        private ProxyType Type { get; set; } = ProxyType.HTTP;

        /// <summary>
        /// Proxy uses username and password to work
        /// </summary>
        private bool HasCredentials { get; set; } = false;

        /// <summary>
        /// Add a proxy
        /// </summary>
        /// <param name="proxy">Non-parsed proxy</param>
        /// <param name="type">ProxyType</param>
        public Proxy(string proxy, ProxyType type)
        {
            // Save full proxy
            Full = proxy;

            // Save type
            Type = type;

            // Split proxy
            string[] parts = proxy.Split(':');

            // Parse host
            Host = parts[0];

            // Parse port
            Port = int.Parse(parts[1]);

            // Assign username and password if present
            if (parts.Length == 3 && parts[2] != null && parts[3] != null)
            {
                // Use credentials
                HasCredentials = true;

                // Parse username
                Username = parts[2];

                // Parse password
                Password = parts[3];
            }
        }

        /// <summary>
        /// Returns full proxy
        /// </summary>
        /// <returns>Proxy like host:port[:user:pass]</returns>
        public string GetFullProxy()
            => Full;

        /// <summary>
        /// Return a host:port
        /// </summary>
        /// <returns>Proxy with only host:port</returns>
        public string GetProxy()
            => Host + ":" + Port.ToString();

        /// <summary>
        /// Return proxy username
        /// </summary>
        /// <returns>Username</returns>
        public string GetUsername()
            => Username;

        /// <summary>
        /// Return proxy password
        /// </summary>
        /// <returns>password</returns>
        public string GetPassword()
            => Password;

        /// <summary>
        /// Return proxy credentials
        /// </summary>
        /// <returns>Credentials as user:pass</returns>
        public string GetCredentials()
            => Username + ":" + Password;

        /// <summary>
        /// Return true if proxy is using username and password
        /// </summary>
        /// <returns>Boolean</returns>
        public bool UseCredentials()
            => HasCredentials;

        /// <summary>
        /// Return a parsed and ready-to-use proxy
        /// </summary>
        /// <returns>ProxyClient</returns>
        public ProxyClient GetParsedProxy()
        {
            // Switch type
            switch (Type)
            {
                case ProxyType.HTTP:
                    return HttpProxyClient.Parse(Full);

                case ProxyType.Socks4:
                    return Socks4ProxyClient.Parse(Full);

                case ProxyType.Socks4A:
                    return Socks4AProxyClient.Parse(Full);

                case ProxyType.Socks5:
                    return Socks5ProxyClient.Parse(Full);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Return a proxy with url form (e.g.: protocol://[user:pass@]host:port)
        /// </summary>
        /// <returns>Proxy as URL</returns>
        public string GetURLProxy()
        {
            // Make Selenium Proxy
            StringBuilder proxy = new StringBuilder();

            // Switch type
            switch (Type)
            {
                case ProxyType.HTTP:
                    proxy.Append("http://");
                    break;

                case ProxyType.Socks4:
                    proxy.Append("socks4://");
                    break;

                case ProxyType.Socks4A:
                    proxy.Append("socks4a://");
                    break;

                case ProxyType.Socks5:
                    proxy.Append("socks5://");
                    break;

                default:
                    return null;
            }

            // Set crendetials
            if (UseCredentials())
            {
                proxy.Append(GetCredentials() + "@");
            }

            // Set proxy
            proxy.Append(GetProxy());

            // Return proxy
            return proxy.ToString();
        }
    }
}
