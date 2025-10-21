using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTestbase
{
    /// <summary>
    /// Root object of the JSON config file
    /// </summary>
    public class BrowserSettings
    {
        /// <summary>
        /// Base URL used by all tests
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// All browser profiles: chrome, firefox, edge
        /// </summary>
        public Dictionary<string, BrowserProfile> Browsers { get; init; } = new();

        /// <summary>
        /// Global timeout settings
        /// </summary>
        public TimeoutConfig Timeouts { get; init; } = new();
    }

    /// <summary>
    /// Configuration for each browser type
    /// </summary>
    public class BrowserProfile
    {
        /// <summary>
        /// Headless flags for debug vs server
        /// </summary>
        public HeadlessConfig Headless { get; set; } = new();

        /// <summary>
        /// Default window size in WIDTH,HEIGHT
        /// </summary>
        public string WindowSize { get; set; } = "1920,1080";

        /// <summary>
        /// Accept self-signed/invalid HTTPS certs
        /// </summary>
        public bool AcceptInsecureCertificates { get; set; }

        /// <summary>
        /// Extra command-line arguments for the browser
        /// </summary>
        public List<string> Arguments { get; set; } = new();
    }

    /// <summary>
    /// Distinguishes headless for local debugging vs server/CI
    /// </summary>
    public class HeadlessConfig
    {
        public bool Debug { get; set; }
        public bool Server { get; set; }
    }

    /// <summary>
    /// Timeout settings in milliseconds
    /// </summary>
    public class TimeoutConfig
    {
        public int ImplicitMs { get; set; } = 0;
        public int PageLoadMs { get; set; } = 60000;
    }
}
