using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class Headers(IWebDriver? driver, BrowserSession? session)
    {
        public Headers WaitToLoad()
        {
            session?.Wait().Until(drv => GetElement(_consentFormDismiss).Displayed);
            GetElement(_consentFormDismiss).Click();
            session.Wait().Until(drv => drv.FindElements(By.CssSelector(".loading-mask")).All(e => !e.Displayed));
            session.Wait().Until(drv => GetElement(_womenMenu).Displayed);
            return this;
        }

        public void ClickSignIn()
        {
            GetElement(_signIn).Click();
            session?.Wait().Until(drv =>
                drv.Url.Contains(
                    "https://magento.softwaretestingboard.com/customer/account/login/"));
        }

        public int GetINumberOfItemsInCart()
        {
            if (!GetElement(_cartDropDownContainer).Displayed)
            {
                GetElement(_cartButton).Click();
            }
            session?.Wait().Until(drv => GetElement(_itemsTotal).Displayed);
            string itemsTotalText = GetElement(_itemsTotal).Text;
            string numberOnly = System.Text.RegularExpressions.Regex.Match(itemsTotalText, @"\d+").Value;


            return int.Parse(numberOnly);
        }

        public string GetCartSubtotal()
        {
            if (!GetElement(_cartDropDownContainer).Displayed)
            {
                GetElement(_cartButton).Click();
            }
            session?.Wait().Until(drv => GetElement(_priceTotal).Displayed);
            string itemsTotalText = GetElement(_priceTotal).Text;

            return itemsTotalText;
        }

        public void ProceedToCheckout()
        {
            if (!GetElement(_cartDropDownContainer).Displayed)
            {
                GetElement(_cartButton).Click();
            }
            GetElement(_proceedToCheckout).Click();

        }

        public int GetCartCounterNumber()
        {
            IWebElement counter = GetElement(_cartCounterNumber);
            string counterValue = counter.Text;
            if (string.IsNullOrWhiteSpace(counterValue))
            {
                counterValue = "0";
            }

            return int.Parse(counterValue);
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
            catch (Exception)
            {
                return null!;
            }
        }
    }
}
