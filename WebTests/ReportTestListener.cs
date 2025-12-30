using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebTests;

public class TestResultRecord
{
    public string Name { get; set; }
    public string Status { get; set; }
    public double Duration { get; set; }
    public string Timestamp { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
}

public class SuiteReport
{
    public string Timestamp { get; set; }
    public double TotalTimeSeconds { get; set; }
    public string OS { get; set; }
    public string DotNetVersion { get; set; }
    public List<TestResultRecord> TestResults { get; set; }
}

/// <summary>
/// Collects per-test information during the run and can build a final suite report.
/// </summary>
public static class TestReportCollector
{
    private static readonly object _lock = new();
    private static readonly List<TestResultRecord> _results = new();
    private static readonly Stopwatch _suiteTimer = new();
    private static bool _started;

    public static void StartSuite()
    {
        lock (_lock)
        {
            if (_started) return;
            _suiteTimer.Start();
            _started = true;
        }
    }

    /// <summary>
    /// Record the current test result. Optional customMessage can override the default message.
    /// </summary>
    public static void Record(string? customMessage = null)
    {
        var context = TestContext.CurrentContext;

        lock (_lock)
        {
            _results.Add(new TestResultRecord
            {
                Name = context.Test.Name,
                Status = context.Result.Outcome.Status.ToString(),
                // Approximate per-test duration using suite timer elapsed.
                Duration = Math.Round(_suiteTimer.Elapsed.TotalSeconds, 3),
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = string.IsNullOrWhiteSpace(customMessage)
                    ? context.Result.Message
                    : customMessage,
                StackTrace = context.Result.StackTrace
            });
        }
    }

    public static SuiteReport BuildSuiteReport()
    {
        lock (_lock)
        {
            if (_suiteTimer.IsRunning)
            {
                _suiteTimer.Stop();
            }

            return new SuiteReport
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalTimeSeconds = Math.Round(_suiteTimer.Elapsed.TotalSeconds, 3),
                OS = Environment.OSVersion.ToString(),
                DotNetVersion = Environment.Version.ToString(),
                TestResults = new List<TestResultRecord>(_results)
            };
        }
    }
}