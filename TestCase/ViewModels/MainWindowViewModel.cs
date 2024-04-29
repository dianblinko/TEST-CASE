using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCase.Infrasctructure;
using TestCase.Services;
using TestCase.Views;

namespace TestCase.ViewModels;

/// <summary>
/// Модель представления главного окна.
/// </summary>
public class MainWindowViewModel : BindableBase
{
    /// <summary>
    /// Сервис работы с потоками.
    /// </summary>
    private readonly IThreadService _threadService;

    /// <summary>
    /// Данные для таблицы.
    /// </summary>
    public ObservableCollection<UnitedTableRowViewModel> TableRows { get; }

    /// <summary>
    /// Признак работы потока генерации автомобилей.
    /// </summary>
    public bool CarThreadIsWorking
    {
        get => _threadService.CarThreadIsWorking; 
        set => _threadService.CarThreadIsWorking = value;
    }
    
    /// <summary>
    /// Признак работы потока генерации водителей.
    /// </summary>
    public bool DriverThreadIsWorking
    {
        get => _threadService.DriverThreadIsWorking; 
        set => _threadService.DriverThreadIsWorking = value;
    }

    /// <summary>
    /// Запустить новое окно.
    /// </summary>
    private void StartNewWindow()
    {
        var window = new AdditionalWindow();
        window.Closing += (sender, args) =>
            App.Host.Services.GetRequiredService<ILogger>().LogInformation("Дополнительное окно закрыто.");
        window.Show();
    }
    
    /// <summary>
    /// Команда открытия нового окна.
    /// </summary>
    public DelegateCommand OpenWindowCommand { get; }

    /// <inheritdoc cref="MainWindowViewModel"/>
    public MainWindowViewModel(IThreadService threadService)
    {
        TableRows = App.Host.Services.GetRequiredService<TableDataService>().TableRows;
        _threadService = threadService;
        OnPropertyChanged(nameof(CarThreadIsWorking));
        OnPropertyChanged(nameof(DriverThreadIsWorking));
        OpenWindowCommand = new DelegateCommand(x => StartNewWindow());
    }
}