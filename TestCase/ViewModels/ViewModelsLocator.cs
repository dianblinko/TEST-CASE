using Microsoft.Extensions.DependencyInjection;

namespace TestCase.ViewModels;

/// <summary>
/// Локатор вью моделей.
/// </summary>
public class ViewModelsLocator
{
    /// <summary>
    /// Главная вьюмодель.
    /// </summary>
    public MainWindowViewModel MainWindowViewModel => App.Host.Services.GetRequiredService<MainWindowViewModel>();

    /// <summary>
    /// Вью модель дополнительного окна.
    /// </summary>
    public AdditionalWindowViewModel AdditionalWindowViewModel => App.Host.Services.GetRequiredService<AdditionalWindowViewModel>();
}