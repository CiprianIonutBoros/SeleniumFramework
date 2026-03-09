using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class CheckoutStepTwoPage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            Session?.Wait().Until(_ => GetElement(_header) is { Displayed: true, Text: "Checkout: Overview" });
        }

        /// <summary>
        /// Returns the page title text (e.g. "Checkout: Overview").
        /// </summary>
        public string GetHeaderText()
        {
            return GetText(_header);
        }

        // ── Cart Items ───────────────────────────────────────────────

        /// <summary>
        /// Returns the total number of items in the checkout summary.
        /// </summary>
        public int GetItemCount()
        {
            return GetElements(_cartItems).Count;
        }

        /// <summary>
        /// Returns all item names in the checkout summary.
        /// </summary>
        public List<string> GetItemNames()
        {
            return GetElements(_cartItemNames)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns all item prices as text (e.g. "$29.99").
        /// </summary>
        public List<string> GetItemPrices()
        {
            return GetElements(_cartItemPrices)
                .Select(e => e.Text)
                .ToList();
        }

        /// <summary>
        /// Returns all item quantities as integers.
        /// </summary>
        public List<int> GetItemQuantities()
        {
            return GetElements(_cartItemQuantities)
                .Select(e => int.TryParse(e.Text, out int qty) ? qty : 0)
                .ToList();
        }

        /// <summary>
        /// Checks whether a specific product is present in the checkout summary.
        /// </summary>
        public bool IsItemInSummary(string productName)
        {
            return GetElements(_cartItemNames)
                .Any(e => e.Text.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        // ── Payment & Shipping ───────────────────────────────────────

        /// <summary>
        /// Returns the payment information value (e.g. "SauceCard #31337").
        /// </summary>
        public string GetPaymentInfo()
        {
            return GetText(_paymentInfoValue);
        }

        /// <summary>
        /// Returns the shipping information value (e.g. "Free Pony Express Delivery!").
        /// </summary>
        public string GetShippingInfo()
        {
            return GetText(_shippingInfoValue);
        }

        // ── Price Summary ────────────────────────────────────────────

        /// <summary>
        /// Returns the subtotal text (e.g. "Item total: $29.99").
        /// </summary>
        public string GetSubtotalText()
        {
            return GetText(_subtotalLabel);
        }

        /// <summary>
        /// Returns the subtotal as a decimal value.
        /// </summary>
        public decimal GetSubtotalAmount()
        {
            return ParsePrice(GetSubtotalText());
        }

        /// <summary>
        /// Returns the tax text (e.g. "Tax: $2.40").
        /// </summary>
        public string GetTaxText()
        {
            return GetText(_taxLabel);
        }

        /// <summary>
        /// Returns the tax as a decimal value.
        /// </summary>
        public decimal GetTaxAmount()
        {
            return ParsePrice(GetTaxText());
        }

        /// <summary>
        /// Returns the total text (e.g. "Total: $32.39").
        /// </summary>
        public string GetTotalText()
        {
            return GetText(_totalLabel);
        }

        /// <summary>
        /// Returns the total as a decimal value.
        /// </summary>
        public decimal GetTotalAmount()
        {
            return ParsePrice(GetTotalText());
        }

        // ── Navigation ───────────────────────────────────────────────

        /// <summary>
        /// Clicks "Finish" to complete the order.
        /// </summary>
        public void Finish()
        {
            Log.Step("Clicking Finish");
            GetElement(_finishButton)?.Click();
        }

        /// <summary>
        /// Clicks "Cancel" to navigate back to the inventory page.
        /// </summary>
        public void Cancel()
        {
            Log.Step("Clicking Cancel");
            GetElement(_cancelButton)?.Click();
        }

        // ── Helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Extracts a decimal price from text like "Item total: $29.99" or "Total: $32.39".
        /// </summary>
        private static decimal ParsePrice(string text)
        {
            string digits = text.Substring(text.IndexOf('$') + 1);
            return decimal.TryParse(digits, out decimal value) ? value : 0m;
        }
    }
}