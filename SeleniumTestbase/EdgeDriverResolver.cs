using System.Diagnostics;
using System.IO.Compression;

namespace SeleniumTestbase
{
    /// <summary>
    /// Resolves the correct msedgedriver.exe for the installed Edge version
    /// by downloading it from Microsoft's Edgedriver API.
    /// </summary>
    public static class EdgeDriverResolver
    {
        private static readonly HttpClient Http = new();
        private static readonly string CacheRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".edgedriver-cache");

        /// <summary>
        /// Returns the directory path containing msedgedriver.exe matching the installed Edge version.
        /// Downloads the driver on first use and caches it for subsequent runs.
        /// </summary>
        public static string GetDriverDirectory()
        {
            string edgeVersion = GetInstalledEdgeVersion();
            string driverDir = Path.Combine(CacheRoot, edgeVersion);
            string driverExe = Path.Combine(driverDir, "msedgedriver.exe");

            if (File.Exists(driverExe))
                return driverDir;

            Directory.CreateDirectory(driverDir);
            DownloadDriver(edgeVersion, driverDir);

            if (!File.Exists(driverExe))
                throw new FileNotFoundException(
                    $"msedgedriver.exe was not found after download for Edge {edgeVersion}.");

            return driverDir;
        }

        private static string GetInstalledEdgeVersion()
        {
            // Read version from the Edge binary's file version info
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

            if (!File.Exists(edgePath))
                edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";

            if (!File.Exists(edgePath))
                throw new FileNotFoundException("Microsoft Edge installation not found.");

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(edgePath);
            string version = versionInfo.ProductVersion
                ?? throw new InvalidOperationException("Could not determine Edge version.");

            return version;
        }

        private static void DownloadDriver(string edgeVersion, string destinationDir)
        {
            // Use the JSON API endpoint which is more reliable than the legacy azureedge CDN
            string platform = "win64";
            if (!Environment.Is64BitOperatingSystem)
                platform = "win32";

            string downloadUrl =
                $"https://msedgedriver.azureedge.net/{edgeVersion}/edgedriver_{platform}.zip";

            // Try the primary URL first, fall back to the new edgedriver endpoint
            byte[] zipBytes;
            try
            {
                zipBytes = Http.GetByteArrayAsync(downloadUrl).GetAwaiter().GetResult();
            }
            catch
            {
                // Fallback: use the newer Microsoft Edge Developer download API
                string fallbackUrl =
                    $"https://msedgewebdriverstorage.blob.core.windows.net/edgewebdriver/{edgeVersion}/edgedriver_{platform}.zip";
                zipBytes = Http.GetByteArrayAsync(fallbackUrl).GetAwaiter().GetResult();
            }

            string zipPath = Path.Combine(destinationDir, "edgedriver.zip");
            File.WriteAllBytes(zipPath, zipBytes);

            ZipFile.ExtractToDirectory(zipPath, destinationDir, overwriteFiles: true);
            File.Delete(zipPath);

            // The zip may extract into a subdirectory — move msedgedriver.exe up if needed
            string driverExe = Path.Combine(destinationDir, "msedgedriver.exe");
            if (!File.Exists(driverExe))
            {
                string? nested = Directory.GetFiles(destinationDir, "msedgedriver.exe", SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (nested != null)
                    File.Move(nested, driverExe, overwrite: true);
            }
        }
    }
}