using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class LoginPage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            WaitForElement(_loginButton);
        }

        public void Login(UserType type)
        {
            Log.Step($"Logging in as {type}");
            User user = UserProvider.GetByType(type);
            GetElement(_username)?.SendKeys(user.Username!);
            GetElement(_password)?.SendKeys(user.Password!);
            GetElement(_loginButton)?.Click();
        }

        public bool IsErrorMessageDisplayed()
        {
            return WaitForElement(_errorMessage);
        }

        public string GetErrorMessageText()
        {
            return GetElement(_errorMessage)?.Text ?? string.Empty;
        }

        public bool IsInventoryPageDisplayed()
        {
            return WaitForElement(_inventoryContainer);
        }
    }
}
