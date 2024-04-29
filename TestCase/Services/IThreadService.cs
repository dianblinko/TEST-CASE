namespace TestCase.Services;

/// <summary>
/// Сервис работы с потоками генерации записей.
/// </summary>
public interface IThreadService
{
    /// <summary>
    /// Признак работы потока генерации автомобилей.
    /// </summary>
    bool CarThreadIsWorking { get; set; }

    /// <summary>
    /// Признак работы потока генерации водителей.
    /// </summary>
    bool DriverThreadIsWorking { get; set; }
    
    /// <summary>
    /// Завершить поток 
    /// </summary>
    void AbortAllThread();
}