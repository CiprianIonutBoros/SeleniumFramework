using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class CartPage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            Session?.Wait().Until(_ => GetElement(_header) is { Displayed: true, Text: "Your Cart" });
        }

        /// <summary>
        /// Returns the page title text (e.g. "Your Cart").
        /// </summary>
        public string GetHeaderText()
        {
            return GetElement(_header)?.Text ?? string.Empty;
        }

        /// <summary>
        /// Returns the total number of items in the cart.
        /// </summary>
        public int GetCartItemCount()
        {
            return GetElements(_cartItems).Count;
        }

        /// <summary>
        /// Returns all cart item names.
        /// </summary>
        public List<string> GetItemNames()
        {
            return GetElements(_cartItemNames)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns all cart item prices as text (e.g. "$29.99").
        /// </summary>
        public List<string> GetItemPrices()
        {
            return GetElements(_cartItemPrices)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns all cart item quantities as integers.
        /// </summary>
        public List<int> GetItemQuantities()
        {
            return GetElements(_cartItemQuantities)
                .Select(e => int.TryParse(e.Text, out int qty) ? qty : 0)
                .ToList();
        }

        /// <summary>
        /// Checks whether a specific product is present in the cart.
        /// </summary>
        public bool IsItemInCart(string productName)
        {
            return GetElements(_cartItemNames)
                .Any(e => e.Text.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Removes an item from the cart by its product name.
        /// </summary>
        public void RemoveItemByName(string productName)
        {
            Log.Step($"Removing '{productName}' from cart");
            string dataTestValue = "remove-" + productName.ToLower().Replace(" ", "-");
            By removeButton = By.CssSelector($"button[data-test='{dataTestValue}']");
            GetElement(removeButton)?.Click();
        }

        /// <summary>
        /// Returns the number shown on the shopping cart badge, or 0 if no badge is visible.
        /// </summary>
        public int GetCartBadgeCount()
        {
            IWebElement? badge = GetElement(_shoppingCartBadge);
            return badge != null && int.TryParse(badge.Text, out int count) ? count : 0;
        }

        /// <summary>
        /// Returns true if the cart is empty (no items displayed).
        /// </summary>
        public bool IsCartEmpty()
        {
            return GetElements(_cartItems).Count == 0;
        }

        /// <summary>
        /// Clicks "Continue Shopping" to navigate back to the inventory page.
        /// </summary>
        public void ContinueShopping()
        {
            Log.Step("Clicking Continue Shopping");
            GetElement(_continueShoppingButton)?.Click();
        }

        /// <summary>
        /// Clicks "Checkout" to proceed to the checkout step one page.
        /// </summary>
        public void Checkout()
        {
            Log.Step("Clicking Checkout");
            GetElement(_checkoutButton)?.Click();
        }
    }
}