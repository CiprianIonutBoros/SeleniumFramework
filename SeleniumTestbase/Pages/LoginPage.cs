using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class LoginPage(IWebDriver driver, BrowserSession? session)
    {
        public LoginPage WaitToLoad()
        {
            session.Wait().Until(drv => GetElement(_signInButton).Displayed);
            return this;
        }

        public void LogIn()
        {
            IWebElement emailInput = GetElement(_emaiInput);
            IWebElement passwordInput = GetElement(_passwordInput);

            emailInput.Click();
            emailInput.SendKeys("test@zitec.ro");
            passwordInput.Click();
            passwordInput.SendKeys("P@ssw0rd");

            GetElement(_signInButton).Click();

            session?.Wait().Until(drv => drv.Url.Equals("https://magento.softwaretestingboard.com/"));
        }


        /// <summary>
        /// Retrieves a single element using a specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>IWebElement.</returns>
        private IWebElement GetElement(By locator)
        {
            try
            {
                return driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
    }
}
