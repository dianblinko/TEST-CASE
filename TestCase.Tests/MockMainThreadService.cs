using TestCase.Services;

namespace TestCase.Tests;

/// <summary>
/// Заглушка для работы с основным потоком.
/// </summary>
public class MockMainThreadService : IMainThreadService
{
    public void BeginInvoke(Action a)
    {
        a?.Invoke();
    }
}