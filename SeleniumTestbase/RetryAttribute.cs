using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace SeleniumTestbase
{
    /// <summary>
    /// Custom NUnit attribute that retries a test method up to a specified number
    /// of attempts before reporting it as failed. Useful for flaky UI tests where
    /// transient timing or network issues cause intermittent failures.
    /// <example>
    /// <code>
    /// [Test]
    /// [RetryOnFailure(3)]
    /// public void FlakyTest() { ... }
    /// </code>
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RetryOnFailureAttribute(int maxAttempts = 2) : NUnitAttribute, IRepeatTest
    {
        /// <summary>
        /// Wraps the original test command in a <see cref="RetryCommand"/> that
        /// re-executes the test on failure up to <paramref name="maxAttempts"/> times.
        /// </summary>
        public TestCommand Wrap(TestCommand command)
        {
            return new RetryCommand(command, maxAttempts);
        }

        /// <summary>
        /// Internal command that executes the wrapped test in a retry loop.
        /// If the test passes or all attempts are exhausted, the final result is returned.
        /// </summary>
        private class RetryCommand(TestCommand innerCommand, int maxAttempts) : DelegatingTestCommand(innerCommand)
        {
            /// <summary>
            /// Executes the test up to <paramref name="maxAttempts"/> times.
            /// Stops early if the test passes or produces a non-failure result.
            /// </summary>
            public override TestResult Execute(TestExecutionContext context)
            {
                int attempt = 0;

                while (true)
                {
                    attempt++;

                    // Reset the result for each attempt so previous failures don't leak
                    context.CurrentResult = context.CurrentTest.MakeTestResult();

                    try
                    {
                        innerCommand.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        // Catch unhandled exceptions so the retry loop can continue
                        context.CurrentResult.RecordException(ex);
                    }

                    // If the test passed (or was inconclusive/skipped), stop retrying
                    if (context.CurrentResult.ResultState != ResultState.Failure
                        && context.CurrentResult.ResultState != ResultState.Error)
                    {
                        break;
                    }

                    // All attempts exhausted — return the last failure
                    if (attempt >= maxAttempts)
                    {
                        break;
                    }

                    TestContext.WriteLine($"[RETRY] Attempt {attempt} failed. Retrying...");
                }

                return context.CurrentResult;
            }
        }
    }
}