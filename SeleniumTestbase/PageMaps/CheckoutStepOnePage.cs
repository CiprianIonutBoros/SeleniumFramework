using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo checkout step one page (customer information).
    /// Partial class — behavior is in <see cref="Pages/CheckoutStepOnePage.cs"/>.
    /// </summary>
    public partial class CheckoutStepOnePage
    {
        // ── Header ───────────────────────────────────────────────────
        private readonly By _header = By.CssSelector("[data-test='title']");

        // ── Form Fields ──────────────────────────────────────────────
        private readonly By _firstNameInput = By.CssSelector("[data-test='firstName']");
        private readonly By _lastNameInput = By.CssSelector("[data-test='lastName']");
        private readonly By _postalCodeInput = By.CssSelector("[data-test='postalCode']");

        // ── Error Display ────────────────────────────────────────────
        private readonly By _errorMessage = By.CssSelector("[data-test='error']");

        // ── Navigation Buttons ───────────────────────────────────────
        private readonly By _cancelButton = By.CssSelector("[data-test='cancel']");
        private readonly By _continueButton = By.CssSelector("[data-test='continue']");
    }
}