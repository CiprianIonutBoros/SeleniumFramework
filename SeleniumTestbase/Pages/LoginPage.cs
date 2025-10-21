using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeleniumTestbase
{
    public partial class LoginPage(IWebDriver? driver, BrowserSession? session) 
    {
        readonly Actions _actions = new Actions(driver);

        public void WaitToLoad()
        { 
            session?.Wait().Until(_ => GetElement(_loginButton) is { Displayed: true });
        }

        public void Login(UserType type)
        {
            User? user = GetUserByType(type);
            GetElement(_username)?.SendKeys(user.Username);
            GetElement(_password)?.SendKeys(user.Password);
            IWebElement? loginButton = GetElement(_loginButton);
            loginButton?.Click();
        }


        private User? GetUserByType(UserType type)
        {
            string json = File.ReadAllText("users.json");
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            UserContainer? container = JsonSerializer.Deserialize<UserContainer>(json, options);

            User? user = container?.Users.FirstOrDefault(u => u.Type == type);
            if (user == null)
            {
                string available = string.Join(", ", container?.Users ?? new List<User>());
                throw new ArgumentException(
                    $"User type '{type}' not found in Users.json. Available: {available}");
            }

            return user;
        }

        /// <summary>
        /// Retrieves a single element using a specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>IWebElement.</returns>
        private IWebElement? GetElement(By locator)
        {
            try
            {
                return driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }
    }
}
