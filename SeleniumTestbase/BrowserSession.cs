using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTestbase
{
    public sealed class BrowserSession(IWebDriver? driver) : IDisposable
    {
        public IWebDriver? Driver { get; } = driver;

        public void Dispose()
        {
            Driver?.Quit();
        }

        public void Navigate(string? url)
        {
            Driver?.Navigate().GoToUrl(url ?? throw new ArgumentNullException(nameof(url)));
        }

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
