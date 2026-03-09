using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTestbase
{
    /// <summary>
    /// Wraps an <see cref="IWebDriver"/> instance with convenience methods
    /// for navigation, waiting, and JavaScript execution. Each test gets
    /// its own <see cref="BrowserSession"/> via <see cref="TestBase.SetUp"/>,

    /// ensuring complete isolation between parallel tests.
    /// </summary>
    public sealed class BrowserSession(IWebDriver? driver) : IDisposable
    {
        /// <summary>
        /// The underlying Selenium WebDriver instance.
        /// </summary>
        public IWebDriver? Driver { get; } = driver;

        /// <summary>
        /// Returns the current page URL, or empty string if the driver is null.
        /// </summary>
        public string CurrentUrl => Driver?.Url ?? string.Empty;

        /// <summary>
        /// Returns the current page title, or empty string if the driver is null.
        /// </summary>
        public string PageTitle => Driver?.Title ?? string.Empty;

        /// <summary>
        /// Quits the browser and releases all associated resources.
        /// </summary>
        public void Dispose()
        {
            Driver?.Quit();
        }

        /// <summary>
        /// Navigates the browser to the specified URL.
        /// </summary>
        /// <param name="url">The absolute URL to navigate to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="url"/> is null.</exception>
        public void Navigate(string? url)
        {
            Driver?.Navigate().GoToUrl(url ?? throw new ArgumentNullException(nameof(url)));
        }

        /// <summary>
        /// Navigates the browser back to the previous page in history.
        /// </summary>
        public void NavigateBack()
        {
            Driver?.Navigate().Back();
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void RefreshPage()
        {
            Driver?.Navigate().Refresh();
        }

        /// <summary>
        /// Executes arbitrary JavaScript in the browser context.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">Arguments passed to the script (accessible via arguments[0], etc.).</param>
        /// <returns>The value returned by the script, or null.</returns>
        public object? ExecuteScript(string script, params object[] args)
        {
            if (Driver is IJavaScriptExecutor js)
                return js.ExecuteScript(script, args);
            return null;
        }

        /// <summary>
        /// Waits until the browser URL contains the specified text fragment.
        /// </summary>
        /// <param name="fragment">The URL substring to wait for.</param>
        /// <param name="timeoutMs">Maximum wait time in milliseconds.</param>
        /// <returns>True if the URL contains the fragment within the timeout; otherwise false.</returns>
        public bool WaitForUrlContaining(string fragment, int timeoutMs = 10000)
        {
            try
            {
                Wait(timeoutMs).Until(ExpectedConditions.UrlContains(fragment));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a configured <see cref="WebDriverWait"/> with common exception types
        /// already ignored (NoSuchElement, NotFound, StaleElementReference).
        /// </summary>
        /// <param name="timeoutMilliseconds">Maximum wait time in milliseconds. Default is 35 seconds.</param>
        /// <param name="pollingMilliseconds">Polling interval in milliseconds. Default is 250ms.</param>
        /// <returns>A pre-configured <see cref="WebDriverWait"/> instance.</returns>
        public WebDriverWait Wait(int timeoutMilliseconds = 35000, int pollingMilliseconds = 250)
        {
            WebDriverWait wait = new WebDriverWait(new SystemClock(), driver: Driver,
                TimeSpan.FromMilliseconds(timeoutMilliseconds), TimeSpan.FromMilliseconds(pollingMilliseconds));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(NotFoundException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }
    }
}
