using Kotsh.Models;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Net;

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
        public enum Methods
        {
            /// <summary>
            /// GET method
            /// </summary>
            GET,

            /// <summary>
            /// POST method
            /// </summary>
            POST
        }

        /// <summary>
        /// Request builder (public to let user do custom things)
        /// </summary>
        public HttpRequest request;

        /// <summary>
        /// Cookies
        /// </summary>
        public CookieStorage cookies = new CookieStorage();

        /// <summary>
        /// Target URL
        /// </summary>
        private Uri URL;

        /// <summary>
        /// HTTP Method
        /// </summary>
        private Methods method;

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
                UserAgent = Http.ChromeUserAgent(),
                
                // Cookies
                Cookies = cookies
            };

            // Store URL
            this.URL = new Uri(Block.Dictionary.Replace(URL));

            // Return methods
            return this;
        }

        /// <summary>
        /// Add a method
        /// </summary>
        /// <param name="method">Method (GET, POST, etc...)</param>
        public Request Method(Methods method)
        {
            // Save method
            this.method = method;

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
        /// <param name="name">Cookie name</param>
        /// <param name="value">Cookie value</param>
        public Request AddCookies(string name, string value)
        {
            // Create cookie
            Cookie cookie = new Cookie(name, value);

            // Set domain
            cookie.Domain = URL.Host;

            // Set path
            cookie.Path = URL.PathAndQuery;

            // Add cookie
            cookies.Add(cookie);

            // Return method
            return this;
        }

        /// <summary>
        /// Clear cookie container
        /// </summary>
        /// <returns>Instance</returns>
        public Request ClearCookies()
        {
            // Clear cookie container
            cookies.Clear();

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
                if (method == Methods.GET)
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
                    Block.core.RunStatistics.Increment(Models.Type.RETRY);

                    // Response is null, relaunching it
                    Execute(true);
                }
            }
            catch (System.Exception)
            {
                // Push retry
                Block.core.RunStatistics.Increment(Models.Type.RETRY);

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
