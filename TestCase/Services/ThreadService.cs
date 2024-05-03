using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCase.Data;
using TestCase.Models;

namespace TestCase.Services;
using System.Windows;

/// <inheritdoc />
public class ThreadService : IThreadService
{
    /// <summary>
    /// Сервис работы с БД.
    /// </summary>
    private static readonly IDataService DataService = App.Host.Services.GetRequiredService<IDataService>();
    
    /// <summary>
    /// Событие запуска генерации автомобиля.
    /// </summary>
    private static readonly AutoResetEvent CarThreadStep = new(false);
    
    /// <summary>
    /// Событие запуска генерации водителя.
    /// </summary>
    private static readonly AutoResetEvent DriverThreadStep = new(false);
    
    /// <summary>
    /// Коллекция автомобилей.
    /// </summary>
    private readonly string[] Cars = ["Мондео", "Крета", "Приус", "УАЗик", "Вольво", "Фокус", "Октавия", "Запорожец"];
    
    /// <summary>
    /// Коллекция водителей.
    /// </summary>
    private readonly string[] Drivers = ["Петр", "Василий", "Николай", "Марина", "Феодосий", "Карина", "Ильвина", "Андрей"];
    
    /// <summary>
    /// Генератор псевдослучайных чисел.
    /// </summary>
    private readonly Random Random = new Random();
    
    /// <summary>
    /// Таймер.
    /// </summary>
    private Timer _timer = new(TimerCallback, null, 0, 1000);

    /// <summary>
    /// Объект синхронизации доступа к коллекции записей. 
    /// </summary>
    private object _lockRecordCollectionObject = new object();

    /// <summary>
    /// Объект синхронизации доступа к свойсту _carThreadIsWorking. 
    /// </summary>
    private object _lockCarThreadIsWorkingObject = new object();

    /// <summary>
    /// Объект синхронизации доступа к свойсту _driverThreadIsWorking. 
    /// </summary>
    private readonly object _lockDriverThreadIsWorkingObject = new object();
    
    /// <summary>
    /// Счетчик таймера.
    /// </summary>
    private static int _timerCounter;

    /// <summary>
    /// Коллекция сгенерированных записей.
    /// </summary>
    private readonly ObservableCollection<Record> _generatedRecords;

    /// <summary>
    /// Поток генерации автомобилей.
    /// </summary>
    private Thread _generateCarThread;

    /// <summary>
    /// Поток генерации водителей.
    /// </summary>
    private Thread _generateDriverThread;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Токен отмены потока генерации автомобилей.
    /// </summary>
    private CancellationTokenSource _carThreadCancellationToken;

    /// <summary>
    /// Токен отмены потока генерации водителей.
    /// </summary>
    private CancellationTokenSource _driverThreadCancellationToken;

    /// <summary>
    /// Сервис для работы с основным потоков.
    /// </summary>
    private readonly IMainThreadService _mainThreadService;

    /// <summary>
    /// Метод обработки тика таймера.
    /// </summary>
    /// <param name="state"></param>
    private static void TimerCallback(object? state)
    {
        if (_timerCounter % 2 == 0)
        {
            CarThreadStep.Set();
        }

        if (_timerCounter % 3 == 0)
        {
            DriverThreadStep.Set();
        }

        if (_timerCounter == 6)
        {
            _timerCounter = 0;
        }

        _timerCounter++;
    }
    
    /// <summary>
    /// Потоковая функция генерации автомобилей.
    /// </summary>
    private void GenerateCarThreadFunction(object? obj)
    {
        try
        {
            _logger.LogInformation("Поток генерации автомобилей запущен.");
            if (obj is not CancellationToken token)
            {
                _logger.LogError("GenerateCarThreadFunction получила в качестве аргумента не CancellationToken");
                return;
            }

            var max = Cars.Length;
            CarThreadStep.Reset();
            while (!token.IsCancellationRequested)
            {
                var eventThatSignaledIndex = WaitHandle.WaitAny([CarThreadStep, token.WaitHandle,]);
                if (token.IsCancellationRequested || eventThatSignaledIndex == 1) break;
                var value = new Car(DateTime.Now, Cars[Random.Next(max)]);
                AddRecordToCollection(value);
                DataService.Create(value);
            }

            _logger.LogInformation("Поток генерации автомобилей остановлен.");
        }
        catch (ThreadInterruptedException threadInterruptedException)
        {
            _logger.LogWarning(threadInterruptedException, "Поток генерации автомобилей остановлен через прерывание");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Поток генерации автомобилей завершился с ошибкой");
        }
    }

    /// <summary>
    /// Потоковая функция генерации водителей.
    /// </summary>
    private void GenerateDriverThreadFunction(object? obj)
    {
        try
        {
            _logger.LogInformation("Поток генерации водителей запущен.");
            if (obj is not CancellationToken token)
            {
                _logger.LogError("GenerateDriverThreadFunction получила в качестве аргумента не CancellationToken");
                return;
            }

            var max = Drivers.Length;
            DriverThreadStep.Reset();
            while (!token.IsCancellationRequested)
            {
                var eventThatSignaledIndex = WaitHandle.WaitAny([DriverThreadStep, token.WaitHandle,]);
                if (token.IsCancellationRequested || eventThatSignaledIndex == 1) break;
                var value = new Driver(DateTime.Now, Drivers[Random.Next(max)]);
                AddRecordToCollection(value);
                DataService.Create(value);
            }

            _logger.LogInformation("Поток генерации водителей остановлен.");
        }
        catch (ThreadInterruptedException threadInterruptedException)
        {
            _logger.LogWarning(threadInterruptedException, "Поток генерации автомобилей остановлен через прерывание");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Поток генерации автомобилей завершился с ошибкой");
        }
    }

    /// <summary>
    /// Добавить запись к коллекции.
    /// </summary>
    /// <param name="value"></param>
    private void AddRecordToCollection(Record value)
    {
        _mainThreadService.BeginInvoke(delegate
        {
            lock (_lockRecordCollectionObject)
            {
                _generatedRecords.Add(value);
            }
        });
    }

    /// <summary>
    /// Прервать поток генерации автомобилей.
    /// </summary>
    private void StopCarThread()
    {
        if (_carThreadCancellationToken.IsCancellationRequested) return;
        lock (_lockCarThreadIsWorkingObject)
        {
            if (_carThreadCancellationToken.IsCancellationRequested) return;
            _carThreadCancellationToken.Cancel();
            _carThreadCancellationToken.Dispose();
        }
    }

    /// <summary>
    /// Прервать поток генерации водителей.
    /// </summary>
    private void StopDriverThread()
    {
        if (_driverThreadCancellationToken.IsCancellationRequested) return;
        lock (_lockDriverThreadIsWorkingObject)
        {
            if (_driverThreadCancellationToken.IsCancellationRequested) return;
            _driverThreadCancellationToken.Cancel();
            _driverThreadCancellationToken.Dispose();
        }
    }

    /// <summary>
    /// Запустить поток генерации автомобилей.
    /// </summary>
    private void StartCarThread()
    {
        if (!_carThreadCancellationToken?.IsCancellationRequested ?? false) return;
        lock (_lockCarThreadIsWorkingObject)
        {
            if (!_carThreadCancellationToken?.IsCancellationRequested ?? false) return;
        
            _carThreadCancellationToken = new CancellationTokenSource();
            _generateCarThread = new Thread(GenerateCarThreadFunction)
            {
                Name = "FirstThread",
            };
        
            _generateCarThread.Start(_carThreadCancellationToken.Token);
        }
    }

    /// <summary>
    /// Запустить поток генерации водителей.
    /// </summary>
    private void StartDriverThread()
    {
        if (!_driverThreadCancellationToken?.IsCancellationRequested ?? false) return;
        lock (_lockDriverThreadIsWorkingObject)
        {
            if (!_driverThreadCancellationToken?.IsCancellationRequested ?? false) return;

            _driverThreadCancellationToken = new CancellationTokenSource();
            _generateDriverThread = new Thread(GenerateDriverThreadFunction)
            {
                Name = "SecondThread",
            };

            _generateDriverThread.Start(_driverThreadCancellationToken.Token);
        }
    }

    /// <inheritdoc/>
    public bool CarThreadIsWorking
    {
        get => !_carThreadCancellationToken?.IsCancellationRequested ?? false; 
        set { if (value) StartCarThread(); else StopCarThread();}
    }
    
    /// <inheritdoc/>
    public bool DriverThreadIsWorking 
    { 
        get => !_driverThreadCancellationToken?.IsCancellationRequested ?? false;
        set { if (value) StartDriverThread(); else StopDriverThread();}
    }

    /// <inheritdoc/>
    public void AbortAllThread()
    {
        StopCarThread();
        StopDriverThread();

        if (_generateCarThread.Join(200))
        {
            _generateCarThread.Interrupt();
        }

        if (_generateDriverThread.Join(200))
        {
            _generateDriverThread.Interrupt();
        }
    }

    /// <summary>
    /// Сервис работы с потоками генерации записей.
    /// </summary>
    /// <param name="tableDataService">Сервис хранения сгенерированных записей.</param>
    /// <param name="logger">Логгер</param>
    public ThreadService(TableDataService tableDataService, ILogger logger, IMainThreadService mainThreadService)
    {
        _logger = logger;
        _mainThreadService = mainThreadService;
        lock (_lockRecordCollectionObject)
        {
            _generatedRecords = tableDataService.Records;
        }
        
        StartDriverThread();
        StartCarThread();
    }
}