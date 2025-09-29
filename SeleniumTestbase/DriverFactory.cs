using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

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
                    return Finish(new EdgeDriver(BuildEdge(profile, headless)), settings, profile);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static (BrowserType name, BrowserProfile profile, bool headless)
            Select(BrowserSettings? s, BrowserType fromFixture)
        {
            // BrowserType from the fixture param
            BrowserType name = fromFixture;

            // Dictionary is keyed by strings like "chrome", so we convert
            string key = name.ToString().ToLowerInvariant();
            BrowserProfile? p = s?.Browsers[key];

            // Headless based on environment
            bool isServer = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            bool h = isServer ? p.Headless.Server : p.Headless.Debug;

            return (name, p, h);
        }

        static ChromeOptions BuildChrome(BrowserProfile p, bool headless)
        {
            ChromeOptions o = new ChromeOptions { AcceptInsecureCertificates = p.AcceptInsecureCertificates };
            if (headless) o.AddArgument("--headless=new");
            o.AddArgument($"--window-size={p.WindowSize}");
            foreach (string a in p.Arguments) o.AddArgument(a);
            return o;
        }

        static FirefoxOptions BuildFirefox(BrowserProfile p, bool headless)
        {
            FirefoxOptions o = new FirefoxOptions { AcceptInsecureCertificates = p.AcceptInsecureCertificates };
            if (headless) o.AddArgument("--headless");
            foreach (string a in p.Arguments) o.AddArgument(a);
            return o;
        }

        static EdgeOptions BuildEdge(BrowserProfile p, bool headless)
        {
            EdgeOptions o = new EdgeOptions();
            if (headless) o.AddArgument("--headless=new");
            o.AddArgument($"--window-size={p.WindowSize}");
            foreach (string a in p.Arguments) o.AddArgument(a);
            return o;
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
