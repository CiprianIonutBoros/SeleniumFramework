using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using OpenQA.Selenium;
using TestContext = NUnit.Framework.TestContext;

namespace SeleniumTestbase
{
    public abstract class TestBase(BrowserType browser)
    {
        protected BrowserSession? _session;
        private BrowserSettings? _browserSettings;

        [SetUp]
        public void SetUp()
        {
            _browserSettings = LoadBrowserSettings();
            (BrowserType name, BrowserProfile profile, bool headless) = DriverFactory.Select(this._browserSettings, browser);
            IWebDriver driver = DriverFactory.Create(name, _browserSettings, profile, headless);
            _session = new BrowserSession(driver);
            _session.Navigate(_browserSettings?.BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _session?.Dispose();
        }

        private static BrowserSettings? LoadBrowserSettings()
        {
            string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "browserSettings.json");
            // read JSON file
            string jsonText = File.ReadAllText(path);

            // deserialize into the class
            BrowserSettings? settings = JsonConvert.DeserializeObject<BrowserSettings>(jsonText);

            return settings;
        }
    }
}