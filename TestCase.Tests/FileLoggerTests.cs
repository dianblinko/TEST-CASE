using Microsoft.Extensions.Logging;
using TestCase.Services;

namespace TestCase.Tests;

[TestFixture]
public class FileLoggerTests
{
    private string TestLogsDirectory = Environment.CurrentDirectory + "\\TestLogs";

    [SetUp]
    public void Setup()
    {
        // Создаем временную директорию для логов
        Directory.CreateDirectory(TestLogsDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        // Удаляем временную директорию с логами
        Directory.Delete(TestLogsDirectory, true);
    }

    [Test]
    public void Log_WritesLogToFile()
    {
        // Arrange
        var logger = new FileLogger(TestLogsDirectory);
        var message = "Test log message";

        // Act
        logger.LogInformation(message);

        // Assert
        var logFilePaths = GetLogFilePath();
        Assert.That(logFilePaths.Count, Is.EqualTo(1));
        var logContents = File.ReadAllText(logFilePaths.First());
        Assert.That(logContents.Contains(message), Is.True);
    }

    [Test]
    public void Log_CreatesNewLogFileAfter5Minute()
    {
        // Arrange
        var logger = new FileLogger(TestLogsDirectory);
        var message = "Test log message";

        // Act
        logger.LogInformation(message);

        // Assert
        var filePaths = GetLogFilePath();
        Assert.That(filePaths.Count, Is.EqualTo(1));

        // Act
        Thread.Sleep((int)TimeSpan.FromMinutes(5).TotalMilliseconds + 10);
        logger.LogInformation("Test message after 1 minute");

        // Assert
        filePaths = GetLogFilePath();
        Assert.That(filePaths.Count, Is.EqualTo(2));
    }

    private IList<string> GetLogFilePath()
    {
        return Directory.GetFiles(TestLogsDirectory).Select(x => Path.Combine(TestLogsDirectory, x)).ToList();
    }
}
