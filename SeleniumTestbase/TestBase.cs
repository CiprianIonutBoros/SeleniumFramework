using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using TestContext = NUnit.Framework.TestContext;

namespace SeleniumTestbase
{
    public abstract class TestBase
    {
        protected BrowserSession? Session;
        protected PageNavigator Pages = null!;
        private BrowserSettings? _browserSettings;
        private BrowserType _browser;
        private bool _headless;

        private static readonly string ScreenshotDir = Path.Combine(
            TestContext.CurrentContext.WorkDirectory, "Screenshots");

        [SetUp]
        public void SetUp()
        {
            _browser = ResolveBrowser();
            _browserSettings = LoadBrowserSettings();
            (BrowserProfile profile, bool headless) = DriverFactory.Select(_browserSettings, _browser);
            _headless = headless;

            SlotLayout? slot = _headless ? null : WindowLayoutManager.ClaimSlot();

            IWebDriver driver = DriverFactory.Create(_browser, _browserSettings, profile, headless, slot);
            Session = new BrowserSession(driver);
            Pages = new PageNavigator(Session);

            string baseUrl = ResolveBaseUrl(_browserSettings);
            Log.Info($"Browser: {_browser} | Environment: {Environment.GetEnvironmentVariable("TEST_ENV") ?? "default"} | URL: {baseUrl}");
            Session.Navigate(baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                Log.Error($"Test FAILED: {TestContext.CurrentContext.Test.FullName}");
                CaptureScreenshot();
            }

            if (!_headless)
            {
                WindowLayoutManager.ReleaseSlot();
            }

            Session?.Dispose();
        }

        // ── Navigation Helpers ───────────────────────────────────────
        // Reusable across all test fixtures. Each creates a fresh
        // browser state via [SetUp] + InstancePerTestCase.

        protected void LoginAsStandard()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Standard);
            Pages.Inventory.WaitToLoad();
        }

        protected void NavigateToCheckoutStepOne(params string[] products)
        {
            LoginAsStandard();
            foreach (string product in products)
                Pages.Inventory.AddItemToCartByName(product);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Pages.Cart.Checkout();
            Pages.CheckoutStepOne.WaitToLoad();
        }

        protected void NavigateToCheckoutStepTwo(params string[] products)
        {
            NavigateToCheckoutStepOne(products);
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);
            Pages.CheckoutStepTwo.WaitToLoad();
        }

        protected void NavigateToCheckoutComplete(params string[] products)
        {
            NavigateToCheckoutStepTwo(products);
            Pages.CheckoutStepTwo.Finish();
            Pages.CheckoutComplete.WaitToLoad();
        }

        // ── Private Infrastructure ───────────────────────────────────

        private static BrowserType ResolveBrowser()
        {
            string browserEnv = Environment.GetEnvironmentVariable("BROWSER") ?? "Edge";

            if (Enum.TryParse<BrowserType>(browserEnv, ignoreCase: true, out BrowserType browser))
            {
                return browser;
            }

            string available = string.Join(", ", Enum.GetNames<BrowserType>());
            throw new ArgumentException(
                $"Invalid BROWSER environment variable '{browserEnv}'. Available: {available}");
        }

        private static string ResolveBaseUrl(BrowserSettings? settings)
        {
            string env = Environment.GetEnvironmentVariable("TEST_ENV") ?? "";

            if (!string.IsNullOrEmpty(env)
                && settings?.Environments != null
                && settings.Environments.TryGetValue(env, out string? envUrl)
                && !string.IsNullOrEmpty(envUrl))
            {
                return envUrl;
            }

            return settings?.BaseUrl ?? throw new InvalidOperationException(
                "No base URL configured. Set TEST_ENV or provide a baseUrl in browserSettings.json.");
        }

        private void CaptureScreenshot()
        {
            try
            {
                if (Session?.Driver is ITakesScreenshot screenshotDriver)
                {
                    Directory.CreateDirectory(ScreenshotDir);

                    string testName = TestContext.CurrentContext.Test.FullName;
                    string sanitized = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filePath = Path.Combine(ScreenshotDir, $"{sanitized}_{timestamp}.png");

                    Screenshot screenshot = screenshotDriver.GetScreenshot();
                    screenshot.SaveAsFile(filePath);

                    TestContext.AddTestAttachment(filePath, "Screenshot on failure");
                    Log.Info($"Screenshot saved: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to capture screenshot: {ex.Message}");
            }
        }

        private static BrowserSettings? LoadBrowserSettings()
        {
            string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "browserSettings.json");
            string jsonText = File.ReadAllText(path);

            BrowserSettings? settings = JsonConvert.DeserializeObject<BrowserSettings>(jsonText);

            return settings;
        }
    }
}