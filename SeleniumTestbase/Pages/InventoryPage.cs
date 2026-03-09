using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTestbase
{
    public partial class InventoryPage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            WaitForElement(_header);
        }

        /// <summary>
        /// Returns the page title text (e.g. "Products").
        /// </summary>
        public string GetHeaderText()
        {
            return GetElement(_header)?.Text ?? string.Empty;
        }

        /// <summary>
        /// Returns all inventory item names displayed on the page.
        /// </summary>
        public List<string> GetItemNames()
        {
            return GetElements(_inventoryItemNames)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns all inventory item prices as text (e.g. "$29.99").
        /// </summary>
        public List<string> GetItemPrices()
        {
            return GetElements(_inventoryItemPrices)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns the total count of inventory items on the page.
        /// </summary>
        public int GetItemCount()
        {
            return GetElements(_inventoryItems).Count;
        }

        /// <summary>
        /// Adds an item to the cart by its product name.
        /// </summary>
        public void AddItemToCartByName(string productName)
        {
            Log.Step($"Adding '{productName}' to cart");
            string dataTestValue = "add-to-cart-" + productName.ToLower().Replace(" ", "-");
            By addButton = By.CssSelector($"button[data-test='{dataTestValue}']");
            GetElement(addButton)?.Click();
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
        /// Clicks the shopping cart icon to navigate to the cart page.
        /// </summary>
        public void GoToCart()
        {
            Log.Step("Navigating to cart");
            GetElement(_shoppingCartLink)?.Click();
        }

        /// <summary>
        /// Opens the burger menu sidebar.
        /// </summary>
        public void OpenBurgerMenu()
        {
            GetElement(_burgerMenuButton)?.Click();
            WaitForElement(_logoutLink, 5000);
        }

        /// <summary>
        /// Logs out via the burger menu.
        /// </summary>
        public void Logout()
        {
            Log.Step("Logging out via burger menu");
            OpenBurgerMenu();
            GetElement(_logoutLink)?.Click();
        }

        /// <summary>
        /// Sorts inventory items using the dropdown.
        /// Values: "az", "za", "lohi", "hilo".
        /// </summary>
        public void SortBy(string value)
        {
            Log.Step($"Sorting items by '{value}'");
            IWebElement? dropdown = GetElement(_sortDropdown);
            if (dropdown != null)
            {
                SelectElement select = new SelectElement(dropdown);
                select.SelectByValue(value);
            }
        }

        /// <summary>
        /// Clicks on an inventory item name to navigate to its detail page.
        /// </summary>
        public void OpenItemByName(string productName)
        {
            Log.Step($"Opening item detail for '{productName}'");
            IWebElement? item = GetElements(_inventoryItemNames)
                .FirstOrDefault(e => e.Text.Equals(productName, StringComparison.OrdinalIgnoreCase));
            item?.Click();
        }

        /// <summary>
        /// Checks whether all inventory item images have a valid src attribute.
        /// </summary>
        public bool AllItemImagesAreValid()
        {
            return GetElements(_inventoryItemImages)
                .All(img =>
                {
                    string? src = img.GetAttribute("src");
                    return !string.IsNullOrEmpty(src) && !src.Contains("sl-404");
                });
        }
    }
}