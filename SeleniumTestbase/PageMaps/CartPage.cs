using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo shopping cart page.
    /// Partial class — behavior is in <see cref="Pages/CartPage.cs"/>.
    /// </summary>
    public partial class CartPage
    {
        // ── Header ───────────────────────────────────────────────────
        private readonly By _header = By.CssSelector("[data-test='title']");
        private readonly By _shoppingCartBadge = By.CssSelector("[data-test='shopping-cart-badge']");

        // ── Cart Items ───────────────────────────────────────────────
        private readonly By _cartItems = By.CssSelector("[data-test='inventory-item']");
        private readonly By _cartItemNames = By.CssSelector("[data-test='inventory-item-name']");
        private readonly By _cartItemDescriptions = By.CssSelector("[data-test='inventory-item-desc']");
        private readonly By _cartItemPrices = By.CssSelector("[data-test='inventory-item-price']");
        private readonly By _cartItemQuantities = By.CssSelector("[data-test='item-quantity']");

        // ── Remove Buttons ───────────────────────────────────────────
        private readonly By _removeButtons = By.CssSelector("button[data-test^='remove']");

        // ── Navigation Buttons ───────────────────────────────────────
        private readonly By _continueShoppingButton = By.CssSelector("[data-test='continue-shopping']");
        private readonly By _checkoutButton = By.CssSelector("[data-test='checkout']");
    }
}