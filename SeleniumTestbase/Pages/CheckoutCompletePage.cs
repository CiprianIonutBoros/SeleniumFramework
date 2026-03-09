using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class CheckoutCompletePage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            Session?.Wait().Until(_ => GetElement(_header) is { Displayed: true, Text: "Checkout: Complete!" });
        }

        /// <summary>
        /// Returns the page title text (e.g. "Checkout: Complete!").
        /// </summary>
        public string GetHeaderText()
        {
            return GetText(_header);
        }

        /// <summary>
        /// Returns the confirmation header text (e.g. "Thank you for your order!").
        /// </summary>
        public string GetCompleteHeaderText()
        {
            return GetText(_completeHeader);
        }

        /// <summary>
        /// Returns the confirmation body text.
        /// </summary>
        public string GetCompleteText()
        {
            return GetText(_completeText);
        }

        /// <summary>
        /// Returns true if the pony express image is displayed.
        /// </summary>
        public bool IsPonyExpressImageDisplayed()
        {
            return IsDisplayed(_ponyExpressImage);
        }

        /// <summary>
        /// Clicks "Back Home" to navigate back to the inventory page.
        /// </summary>
        public void BackHome()
        {
            Log.Step("Clicking Back Home");
            GetElement(_backHomeButton)?.Click();
        }
    }
}