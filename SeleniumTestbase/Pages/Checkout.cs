using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTestbase
{
    public partial class Checkout(IWebDriver? driver, BrowserSession? session)
    {
        public Checkout WaitToLoad()
        {
            session?.Wait().Until(drv => drv.FindElements(By.CssSelector(".loading-mask")).All(e => !e.Displayed));
            session?.Wait().Until(drv => GetElement(_email)?.Displayed);

            return this;
        }

        public void InputEmail(string email)
        {
            GetElement(_email)?.SendKeys(email);
        }

        public void InputFirstName(string firstName)
        {
            GetElement(_firstName)?.SendKeys(firstName);
        }

        public void InputLastName(string lastname)
        {
            GetElement(_lastname)?.SendKeys(lastname);
        }
        public void InputAddress(string address)
        {
            GetElement(_address)?.SendKeys(address);
        }
        public void InputCity(string city)
        {
            GetElement(_city)?.SendKeys(city);
        }
        public void SelectState(string name)
        {
            IWebElement? dropdown = GetElement(_stateSelector);
            dropdown?.Click();
            SelectElement select = new SelectElement(dropdown);
            select.SelectByText(name);
        }
        public void InputPostalCode(string postalCode)
        {
            GetElement(_postalCode)?.SendKeys(postalCode);
        }
        public void InputPhoneNumber(string phoneNumber)
        {
            GetElement(_phoneNumber)?.SendKeys(phoneNumber);
        }

        public void GotToReviewAndPayments()
        {
            GetElement(_nextButton)?.Click();
            session?.Wait().Until(drv => drv.FindElements(By.CssSelector(".loading-mask")).All(e => !e.Displayed));
            session?.Wait().Until(drv => GetElement(_placeOrder)?.Displayed);
        }


        public void PlaceOrder()
        {
            GetElement(_placeOrder)?.Click();
            session?.Wait().Until(drv => drv.FindElements(By.CssSelector(".loading-mask")).All(e => !e.Displayed));
        }

        public string? GetPageTitle()
        {
            return GetElement(_pageTitle)?.Text;
        }


        public void SelectShipping(bool cheap)
        {
            if (cheap)
            {
                GetElement(_radioTableRate)?.Click();
            }
            else
            {
                GetElement(_radioFixed)?.Click();
            }
        }




        /// <summary>
        /// Retrieves a single element using a specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>IWebElement.</returns>
        private IWebElement? GetElement(By locator)
        {
            try
            {
                return driver?.FindElement(locator);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
