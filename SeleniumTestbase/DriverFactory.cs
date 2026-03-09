using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace SeleniumTestbase
{
    public enum BrowserType { Chrome, Firefox, Edge }

    public class DriverFactory
    {
        public static IWebDriver Create(BrowserType type, BrowserSettings? settings, BrowserProfile profile, bool headless, SlotLayout? slot = null)
        {
            switch (type)
            {
                case BrowserType.Chrome:
                    return Finish(new ChromeDriver(BuildChrome(profile, headless, slot)), type, settings, slot);
                case BrowserType.Firefox:
                    return Finish(new FirefoxDriver(BuildFirefox(profile, headless, slot)), type, settings, slot);
                case BrowserType.Edge:
                    return Finish(CreateEdge(profile, headless, slot), type, settings, slot);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static (BrowserProfile profile, bool headless) Select(BrowserSettings? s, BrowserType fromFixture)
        {
            string key = fromFixture.ToString().ToLowerInvariant();
            BrowserProfile? browserProfile = s?.Browsers[key];

            if (browserProfile == null)
            {
                string available = string.Join(", ", s?.Browsers.Keys.ToArray() ?? []);
                throw new ArgumentException(
                    $"Browser profile '{key}' not found in browserSettings.json. Available: {available}");
            }

            bool isServer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            bool isHeadless = isServer ? browserProfile.Headless.Server : browserProfile.Headless.Debug;

            return (browserProfile, isHeadless);
        }

        static ChromeOptions BuildChrome(BrowserProfile browserProfile, bool headless, SlotLayout? slot)
        {
            ChromeOptions chromeOptions = new ChromeOptions { AcceptInsecureCertificates = browserProfile.AcceptInsecureCertificates };
            if (headless) chromeOptions.AddArgument("--headless=new");

            // Use slot layout for size/position so browser opens directly in place
            if (slot != null)
            {
                chromeOptions.AddArgument(slot.WindowSizeArg);
                chromeOptions.AddArgument(slot.WindowPositionArg);
            }
            else
            {
                chromeOptions.AddArgument($"--window-size={browserProfile.WindowSize}");
            }

            foreach (string a in browserProfile.Arguments) chromeOptions.AddArgument(a);

            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);

            return chromeOptions;
        }

        static FirefoxOptions BuildFirefox(BrowserProfile browserProfile, bool headless, SlotLayout? slot)
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions { AcceptInsecureCertificates = browserProfile.AcceptInsecureCertificates };
            if (headless) firefoxOptions.AddArgument("--headless");

            // Firefox supports --width/--height but not --window-position as launch args
            if (slot != null)
            {
                firefoxOptions.AddArgument($"--width={slot.Width}");
                firefoxOptions.AddArgument($"--height={slot.Height}");
            }

            foreach (string a in browserProfile.Arguments) firefoxOptions.AddArgument(a);
            return firefoxOptions;
        }

        static EdgeOptions BuildEdge(BrowserProfile p, bool headless, SlotLayout? slot)
        {
            EdgeOptions edgeOptions = new EdgeOptions();
            if (headless) edgeOptions.AddArgument("--headless=new");

            // Use slot layout for size/position so browser opens directly in place
            if (slot != null)
            {
                edgeOptions.AddArgument(slot.WindowSizeArg);
                edgeOptions.AddArgument(slot.WindowPositionArg);
            }
            else
            {
                edgeOptions.AddArgument($"--window-size={p.WindowSize}");
            }

            foreach (string a in p.Arguments) edgeOptions.AddArgument(a);

            edgeOptions.AddUserProfilePreference("credentials_enable_service", false);
            edgeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            edgeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);

            return edgeOptions;
        }

        static IWebDriver CreateEdge(BrowserProfile p, bool headless, SlotLayout? slot)
        {
            EdgeOptions options = BuildEdge(p, headless, slot);
            string driverDir = EdgeDriverResolver.GetDriverDirectory();
            EdgeDriverService service = EdgeDriverService.CreateDefaultService(driverDir);
            return new EdgeDriver(service, options);
        }

        static IWebDriver Finish(IWebDriver d, BrowserType type, BrowserSettings? s, SlotLayout? slot)
        {
            d.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(s.Timeouts.ImplicitMs);
            d.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(s.Timeouts.PageLoadMs);

            // Firefox doesn't support --window-position at launch, so apply it post-creation.
            // Size is already correct from --width/--height, and position is set instantly
            // before any navigation — no visible flash.
            if (type == BrowserType.Firefox && slot != null)
            {
                WindowLayoutManager.ApplyPosition(d, slot);
            }

            return d;
        }
    }
}
