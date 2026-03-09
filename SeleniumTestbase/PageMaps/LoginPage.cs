using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo login page (https://www.saucedemo.com/).
    /// Partial class — behavior is in <see cref="Pages/LoginPage.cs"/>.
    /// </summary>
    public partial class LoginPage
    {
        // ── Form Fields ──────────────────────────────────────────────
        private readonly By _username = By.Id("user-name");
        private readonly By _password = By.Id("password");
        private readonly By _loginButton = By.Id("login-button");

        // ── Error Display ────────────────────────────────────────────
        private readonly By _errorMessage = By.CssSelector("[data-test='error']");

        // ── Post-Login Verification ──────────────────────────────────
        private readonly By _inventoryContainer = By.Id("inventory_container");
    }
}
