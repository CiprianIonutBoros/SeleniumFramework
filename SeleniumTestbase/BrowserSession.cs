using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public void NavigateToMenu(params string[] menuItemIds)
        {
            if (driver == null) throw new NullReferenceException("Driver not initialized");

            Actions actions = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            for (int i = 0; i < menuItemIds.Length; i++)
            {
                IWebElement menuItem = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(menuItemIds[i])));

                // Hover over the menu item
                actions.MoveToElement(menuItem).Perform();

                // If it's not the last element, wait for the next submenu to appear
                if (i + 1 < menuItemIds.Length)
                {
                    string nextMenuItemId = menuItemIds[i + 1];
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id(nextMenuItemId)));
                }
                else
                {
                    // Last item: wait until it's clickable and click it
                    wait.Until(ExpectedConditions.ElementToBeClickable(menuItem)).Click();
                }
            }
        }
    }
}
