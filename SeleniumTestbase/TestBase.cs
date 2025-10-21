using NUnit.Framework;
using OpenQA.Selenium;
using System.Text.Json;
using TestContext = NUnit.Framework.TestContext;

namespace SeleniumTestbase
{
    public abstract class TestBase(BrowserType browser)
    {
        protected BrowserSession? Session;
        private BrowserSettings? _browserSettings;

        [SetUp]
        public void SetUp()
        {
            _browserSettings = LoadBrowserSettings();
            (BrowserProfile profile, bool headless) = DriverFactory.Select(this._browserSettings, browser);
            IWebDriver driver = DriverFactory.Create(browser, _browserSettings, profile, headless);
            Session = new BrowserSession(driver);
            Session.Navigate(_browserSettings?.BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            Session?.Dispose();
        }

        private static BrowserSettings? LoadBrowserSettings()
        {
            string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "browserSettings.json");
            // read JSON file
            string jsonText = File.ReadAllText(path);

            // deserialize into the class
            BrowserSettings? settings = JsonSerializer.Deserialize<BrowserSettings>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return settings;
        }
    }
}