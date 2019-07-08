using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;

namespace Kotsh.Blocks.Action
{
    /// <summary>
    /// Selenium allows to do browser automation
    /// Kotsh supports Selenium but your browser needs to be Chrome and to be at this path:
    /// C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
    /// </summary>
    public class Selenium
    {
        /// <summary>
        /// Block instance
        /// </summary>
        private Block Block;

        /// <summary>
        /// Selenium Web Driver
        /// </summary>
        public IWebDriver Driver;

        public bool BrowserOpen { get; set; } = false;

        /// <summary>
        /// Element selector
        /// </summary>
        private By Selector;

        /// <summary>
        /// Default timeout (60s)
        /// </summary>
        public int Timeout = 60;

        /// <summary>
        /// Ban proxy on timeout
        /// </summary>
        public bool BanOnTimeout = true;

        /// <summary>
        /// Default Chrome Driver Directory Path
        /// </summary>
        public string ChromeDriverPath = Directory.GetCurrentDirectory();

        /// <summary>
        /// Default Chrome Directory Path
        /// </summary>
        public string ChromePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";

        /// <summary>
        /// Default options for ChromeDriver
        /// </summary>
        private ChromeOptions options;

        /// <summary>
        /// Default service for ChromeDriver
        /// </summary>
        private ChromeDriverService service;

        /// <summary>
        /// Initialize class by storing Block instance
        /// </summary>
        /// <param name="block"></param>
        public Selenium(Block block)
        {
            // Store instance
            this.Block = block;

            // Set options
            options = new ChromeOptions();

            // Disable extensions
            options.AddArgument("--disable-extensions");

            // Disable logging level
            options.AddArgument("--log-level=3");

            // Set Chrome location
            options.BinaryLocation = ChromePath;

            // Disable images
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            // Create a service
            try
            {
                service = ChromeDriverService.CreateDefaultService(ChromeDriverPath);
            } catch (DriverServiceNotFoundException)
            {
                Console.WriteLine("FATAL: chromedriver.exe does not exists!");
            }

            // Disable log
            service.EnableVerboseLogging = false;
            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
        }

        /// <summary>
        /// Start the driver/browser
        /// </summary>
        /// <returns>Selenium instance</returns>
        public Selenium Start()
        {
            // Set proxy
            if (Block.core.ProxyController.UseProxy)
                options.AddArgument("--proxy-server=" + Block.core.ProxyController.GetURLProxy());

            // Open browser
            Driver = new ChromeDriver(service, options);

            // Set browser as opened
            BrowserOpen = true;

            // Return instance
            return this;
        }

        /// <summary>
        /// Navigate to a page
        /// </summary>
        /// <param name="URL">Target URL</param>
        /// <returns>Selenium instance</returns>
        public Selenium Navigate(string URL)
        {
            // Open URL
            try
            {
                Driver.Navigate().GoToUrl(URL);
            }
            catch (WebDriverTimeoutException)
            {
                if (BanOnTimeout)
                {
                    Block.response.type = Models.Type.BANNED;
                }
                else
                {
                    Block.response.type = Models.Type.RETRY;
                }

                // Relaunch after issue
                Navigate(URL);
            }
            catch (Exception)
            {
                // Push retry
                Block.core.RunStatistics.Increment(Models.Type.RETRY);

                // Relaunch after issue
                Navigate(URL);
            }

            // Set timeout
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Timeout);

            // Save informations into the source
            SaveSeleniumData();

            // Continue instance
            return this;
        }

        /// <summary>
        /// Set selector
        /// </summary>
        /// <param name="selector">Element ID</param>
        /// <returns>Selenium instance</returns>
        public Selenium SelectElementById(string selector)
        {
            // Set selector
            Selector = By.Id(selector);

            // Return instance
            return this;
        }

        /// <summary>
        /// Set selector
        /// </summary>
        /// <param name="selector">Element Name</param>
        /// <returns>Selenium instance</returns>
        public Selenium SelectElementByName(string selector)
        {
            // Set selector
            Selector = By.Name(selector);

            // Return instance
            return this;
        }

        /// <summary>
        /// Set selector
        /// </summary>
        /// <param name="selector">Element ClassName</param>
        /// <returns>Selenium instance</returns>
        public Selenium SelectElementByClassName(string selector)
        {
            // Set selector
            Selector = By.ClassName(selector);

            // Return instance
            return this;
        }

        /// <summary>
        /// Set selector
        /// </summary>
        /// <param name="selector">Element CSS Selector</param>
        /// <returns>Selenium instance</returns>
        public Selenium SelectElementByCssSelector(string selector)
        {
            // Set selector
            Selector = By.CssSelector(selector);

            // Return instance
            return this;
        }

        /// <summary>
        /// Set selector
        /// </summary>
        /// <param name="selector">Element XPath</param>
        /// <returns>Selenium instance</returns>
        public Selenium SelectElementByXPath(string selector)
        {
            // Set selector
            Selector = By.XPath(selector);

            // Return instance
            return this;
        }

        /// <summary>
        /// Wait until the page load
        /// WARNING: This does not always works
        /// </summary>
        /// <param name="timeout">Timeout, default to 20 000ms</param>
        /// <returns>Boolean</returns>
        public bool WaitForPageLoad(int timeout = 20000)
        {
            if (BrowserOpen)
            {
                try
                {
                    // Make WebDriverWait instance
                    WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeout));

                    // Wait
                    return wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                }
                catch (Exception)
                {
                    return false;
                }
            } else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return false;
            }
        }

        /// <summary>
        /// Wait for element
        /// WARNING: This element must be visible
        /// </summary>
        /// <param name="timeout">Timeout, default 20 000ms</param>
        /// <returns>Selenium instance</returns>
        public IWebElement WaitForSelector(int timeout = 20000)
        {
            if (BrowserOpen) {
                try
                {
                    // Make WebDriverWait instance
                    WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeout));

                    // Wait
                    return wait.Until(ExpectedConditions.ElementIsVisible(Selector));
                }
                catch (ElementNotVisibleException)
                {
                    Console.WriteLine($"No such element: {Selector.ToString()} could be found.");
                    return null;
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"No such element: {Selector.ToString()} could be found.");
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return null;
            }
        }

        /// <summary>
        /// Find input and fill it
        /// </summary>
        /// <param name="value">Value to fill</param>
        /// <returns>Selenium instance</returns>
        public Selenium FillInput(string value)
        {
            if (BrowserOpen) {
                try
                {
                    // Fill element
                    Driver.FindElement(Selector).SendKeys(Block.Dictionary.Replace(value));

                    // Save informations into the source
                    SaveSeleniumData();
                }
                catch (Exception) { }

                // Continue instance
                return this;
            }
            else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return null;
            }
        }

        /// <summary>
        /// Click element
        /// </summary>
        /// <returns>Selenium instance</returns>
        public Selenium Click()
        {
            if (BrowserOpen) {
                try
                {
                    // Click element
                    Driver.FindElement(Selector).Click();

                    // Save informations into the source
                    SaveSeleniumData();
                }
                catch (Exception) { }

                // Continue instance
                return this;
            }
            else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return null;
            }
        }

        /// <summary>
        /// Get element text
        /// </summary>
        /// <param name="variable">Variable to store</param>
        /// <returns>Selenium Instance</returns>
        public Selenium GetElement(string variable)
        {
            if (BrowserOpen) {
                // Get element
                string text = Driver.FindElement(Selector).Text;

                // Save variable
                Block.Dictionary.Add(variable, text);

                // Save informations into the source
                SaveSeleniumData();

                // Continue instance
                return this;
            }
            else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return null;
            }
        }

        /// <summary>
        /// Submit a form
        /// </summary>
        /// <returns>Selenium instance</returns>
        public Selenium Submit()
        {
            if (BrowserOpen) {
                // Submit form
                try
                {
                    Driver.FindElement(Selector).Submit();

                    SaveSeleniumData();
                }
                catch (Exception) { }

                // Continue instance
                return this;
            }
            else
            {
                // Set retry
                Block.response.type = Models.Type.RETRY;

                // Stop block
                Block.Stop();

                // Error
                return null;
            }
        }

        /// <summary>
        /// Stop the driver and the window
        /// </summary>
        public void Stop()
            => Driver.Close();

        #region Helpers

        /// <summary>
        /// Save status of Selenium
        /// </summary>
        private void SaveSeleniumData()
        {
            // Save informations into the source
            Block.Source.Data = Driver.PageSource;
            Block.Source.URL = Driver.Url;
        }

        #endregion
    }
}
