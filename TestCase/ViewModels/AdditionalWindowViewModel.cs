using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCase.Data;
using TestCase.Infrasctructure;
using TestCase.Models;
using TestCase.Services;
using TestCase.Views;

namespace TestCase.ViewModels;

/// <summary>
/// Модель представления дополнительного окна.
/// </summary>
public class AdditionalWindowViewModel : BindableBase
{
    /// <summary>
    /// Сервис работы с БД.
    /// </summary>
    private readonly IDataService _dataService;

    /// <summary>
    /// Сервис работы со сводной таблицей данных.
    /// </summary>
    private readonly TableDataService _tableDataService;

    /// <summary>
    /// Объект синхронизации потоков.
    /// </summary>
    private object _locker = new();

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Данные для таблицы.
    /// </summary>
    public ObservableCollection<UnitedTableRowViewModel> TableRows { get; } = [];

    /// <summary>
    /// Достать объекты из БД в таблицу.
    /// </summary>
    private void GetObjectFromDb()
    {
        var records = _dataService.GetAll();
        foreach (var record in records)
        {
            if (TableRows.Any())
            {
                var lastRow = TableRows.Last();
                if (Math.Abs(lastRow.DateTime.Subtract(record.Time).TotalMilliseconds) < 100)
                {
                    switch (record)
                    {
                        case Car:
                            lastRow.Car = record.Value;
                            break;
                        case Driver:
                            lastRow.Driver = record.Value;
                            break;
                    }
                    continue;
                }
            }

            TableRows.Add(new UnitedTableRowViewModel
            {
                DateTime = record.Time,
                Car = record is Car ? record.Value : string.Empty,
                Driver = record is Driver ? record.Value : string.Empty,
            });
        }
    }

    /// <inheritdoc cref="AdditionalWindowViewModel"/>
    public AdditionalWindowViewModel(IDataService dataService, ILogger logger)
    {
        _logger = logger;
        _logger.LogInformation("Открыто дополнительное окно");
        _dataService = dataService;
        _tableDataService = App.Host.Services.GetRequiredService<TableDataService>();

        lock (_locker)
        {
            WeakEventManager<ObservableCollection<UnitedTableRowViewModel>, NotifyCollectionChangedEventArgs>
                .AddHandler(_tableDataService.TableRows, nameof(_tableDataService.TableRows.CollectionChanged), (s, args) =>
                {
                    lock (_locker)
                    {
                        foreach (var argsNewItem in args.NewItems.OfType<UnitedTableRowViewModel>())
                        {
                            TableRows.Add(argsNewItem);
                        }
                    }
                });

            GetObjectFromDb();
        }
    }
}