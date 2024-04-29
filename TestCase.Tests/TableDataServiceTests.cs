using System.Collections.Specialized;
using Microsoft.Extensions.Logging;
using Moq;
using TestCase.Models;
using TestCase.Services;

namespace TestCase.Tests;

[TestFixture]
public class TableDataServiceTests
{
    private TableDataService _tableDataService;
    private Mock<ILogger> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _tableDataService = new TableDataService(_loggerMock.Object);
    }

    [Test]
    public void Records_AddsNewCar_WhenTableRowsIsEmpty()
    {
        // Arrange
        var newItem = new Car (DateTime.Now, "Test");

        // Act
        _tableDataService.Records.Add(newItem);

        // Assert
        Assert.That(_tableDataService.TableRows.Count, Is.EqualTo(1));
        var row = _tableDataService.TableRows.First();
        Assert.That(row.DateTime, Is.EqualTo(newItem.Time));
        Assert.That(row.Car, Is.EqualTo(newItem.Value));
        Assert.That(row.Driver, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Records_AddsNewDriver_WhenTableRowsIsEmpty()
    {
        // Arrange
        var newItem = new Driver(DateTime.Now, "Test");

        // Act
        _tableDataService.Records.Add(newItem);

        // Assert
        Assert.That(_tableDataService.TableRows.Count, Is.EqualTo(1));
        var row = _tableDataService.TableRows.First();
        Assert.That(row.DateTime, Is.EqualTo(newItem.Time));
        Assert.That(row.Driver, Is.EqualTo(newItem.Value));
        Assert.That(row.Car, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Records_AddsTwoRecord_WithEqualsTime()
    {
        // Arrange
        var newDriver = new Driver(DateTime.Now, "TestDriver");
        var newCar = new Car(DateTime.Now.AddMilliseconds(50), "TestCar");

        // Act
        _tableDataService.Records.Add(newDriver);
        _tableDataService.Records.Add(newCar);

        // Assert
        Assert.That(_tableDataService.TableRows.Count, Is.EqualTo(1));
        var row = _tableDataService.TableRows.First();
        Assert.That(row.DateTime, Is.EqualTo(newDriver.Time));
        Assert.That(row.Driver, Is.EqualTo(newDriver.Value));
        Assert.That(row.Car, Is.EqualTo(newCar.Value));
    }

    [Test]
    public void Records_AddsTwoRecord_WithNotEqualsTime()
    {
        // Arrange
        var newDriver = new Driver(DateTime.Now, "TestDriver");
        var newCar = new Car(DateTime.Now.AddMilliseconds(101), "TestCar");

        // Act
        _tableDataService.Records.Add(newDriver);
        _tableDataService.Records.Add(newCar);

        // Assert
        Assert.That(_tableDataService.TableRows.Count, Is.EqualTo(2));
        var firstRow = _tableDataService.TableRows.First();
        Assert.That(firstRow.DateTime, Is.EqualTo(newDriver.Time));
        Assert.That(firstRow.Driver, Is.EqualTo(newDriver.Value));
        Assert.That(firstRow.Car, Is.EqualTo(string.Empty));
        
        var secondRow = _tableDataService.TableRows.Last();
        Assert.That(secondRow.DateTime, Is.EqualTo(newCar.Time));
        Assert.That(secondRow.Car, Is.EqualTo(newCar.Value));
        Assert.That(secondRow.Driver, Is.EqualTo(string.Empty));
    }
}