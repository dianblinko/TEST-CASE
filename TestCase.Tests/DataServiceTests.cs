using Dapper;
using Microsoft.Data.Sqlite;
using TestCase.Data;
using TestCase.Models;

namespace TestCase.Tests;

[TestFixture]
public class DataServiceTests
{
    private readonly string TestDbFile = @"\Data\testDb.sqlite";

    [SetUp]
    public void Setup()
    {
        File.Copy(Environment.CurrentDirectory + @"\Data\testCase.sqlite", Environment.CurrentDirectory + TestDbFile, true);
    }

    [Test]
    public void Create_InsertsDataIntoDatabase()
    {
        // Arrange
        var dataService = new DataService(TestDbFile);
        var testData = new Car(DateTime.Now, "TestCar");

        // Act
        dataService.Create(testData);

        // Assert
        using var db = new SqliteConnection("Data Source=" + Environment.CurrentDirectory + TestDbFile);
        var cars = db.Query<Car>("SELECT DateTime as Time, Name as Value FROM Car WHERE Name = 'TestCar'").ToList();
        Assert.That(cars.Count(), Is.EqualTo(1));
        Assert.That(cars.First().Time, Is.EqualTo(testData.Time));
        Assert.That(cars.First().Value, Is.EqualTo(testData.Value));
        
        db.Close();
    }

    [Test]
    public void GetAll_ReturnsAllRecordsOrderedByTime()
    {
        // Arrange
        var dataService = new DataService(TestDbFile);
        var testData1 = new Car (DateTime.Now.AddHours(-1),"Car1");
        var testData2 = new Driver (DateTime.Now.AddHours(-2),"Driver1");
        dataService.Create(testData1);
        dataService.Create(testData2);

        // Act
        var result = dataService.GetAll();

        // Assert
        var resultList = result.ToList();
        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList[0].Time, Is.EqualTo(testData2.Time));
        Assert.That(resultList[0].Value, Is.EqualTo(testData2.Value));
        Assert.That(resultList[1].Time, Is.EqualTo(testData1.Time));
        Assert.That(resultList[1].Value, Is.EqualTo(testData1.Value));
    }
}