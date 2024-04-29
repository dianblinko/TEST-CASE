using Microsoft.Extensions.Logging;
using Moq;
using TestCase.Services;

namespace TestCase.Tests;

[TestFixture]
public class ThreadServiceTests
{
        private Mock<ILogger> _loggerMock;
        private TableDataService _tableDataService;
        private IThreadService _threadService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _tableDataService = new TableDataService(_loggerMock.Object);
            _threadService = new ThreadService(_tableDataService, _loggerMock.Object, new MockMainThreadService());
        }

        [TearDown]
        public void TearDown()
        {
            _threadService.AbortAllThread();
        }

        [Test]
        public void CarThreadIsWorking_Property_Should_StartAndStopThread()
        {
            // Act
            _threadService.CarThreadIsWorking = false;
            Thread.Sleep(200);

            // Assert
            Assert.That(_threadService.CarThreadIsWorking, Is.False);
            
            // Act
            _threadService.CarThreadIsWorking = true;
            Thread.Sleep(200);
            
            // Assert
            Assert.That(_threadService.CarThreadIsWorking, Is.True);
        }

        [Test]
        public void DriverThreadIsWorking_Property_Should_StartAndStopThread()
        {
            // Act
            _threadService.DriverThreadIsWorking = false;
            Thread.Sleep(200);

            // Assert
            Assert.That(_threadService.DriverThreadIsWorking, Is.False);
            
            // Act
            _threadService.DriverThreadIsWorking = true;
            Thread.Sleep(200);
            
            // Assert
            Assert.That(_threadService.DriverThreadIsWorking, Is.True);
        }

        [Test]
        public void CarThreadIsWorking_Should_GenerateRecords()
        {
            // Arrange
            var initialRecordsCount = _threadService.CarThreadIsWorking ? _tableDataService.Records.Count : 0;

            // Act
            Thread.Sleep(2100);
            _threadService.CarThreadIsWorking = false;

            // Assert
            Assert.That(_tableDataService.Records.Count, Is.GreaterThan(initialRecordsCount));
        }

        [Test]
        public void DriverThreadIsWorking_Should_GenerateRecords()
        {
            // Arrange
            var initialRecordsCount = _threadService.DriverThreadIsWorking ? _tableDataService.Records.Count : 0;

            // Act
            Thread.Sleep(3100);
            _threadService.DriverThreadIsWorking = false;

            // Assert
            Assert.That(_tableDataService.Records.Count, Is.GreaterThan(initialRecordsCount));
        }

        [Test]
        public void AbortAllThread_Should_StopAllThreads()
        {
            // Act
            _threadService.AbortAllThread();
            Thread.Sleep(100);

            // Assert
            Assert.That(_threadService.CarThreadIsWorking, Is.False);
            Assert.That(_threadService.DriverThreadIsWorking, Is.False);
        }
}