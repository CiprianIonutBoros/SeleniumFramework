using NUnit.Framework;

// ── Parallelism Configuration ────────────────────────────────────────
// Controls the maximum number of test worker threads NUnit will use.
// Each worker runs its own browser instance via [FixtureLifeCycle(InstancePerTestCase)].
// This value should match the slot count in WindowLayoutManager.Reset()
// and the NumberOfTestWorkers in ci.runsettings.
[assembly: LevelOfParallelism(3)]
