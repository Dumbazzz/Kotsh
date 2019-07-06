using Kotsh.Models;
using Leaf.xNet;
using System.Collections.Generic;
using System.Linq;

namespace Kotsh.Blocks
{
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
        private StringContent body;

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
        /// <param name="timeout">Set default timeout</param>
        public Request Build(string URL, int timeout = 10000)
        {
            // Reset all
            this.URL = default;
            this.method = default;
            this.body = new StringContent("");
            Block.Source.Reset();

            // Initialize HttpRequest
            request = new HttpRequest
            {

                // Ignore protocol errors
                IgnoreProtocolErrors = true,

                // Set connect timeout
                ConnectTimeout = timeout,

                // Set Chrome UA by default
                UserAgent = Http.ChromeUserAgent()
            };

            // Store URL
            this.URL = Block.Dictionary.Replace(URL);

            // Return methods
            return this;
        }

        /// <summary>
        /// Add a method
        /// </summary>
        /// <param name="method">Method (GET, POST, etc...)</param>
        public Request Method(string method)
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

            // Return methods
            return this;
        }

        /// <summary>
        /// Add a single param input
        /// </summary>
        /// <param name="body">POST body</param>
        /// <param name="content_type">HTTP Content-Type header</param>
        public Request AddBody(string body, ContentType content_type)
        {
            // Append body
            this.body = new StringContent(Block.Dictionary.Replace(body), System.Text.Encoding.UTF8);

            // Set corresponding content type
            switch (content_type)
            {
                case ContentType.PLAIN:
                    AddHeader("Content-Type", "text/plain");
                    break;

                case ContentType.JSON:
                    AddHeader("Content-Type", "application/json");
                    break;

                case ContentType.FORM:
                    AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    break;
            }

            // Return method
            return this;
        }

        /// <summary>
        /// Add header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public Request AddHeader(string key, string value)
        {
            // Add header
            request[Block.Dictionary.Replace(key)] = Block.Dictionary.Replace(value);

            // Return method
            return this;
        }

        /// <summary>
        /// Make cookie jar and add as header
        /// </summary>
        /// <param name="cookies">Cookie Dictionnary as key;value</param>
        public Request AddCookies(Dictionary<string, string> cookies)
        {
            // Prepare header
            string header = "";

            // Foreach cookies
            foreach (var cookie in cookies)
            {
                header += Block.Dictionary.Replace(cookie.Key) + "=" + Block.Dictionary.Replace(cookie.Value) + "&";
            }

            // Trim last '&'
            header = header.Trim('&');

            // Send header
            this.AddHeader("Cookie", header);

            // Return method
            return this;
        }

        /// <summary>
        /// Set proxy (if defined) and execute action
        /// </summary>
        /// <param name="can_be_null">If true, it will accept blank responses</param>
        public void Execute(bool can_be_null = false)
        {
            // Set proxy
            request.Proxy = Block.core.ProxyController.Get();

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

                // Check for errors
                if (!can_be_null && response.ToString().Length < 1)
                {
                    // Increment retry
                    Block.core.RunStatistics.Increment(Type.RETRY);

                    // Response is null, relaunching it
                    Execute(true);
                }
            }
            catch (System.Exception)
            {
                // Push retry
                Block.core.RunStatistics.Increment(Type.RETRY);

                // Relaunch after issue
                Execute();
            }
            finally
            {
                // Dispose the request
                request?.Dispose();
            }
        }

        /// <summary>
        /// Set responses to Source object
        /// </summary>
        /// <param name="res">HttpResponse</param>
        private void AssignResponses(HttpResponse res)
        {
            // Last URL
            Block.Source.URL = res.Address.ToString();

            // Data response (JSON, HTML, XML, etc...)
            Block.Source.Data = res.ToString();

            // Status code
            Block.Source.Status = res.StatusCode.ToString();

            // Full response
            Block.Source.Full = res;
        }
    }
}
