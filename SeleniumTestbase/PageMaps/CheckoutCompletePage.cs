using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo checkout complete page (order confirmation).
    /// Partial class — behavior is in <see cref="Pages/CheckoutCompletePage.cs"/>.
    /// </summary>
    public partial class CheckoutCompletePage
    {
        // ── Header ───────────────────────────────────────────────────
        private readonly By _header = By.CssSelector("[data-test='title']");

        // ── Confirmation Content ─────────────────────────────────────
        private readonly By _completeHeader = By.CssSelector("[data-test='complete-header']");
        private readonly By _completeText = By.CssSelector("[data-test='complete-text']");
        private readonly By _ponyExpressImage = By.CssSelector("[data-test='pony-express']");

        // ── Navigation ───────────────────────────────────────────────
        private readonly By _backHomeButton = By.CssSelector("[data-test='back-to-products']");
    }
}