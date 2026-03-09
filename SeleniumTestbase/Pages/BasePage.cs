using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace SeleniumTestbase
{
    /// <summary>
    /// Abstract base class for all Page Objects. Provides shared utilities for
    /// finding elements, waiting for visibility, scrolling, and JavaScript interaction.
    /// Every concrete page inherits these methods so common patterns are never duplicated.
    /// </summary>
    public abstract class BasePage(IWebDriver? driver, BrowserSession? session)
    {
        /// <summary>The underlying Selenium WebDriver instance.</summary>
        protected readonly IWebDriver? Driver = driver;

        /// <summary>The browser session wrapping the driver with wait/navigation helpers.</summary>
        protected readonly BrowserSession? Session = session;

        /// <summary>Selenium Actions API for complex interactions (hover, drag, etc.).</summary>
        protected readonly Actions Actions = new(driver);

        /// <summary>
        /// Waits until the page is fully loaded and ready for interaction.
        /// Each page must define its own load criteria (e.g. a specific header visible).
        /// </summary>
        public abstract void WaitToLoad();

        /// <summary>
        /// Finds a single element matching the locator, or returns null if not found.
        /// Does not throw on missing elements — safe for conditional checks.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <returns>The matching element, or null.</returns>
        protected IWebElement? GetElement(By locator)
        {
            try
            {
                return Driver?.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds all elements matching the locator. Returns an empty list if none found.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <returns>A list of matching elements, or an empty list.</returns>
        protected List<IWebElement> GetElements(By locator)
        {
            try
            {
                return Driver?.FindElements(locator).ToList() ?? [];
            }
            catch (NoSuchElementException)
            {
                return [];
            }
        }

        /// <summary>
        /// Waits until an element matching the locator is displayed.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <param name="timeoutMs">Maximum wait time in milliseconds.</param>
        /// <returns>True if the element appeared within the timeout; otherwise false.</returns>
        protected bool WaitForElement(By locator, int timeoutMs = 5000)
        {
            try
            {
                Session?.Wait(timeoutMs).Until(_ => GetElement(locator) is { Displayed: true });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Waits until an element is no longer visible or present.
        /// Useful for spinners, loading overlays, and toast messages.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <param name="timeoutMs">Maximum wait time in milliseconds.</param>
        /// <returns>True if the element disappeared within the timeout; otherwise false.</returns>
        protected bool WaitForElementToDisappear(By locator, int timeoutMs = 5000)
        {
            try
            {
                Session?.Wait(timeoutMs).Until(_ =>
                {
                    IWebElement? el = GetElement(locator);
                    return el == null || !el.Displayed;
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Scrolls the element into the center of the viewport using JavaScript.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        protected void ScrollToElement(By locator)
        {
            IWebElement? element = GetElement(locator);
            if (element != null && Driver is IJavaScriptExecutor js)
            {
                js.ExecuteScript("arguments[0].scrollIntoView({behavior:'smooth',block:'center'});", element);
            }
        }

        /// <summary>
        /// Clicks an element using JavaScript instead of the Selenium click.
        /// Bypasses overlays or intercepting elements that block standard clicks.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        protected void JsClick(By locator)
        {
            IWebElement? element = GetElement(locator);
            if (element != null && Driver is IJavaScriptExecutor js)
            {
                js.ExecuteScript("arguments[0].click();", element);
            }
        }

        /// <summary>
        /// Returns the visible text content of an element, or empty string if not found.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <returns>The element's text, or an empty string.</returns>
        protected string GetText(By locator)
        {
            return GetElement(locator)?.Text ?? string.Empty;
        }

        /// <summary>
        /// Returns the value of an HTML attribute on an element, or empty string if not found.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <param name="attribute">The attribute name (e.g. "src", "href", "value").</param>
        /// <returns>The attribute value, or an empty string.</returns>
        protected string GetAttribute(By locator, string attribute)
        {
            return GetElement(locator)?.GetAttribute(attribute) ?? string.Empty;
        }

        /// <summary>
        /// Returns true if the element exists in the DOM and is currently displayed.
        /// </summary>
        /// <param name="locator">The <see cref="By"/> locator strategy.</param>
        /// <returns>True if visible; otherwise false.</returns>
        protected bool IsDisplayed(By locator)
        {
            IWebElement? el = GetElement(locator);
            return el is { Displayed: true };
        }
    }
}