using System.IO;
using System.Text.Json;
using NLog;
using NUnit.Framework;

namespace WebTests;

[SetUpFixture]
public class GlobalSetUp
{
    [OneTimeSetUp]
    public void SetupLogging()
    {
        var config = new NLog.Config.LoggingConfiguration();

        var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "NUnitLogs.txt");
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = logFilePath,
            Layout = "${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
        };
        var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
        {
            Layout = "${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
        };

        config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
        LogManager.Configuration = config;

        // Optional initial log entry so the log file is not empty
        var logger = LogManager.GetCurrentClassLogger();
        logger.Info("Global logging configured for WebTests.");

        // Start collecting test execution data for the custom report
        TestReportCollector.StartSuite();
    }

    [OneTimeTearDown]
    public void GenerateExecutionReport()
    {
        Directory.CreateDirectory("TestReport");

        var report = TestReportCollector.BuildSuiteReport();

        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(Path.Combine("TestReport", "ExecutionReport.json"), json);
        File.WriteAllText(Path.Combine("TestReport", "ExecutionReport.html"),
            ReportHtmlGenerator.Generate(report));
    }
}