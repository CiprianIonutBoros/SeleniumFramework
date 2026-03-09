using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Element locators for the SauceDemo inventory/products page.
    /// Partial class — behavior is in <see cref="Pages/InventoryPage.cs"/>.
    /// </summary>
    public partial class InventoryPage
    {
        // ── Header & Navigation ──────────────────────────────────────
        private readonly By _header = By.CssSelector("[data-test='title']");
        private readonly By _shoppingCartLink = By.CssSelector("[data-test='shopping-cart-link']");
        private readonly By _shoppingCartBadge = By.CssSelector("[data-test='shopping-cart-badge']");

        // ── Burger Menu ──────────────────────────────────────────────
        private readonly By _burgerMenuButton = By.Id("react-burger-menu-btn");
        private readonly By _logoutLink = By.Id("logout_sidebar_link");
        private readonly By _closeBurgerMenuButton = By.Id("react-burger-cross-btn");

        // ── Sort ─────────────────────────────────────────────────────
        private readonly By _sortDropdown = By.CssSelector("[data-test='product-sort-container']");

        // ── Inventory Items ──────────────────────────────────────────
        private readonly By _inventoryItems = By.CssSelector("[data-test='inventory-item']");
        private readonly By _inventoryItemNames = By.CssSelector("[data-test='inventory-item-name']");
        private readonly By _inventoryItemDescriptions = By.CssSelector("[data-test='inventory-item-desc']");
        private readonly By _inventoryItemPrices = By.CssSelector("[data-test='inventory-item-price']");
        private readonly By _inventoryItemImages = By.CssSelector("img.inventory_item_img");

        // ── Add to Cart / Remove Buttons ─────────────────────────────
        private readonly By _addToCartButtons = By.CssSelector("button[data-test^='add-to-cart']");
        private readonly By _removeButtons = By.CssSelector("button[data-test^='remove']");
    }
}