using System.Windows;

namespace TestCase.Services;

/// <summary>
/// Сервис работы с UI потоком.
/// </summary>
public class UiService : IMainThreadService
{
    /// <inheritdoc />
    public void BeginInvoke(Action a)
    {
        Application.Current.Dispatcher.BeginInvoke(a);
    }
}