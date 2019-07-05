using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

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

        /// <summary>
        /// Element selector
        /// </summary>
        private By Selector;

        /// <summary>
        /// Default Chrome Path
        /// </summary>
        public string ChromeDriverPath = "C:\\Program Files (x86)\\Google\\Chrome\\Application";

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

            // Disable images
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            // Create a service
            service = ChromeDriverService.CreateDefaultService(ChromeDriverPath);

            // Disable log
            service.EnableVerboseLogging = false;
            service.HideCommandPromptWindow = true;
        }

        /// <summary>
        /// Start browser and navigate to a page
        /// </summary>
        /// <param name="URL">Target URL</param>
        /// <returns>Selenium instance</returns>
        public Selenium Start(string URL)
        {
            // Set proxy
            if (Block.core.ProxyController.UseProxy)
                options.AddArgument("--proxy-server=" + Block.core.ProxyController.GetURLProxy());

            // Open browser
            Driver = new ChromeDriver(service, options);

            // Open URL
            try
            {
                Driver.Navigate().GoToUrl(URL);
            } catch (Exception)
            {
                // Push retry
                Block.core.RunStatistics.Increment(Models.Type.RETRY);

                // Relaunch after issue
                Start(URL);
            }

            // Save informations into the source
            Block.Source.Data = Driver.PageSource;
            Block.Source.URL = Driver.Url;

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
            try
            {
                // Make WebDriverWait instance
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeout));

                // Wait
                return wait.Until(driver => ((IJavaScriptExecutor) driver).ExecuteScript("return document.readyState").Equals("complete"));
            } catch (Exception)
            {
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

        /// <summary>
        /// Find input and fill it
        /// </summary>
        /// <param name="value">Value to fill</param>
        /// <returns>Selenium instance</returns>
        public Selenium FillInput(string value)
        {
            try
            {
                // Fill element
                Driver.FindElement(Selector).SendKeys(Block.Dictionary.Replace(value));

                // Save informations into the source
                Block.Source.Data = Driver.PageSource;
                Block.Source.URL = Driver.Url;
            } catch (Exception) { }

            // Continue instance
            return this;
        }

        /// <summary>
        /// Click element
        /// </summary>
        /// <returns>Selenium instance</returns>
        public Selenium Click()
        {
            try
            {
                // Click element
                Driver.FindElement(Selector).Click();

                // Save informations into the source
                Block.Source.Data = Driver.PageSource;
                Block.Source.URL = Driver.Url;
            } catch (Exception) { }

            // Continue instance
            return this;
        }

        /// <summary>
        /// Get element text
        /// </summary>
        /// <param name="variable">Variable to store</param>
        /// <returns>Selenium Instance</returns>
        public Selenium GetElement(string variable)
        {
            // Get element
            string text = Driver.FindElement(Selector).Text;

            // Save variable
            Block.Dictionary.Add(variable, text);

            // Save informations into the source
            Block.Source.Data = Driver.PageSource;
            Block.Source.URL = Driver.Url;

            // Continue instance
            return this;
        }

        /// <summary>
        /// Submit a form
        /// </summary>
        /// <returns>Selenium instance</returns>
        public Selenium Submit()
        {
            // Submit form
            try
            {
                Driver.FindElement(Selector).Submit();

                // Save informations into the source
                Block.Source.Data = Driver.PageSource;
                Block.Source.URL = Driver.Url;
            } catch (Exception) { }

            // Continue instance
            return this;
        }

        /// <summary>
        /// Stop the driver and the window
        /// </summary>
        public void Stop()
            => Driver.Close();
    }
}
