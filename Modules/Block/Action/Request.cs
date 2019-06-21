using System.Collections.Generic;
using System.Linq;
using Leaf.xNet;

namespace Kotsh.Modules.Block
{
    /// <summary>
    /// Response Object/Model
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Plain-text response
        /// </summary>
        public string data = "";

        /// <summary>
        /// HTTP response code
        /// </summary>
        public string code = "";

        /// <summary>
        /// Headers with response
        /// </summary>
        public HttpResponse full;
    }

    /// <summary>
    /// Request block allows you to call URLs
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Available methods
        /// </summary>
        private string[] methods = {
            "GET",
            "POST"
        };

        /// <summary>
        /// Request builder (public to let user do custom things)
        /// </summary>
        public HttpRequest request;

        /// <summary>
        /// Target URL
        /// </summary>
        private string URL;

        /// <summary>
        /// HTTP Method
        /// </summary>
        private string method;

        /// <summary>
        /// POST body
        /// </summary>
        private RequestParams body = new RequestParams();

        /// <summary>
        /// Response Object
        /// </summary>
        public Response response = new Response();

        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public Request(Block block)
        {
            // Store instance
            this.Block = block;
        }

        /// <summary>
        /// Initialize Request class
        /// </summary>
        /// <param name="URL">Target URL</param>
        public void Build(string URL)
        {
            // Reset all
            this.URL = default;
            this.method = default;
            this.body = new RequestParams();
            this.response = new Response();

            // Initialize HttpRequest
            request = new HttpRequest();

            // Store URL
            this.URL = URL;
        }

        /// <summary>
        /// Add a method
        /// </summary>
        /// <param name="method">Method (GET, POST, etc...)</param>
        public void Method(string method)
        {
            // Make value uppercase
            method = method.ToUpper();

            // Check if method is available
            if (methods.Any(method.Contains))
            {
                // Store method
                this.method = method;
            }
            else
            {
                System.Console.WriteLine("-> Method is not available: " + method);
            }
        }

        /// <summary>
        /// Add a single param input
        /// </summary>
        public void AddBody(string key, string value)
        {
            // Add input
            body[key] = value;
        }

        /// <summary>
        /// Add header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public void AddHeader(string key, string value)
        {
            // Add header
            request.AddHeader(key, value);
        }

        /// <summary>
        /// Make cookie jar and add as header
        /// </summary>
        /// <param name="cookies">Cookie Dictionnary as key;value</param>
        public void AddCookies(Dictionary<string, string> cookies)
        {
            // Prepare header
            string header = "";

            // Foreach cookies
            foreach(var cookie in cookies)
            {
                header += cookie.Key + "=" + cookie.Value + "&";
            }

            // Trim last '&'
            header = header.Trim('&');

            // Send header
            this.AddHeader("Cookie", header);
        }

        /// <summary>
        /// Set proxy (if defined) and execute action
        /// </summary>
        /// <returns>Success/Error boolean</returns>
        public bool Execute()
        {
            // Check if proxies are used
            if (Block.core.Proxies.Count > 0)
            {
                // Get proxy type
                string type = Block.core.runSettings["ProxyProtocol"];

                // Get proxy
                string proxy = Block.core.Tasker.GetProxy();

                // Select proxy
                switch (type)
                {
                    case "HTTP":
                        request.Proxy = HttpProxyClient.Parse(proxy);
                        break;
                    case "SOCKS4":
                        request.Proxy = Socks4ProxyClient.Parse(proxy);
                        break;
                    case "SOCKS4A":
                        request.Proxy = Socks4AProxyClient.Parse(proxy);
                        break;
                    case "SOCKS5":
                        request.Proxy = Socks5ProxyClient.Parse(proxy);
                        break;
                }
            }

            // Handle errors
            try
            {
                // Handle response
                HttpResponse response;

                // Sort method
                if (method == "GET")
                {
                    response = request.Get(URL);
                }
                else
                {
                    response = request.Post(URL, body);
                }

                // Assign responses
                AssignResponses(response);

                // No errors
                return true;
            }
            catch
            {
                // Throw error
                return false;
            }
        }

        private void AssignResponses(HttpResponse res) 
        {
            // Data response (JSON, HTML, XML, etc...)
            response.data = res.ToString();

            // Status code
            response.code = res.StatusCode.ToString();

            // Full response
            response.full = res;
        }
    }
}
