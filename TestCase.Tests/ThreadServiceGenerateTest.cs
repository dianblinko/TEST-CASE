using Microsoft.Extensions.Logging;
using Moq;
using TestCase.Services;

namespace TestCase.Tests;

[TestFixture]
public class ThreadServiceGenerateTest
{
    private IThreadService _threadService;

    [TearDown]
    public void TearDown()
    {
        _threadService.AbortAllThread();
    }

    [Test]
    public void TwoThreadsIsWorking_Should_GenerateRecords()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var tableDataService = new TableDataService(loggerMock.Object);
        _threadService = new ThreadService(tableDataService, loggerMock.Object, new MockMainThreadService());

        // Act
        Thread.Sleep(3100);

        // Assert
        // После 3100 мс.
        Assert.That(tableDataService.Records.Count, Is.EqualTo(2));
        Assert.That(tableDataService.TableRows.Count, Is.EqualTo(2));
            
        // Act
        Thread.Sleep(1000);

        // Assert
        // После 4100 мс.
        Assert.That(tableDataService.Records.Count, Is.EqualTo(3));
        Assert.That(tableDataService.TableRows.Count, Is.EqualTo(3));
            
        // Act
        Thread.Sleep(2000);

        // Assert
        // После 6100 мс.
        Assert.That(tableDataService.Records.Count, Is.EqualTo(5));
        Assert.That(tableDataService.TableRows.Count, Is.EqualTo(4));
    }
    
}