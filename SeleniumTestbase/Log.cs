using NUnit.Framework;

namespace SeleniumTestbase
{
    /// <summary>
    /// Lightweight test logger that writes timestamped entries to the NUnit
    /// <see cref="TestContext"/> output. Entries are captured per-test and
    /// appear in test result reports and Visual Studio's Test Explorer output.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs an informational message (e.g. configuration, navigation events).
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            string entry = $"[INFO  {DateTime.Now:HH:mm:ss.fff}] {message}";
            TestContext.WriteLine(entry);
        }

        /// <summary>
        /// Logs a warning message (e.g. fallback behavior, non-critical issues).
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warn(string message)
        {
            string entry = $"[WARN  {DateTime.Now:HH:mm:ss.fff}] {message}";
            TestContext.WriteLine(entry);
        }

        /// <summary>
        /// Logs an error message (e.g. test failures, exceptions).
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            string entry = $"[ERROR {DateTime.Now:HH:mm:ss.fff}] {message}";
            TestContext.WriteLine(entry);
        }

        /// <summary>
        /// Logs a test step description, visually indented with ">>" for readability
        /// in test output. Use this to document meaningful user actions.
        /// </summary>
        /// <param name="description">A short description of the step being performed.</param>
        public static void Step(string description)
        {
            string entry = $"[STEP  {DateTime.Now:HH:mm:ss.fff}] >> {description}";
            TestContext.WriteLine(entry);
        }
    }
}