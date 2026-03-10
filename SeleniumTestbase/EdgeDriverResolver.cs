using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace SeleniumTestbase
{
    /// <summary>
    /// Resolves the correct msedgedriver for the installed Edge version
    /// by downloading it from Microsoft's Edgedriver API.
    /// Supports Windows and Linux. Thread-safe for parallel test execution.
    /// </summary>
    public static class EdgeDriverResolver
    {
        private static readonly HttpClient Http = new();
        private static readonly object DownloadLock = new();
        private static readonly string CacheRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".edgedriver-cache");

        /// <summary>
        /// Returns the directory path containing the msedgedriver binary matching the installed Edge version.
        /// Downloads the driver on first use and caches it for subsequent runs.
        /// Uses a lock to prevent parallel threads from downloading/extracting simultaneously
        /// (which causes "Text file busy" on Linux).
        /// </summary>
        public static string GetDriverDirectory()
        {
            string edgeVersion = GetInstalledEdgeVersion();
            string driverDir = Path.Combine(CacheRoot, edgeVersion);
            string driverExe = Path.Combine(driverDir, GetDriverFileName());

            // Fast path — already downloaded and ready
            if (File.Exists(driverExe))
                return driverDir;

            // Slow path — only one thread downloads at a time
            lock (DownloadLock)
            {
                // Double-check after acquiring lock (another thread may have finished)
                if (File.Exists(driverExe))
                    return driverDir;

                Directory.CreateDirectory(driverDir);
                DownloadDriver(edgeVersion, driverDir);

                if (!File.Exists(driverExe))
                    throw new FileNotFoundException(
                        $"{GetDriverFileName()} was not found after download for Edge {edgeVersion}.");

                // Ensure the binary is executable on Linux/macOS
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("chmod", $"+x \"{driverExe}\"")?.WaitForExit();
                }
            }

            return driverDir;
        }

        /// <summary>
        /// Detects the installed Edge version on the current OS.
        /// </summary>
        private static string GetInstalledEdgeVersion()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetEdgeVersionWindows();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return GetEdgeVersionLinux();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return GetEdgeVersionMac();

            throw new PlatformNotSupportedException("Unsupported OS for Edge driver resolution.");
        }

        private static string GetEdgeVersionWindows()
        {
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

            if (!File.Exists(edgePath))
                edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";

            if (!File.Exists(edgePath))
                throw new FileNotFoundException("Microsoft Edge installation not found on Windows.");

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(edgePath);
            return versionInfo.ProductVersion
                   ?? throw new InvalidOperationException("Could not determine Edge version.");
        }

        private static string GetEdgeVersionLinux()
        {
            // Try microsoft-edge-stable (installed via apt on GitHub runners)
            return RunProcess("microsoft-edge-stable", "--version")
                   ?? RunProcess("microsoft-edge", "--version")
                   ?? throw new FileNotFoundException(
                       "Microsoft Edge installation not found on Linux. Install via: sudo apt-get install -y microsoft-edge-stable");
        }

        private static string GetEdgeVersionMac()
        {
            string edgePath = "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge";
            if (!File.Exists(edgePath))
                throw new FileNotFoundException("Microsoft Edge installation not found on macOS.");

            return RunProcess(edgePath, "--version")
                   ?? throw new InvalidOperationException("Could not determine Edge version on macOS.");
        }

        /// <summary>
        /// Runs a process and extracts the version number from stdout.
        /// Expected output format: "Microsoft Edge 131.0.2903.86" → "131.0.2903.86"
        /// </summary>
        private static string? RunProcess(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process? process = Process.Start(psi);
                string? output = process?.StandardOutput.ReadToEnd().Trim();
                process?.WaitForExit();

                if (string.IsNullOrEmpty(output))
                    return null;

                // Extract version number — last token that looks like "X.X.X.X"
                string[] parts = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string? version = parts.LastOrDefault(p => p.Contains('.') && char.IsDigit(p[0]));
                return version;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the platform string for the Edge driver download URL.
        /// </summary>
        private static string GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Environment.Is64BitOperatingSystem ? "win64" : "win32";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux64";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "mac64_m1" : "mac64";

            throw new PlatformNotSupportedException("Unsupported OS for Edge driver download.");
        }

        /// <summary>
        /// Returns the driver executable filename for the current OS.
        /// </summary>
        private static string GetDriverFileName()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "msedgedriver.exe"
                : "msedgedriver";
        }

        private static void DownloadDriver(string edgeVersion, string destinationDir)
        {
            string platform = GetPlatform();

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
                string fallbackUrl =
                    $"https://msedgewebdriverstorage.blob.core.windows.net/edgewebdriver/{edgeVersion}/edgedriver_{platform}.zip";
                zipBytes = Http.GetByteArrayAsync(fallbackUrl).GetAwaiter().GetResult();
            }

            string zipPath = Path.Combine(destinationDir, "edgedriver.zip");
            File.WriteAllBytes(zipPath, zipBytes);

            ZipFile.ExtractToDirectory(zipPath, destinationDir, overwriteFiles: true);
            File.Delete(zipPath);

            // The zip may extract into a subdirectory — move the driver binary up if needed
            string driverFileName = GetDriverFileName();
            string driverExe = Path.Combine(destinationDir, driverFileName);
            if (!File.Exists(driverExe))
            {
                string? nested = Directory.GetFiles(destinationDir, driverFileName, SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (nested != null)
                    File.Move(nested, driverExe, overwrite: true);
            }
        }
    }
}