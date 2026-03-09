using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo checkout step two page (order overview).
    /// Partial class — behavior is in <see cref="Pages/CheckoutStepTwoPage.cs"/>.
    /// </summary>
    public partial class CheckoutStepTwoPage
    {
        // ── Header ───────────────────────────────────────────────────
        private readonly By _header = By.CssSelector("[data-test='title']");

        // ── Cart Items ───────────────────────────────────────────────
        private readonly By _cartItems = By.CssSelector("[data-test='inventory-item']");
        private readonly By _cartItemNames = By.CssSelector("[data-test='inventory-item-name']");
        private readonly By _cartItemDescriptions = By.CssSelector("[data-test='inventory-item-desc']");
        private readonly By _cartItemPrices = By.CssSelector("[data-test='inventory-item-price']");
        private readonly By _cartItemQuantities = By.CssSelector("[data-test='item-quantity']");

        // ── Payment & Shipping Info ──────────────────────────────────
        private readonly By _paymentInfoLabel = By.CssSelector("[data-test='payment-info-label']");
        private readonly By _paymentInfoValue = By.CssSelector("[data-test='payment-info-value']");
        private readonly By _shippingInfoLabel = By.CssSelector("[data-test='shipping-info-label']");
        private readonly By _shippingInfoValue = By.CssSelector("[data-test='shipping-info-value']");
        private readonly By _totalInfoLabel = By.CssSelector("[data-test='total-info-label']");

        // ── Price Summary ────────────────────────────────────────────
        private readonly By _subtotalLabel = By.CssSelector("[data-test='subtotal-label']");
        private readonly By _taxLabel = By.CssSelector("[data-test='tax-label']");
        private readonly By _totalLabel = By.CssSelector("[data-test='total-label']");

        // ── Navigation Buttons ───────────────────────────────────────
        private readonly By _cancelButton = By.CssSelector("[data-test='cancel']");
        private readonly By _finishButton = By.CssSelector("[data-test='finish']");
    }
}