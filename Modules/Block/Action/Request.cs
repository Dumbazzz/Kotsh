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
        public string data { get; set; }

        /// <summary>
        /// HTTP response code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Headers with response
        /// </summary>
        public HttpResponse full { get; set; }
    }

    /// <summary>
    /// Request block allows you to call URLs
    /// </summary>
    public class Request : Block
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
        public readonly Response response = new Response();

        /// <summary>
        /// Initialize Request class
        /// </summary>
        /// <param name="URL">Target URL</param>
        public Request(string URL)
        {
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
        /// Execute request
        /// </summary>
        public void Execute()
        {
            // Sort method
            switch(method)
            {
                // GET method
                case "GET":
                    // Execute
                    AssignResponses(request.Get(URL));
                    break;

                // POST method
                case "POST":
                    // Execute
                    AssignResponses(request.Post(URL, body));
                    break;
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
