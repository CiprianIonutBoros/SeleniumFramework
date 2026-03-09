using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Tiles parallel browser instances side-by-side without overlap.
    /// Slots are claimed before browser launch so position/size can be
    /// injected as startup arguments — browsers open directly in place.
    ///
    /// Layout on a 1920×1080 screen with 3 slots:
    /// ┌──────────┬──────────┬──────────┐
    /// │  Slot 0  │  Slot 1  │  Slot 2  │
    /// │  640×1080│  640×1080│  640×1080│
    /// │  x=0     │  x=640   │  x=1280  │
    /// └──────────┴──────────┴──────────┘
    /// </summary>
    public static class WindowLayoutManager
    {
        /// <summary>Thread-safe pool of available slot indices.</summary>
        private static readonly ConcurrentQueue<int> AvailableSlots = new();

        /// <summary>Maps managed thread ID → claimed slot index for release tracking.</summary>
        private static readonly ConcurrentDictionary<int, int> ThreadSlots = new();

        /// <summary>Number of horizontal columns the screen is divided into.</summary>
        private static int _columns;

        /// <summary>
        /// Static constructor — initializes with 3 columns by default,
        /// matching <c>[assembly: LevelOfParallelism(3)]</c>.
        /// </summary>
        static WindowLayoutManager()
        {
            Reset(3);
        }

        /// <summary>
        /// Resets the slot pool with the specified number of columns.
        /// Call this if <c>LevelOfParallelism</c> changes.
        /// </summary>
        /// <param name="columns">Number of horizontal columns to divide the screen into.</param>
        public static void Reset(int columns)
        {
            _columns = columns;
            AvailableSlots.Clear();
            ThreadSlots.Clear();
            for (int i = 0; i < columns; i++)
                AvailableSlots.Enqueue(i);
        }

        /// <summary>
        /// Claims a slot and returns the window size and position.
        /// Call <b>before</b> creating the browser driver so the layout can
        /// be injected as launch arguments (no flash/repositioning).
        /// </summary>
        /// <returns>A <see cref="SlotLayout"/> with position and size for the browser window.</returns>
        public static SlotLayout ClaimSlot()
        {
            int threadId = Environment.CurrentManagedThreadId;

            // Dequeue the next available slot; default to 0 if pool is exhausted
            if (!AvailableSlots.TryDequeue(out int slot))
            {
                slot = 0;
                Log.Warn($"No available layout slot for thread {threadId}. Defaulting to slot 0.");
            }

            // Track which thread owns which slot for release
            ThreadSlots[threadId] = slot;

            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            int windowWidth = screenWidth / _columns;
            int xOffset = slot * windowWidth;

            Log.Info($"Claimed slot {slot + 1}/{_columns} | Position ({xOffset}, 0) | Size {windowWidth}x{screenHeight}");

            return new SlotLayout(slot, xOffset, 0, windowWidth, screenHeight);
        }

        /// <summary>
        /// Releases the slot claimed by the current thread back to the pool.
        /// Call in <c>[TearDown]</c> before quitting the driver.
        /// </summary>
        public static void ReleaseSlot()
        {
            int threadId = Environment.CurrentManagedThreadId;
            if (ThreadSlots.TryRemove(threadId, out int slot))
                AvailableSlots.Enqueue(slot);
        }

        /// <summary>
        /// Applies window position and size to a driver that doesn't support
        /// <c>--window-position</c> as a launch argument (Firefox).
        /// Called immediately after driver creation, before any navigation.
        /// </summary>
        /// <param name="driver">The WebDriver instance to reposition.</param>
        /// <param name="layout">The slot layout containing position and size.</param>
        public static void ApplyPosition(IWebDriver driver, SlotLayout layout)
        {
            driver.Manage().Window.Position = new Point(layout.X, layout.Y);
            driver.Manage().Window.Size = new Size(layout.Width, layout.Height);
        }

        /// <summary>
        /// Returns the primary screen width in pixels.
        /// Uses Win32 <c>GetSystemMetrics</c> on Windows; falls back to 1920.
        /// </summary>
        private static int GetScreenWidth()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetSystemMetrics(0); // SM_CXSCREEN
            return 1920;
        }

        /// <summary>
        /// Returns the primary screen height in pixels.
        /// Uses Win32 <c>GetSystemMetrics</c> on Windows; falls back to 1080.
        /// </summary>
        private static int GetScreenHeight()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetSystemMetrics(1); // SM_CYSCREEN
            return 1080;
        }

        /// <summary>
        /// Win32 API call to retrieve system metrics (screen dimensions).
        /// </summary>
        /// <param name="nIndex">The metric index (0 = screen width, 1 = screen height).</param>
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }

    /// <summary>
    /// Immutable record representing a screen slot's position and dimensions.
    /// Provides pre-formatted browser launch arguments for Chrome/Edge.
    /// </summary>
    /// <param name="Slot">Zero-based slot index.</param>
    /// <param name="X">Horizontal pixel offset from the left edge of the screen.</param>
    /// <param name="Y">Vertical pixel offset from the top edge of the screen.</param>
    /// <param name="Width">Window width in pixels.</param>
    /// <param name="Height">Window height in pixels.</param>
    public record SlotLayout(int Slot, int X, int Y, int Width, int Height)
    {
        /// <summary>Chrome/Edge launch argument: <c>--window-size=W,H</c></summary>
        public string WindowSizeArg => $"--window-size={Width},{Height}";

        /// <summary>Chrome/Edge launch argument: <c>--window-position=X,Y</c></summary>
        public string WindowPositionArg => $"--window-position={X},{Y}";
    }
}