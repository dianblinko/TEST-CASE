namespace TestCase.Services;

/// <summary>
/// Сервис работы с основным потоком.
/// </summary>
public interface IMainThreadService
{
    /// <summary>
    /// Вызывать событие в основном потоке.
    /// </summary>
    /// <param name="a">Событие.</param>
    public void BeginInvoke(Action a);
}