using OpenQA.Selenium;
using System.Drawing;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using LogEntry = OpenQA.Selenium.LogEntry;

namespace SeleniumTestbase
{
    public enum BrowserType { Chrome, Firefox, Edge }

    public class DriverFactory
    {
        public static IWebDriver Create(BrowserType type, BrowserSettings? settings, BrowserProfile profile, bool headless)
        {
            switch (type)
            {
                case BrowserType.Chrome:
                    return Finish(new ChromeDriver(BuildChrome(profile, headless)), settings, profile);
                case BrowserType.Firefox:
                    return Finish(new FirefoxDriver(BuildFirefox(profile, headless)), settings, profile);
                case BrowserType.Edge:
                    return Finish(CreateEdge(profile, headless), settings, profile);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static (BrowserProfile profile, bool headless) Select(BrowserSettings? s, BrowserType fromFixture)
        {
            // Dictionary is keyed by strings like "chrome", so we convert
            string key = fromFixture.ToString().ToLowerInvariant();
            BrowserProfile? browserProfile = s?.Browsers[key];
            
            if (browserProfile == null)
            {
                string? available = string.Join(", ", s?.Browsers.Keys.ToArray() ?? []);
                throw new ArgumentException(
                    $"Browser profile '{key}' not found in browserSettings.json. Available: {available}");
            }

            // Headless based on environment
            bool isServer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            bool isHeadless = isServer ? browserProfile.Headless.Server : browserProfile.Headless.Debug;

            return (browserProfile, isHeadless);
        }

        static ChromeOptions BuildChrome(BrowserProfile browserProfile, bool headless)
        {
            ChromeOptions chromeOptions = new ChromeOptions { AcceptInsecureCertificates = browserProfile.AcceptInsecureCertificates };
            if (headless) chromeOptions.AddArgument("--headless=new");
            chromeOptions.AddArgument($"--window-size={browserProfile.WindowSize}");
            foreach (string a in browserProfile.Arguments) chromeOptions.AddArgument(a);
            return chromeOptions;
        }

        static FirefoxOptions BuildFirefox(BrowserProfile browserProfile, bool headless)
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions { AcceptInsecureCertificates = browserProfile.AcceptInsecureCertificates };
            if (headless) firefoxOptions.AddArgument("--headless");
            foreach (string a in browserProfile.Arguments) firefoxOptions.AddArgument(a);
            return firefoxOptions;
        }

        static EdgeOptions BuildEdge(BrowserProfile p, bool headless)
        {
            EdgeOptions edgeOptions = new EdgeOptions();
            if (headless) edgeOptions.AddArgument("--headless=new");
            edgeOptions.AddArgument($"--window-size={p.WindowSize}");
            foreach (string a in p.Arguments) edgeOptions.AddArgument(a);
            return edgeOptions;
        }

        static IWebDriver CreateEdge(BrowserProfile p, bool headless)
        {
            var options = BuildEdge(p, headless);

            // The Selenium.WebDriver.MSEdgeDriver NuGet drops msedgedriver.exe here:
            var driverDir = AppContext.BaseDirectory;
            var driverExe = Path.Combine(driverDir, "msedgedriver.exe");

            if (!File.Exists(driverExe))
                throw new FileNotFoundException($"msedgedriver.exe not found in {driverDir}. " +
                                                "Ensure Selenium.WebDriver.MSEdgeDriver is installed in the test project and the build is up-to-date.");

            var service = EdgeDriverService.CreateDefaultService(driverDir);
            // (optional) quiet logs:
            // service.UseVerboseLogging = false;
            // service.LogPath = Path.Combine(driverDir, "edgedriver.log");

            return new EdgeDriver(service, options);
        }

        static IWebDriver Finish(IWebDriver d, BrowserSettings? s, BrowserProfile p)
        {
            d.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(s.Timeouts.ImplicitMs);
            d.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(s.Timeouts.PageLoadMs);
            if (d is FirefoxDriver)
            {
                string[] sp = p.WindowSize.Split(',');
                d.Manage().Window.Size = new Size(int.Parse(sp[0]), int.Parse(sp[1]));
            }
            return d;
        }

    }
}
