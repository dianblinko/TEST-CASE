using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Extensions.Logging;
using TestCase.Models;
using TestCase.ViewModels;

namespace TestCase.Services;

/// <summary>
/// Сервис работы со сводной таблицей.
/// </summary>
public class TableDataService
{
    private readonly ILogger _logger;
    public ObservableCollection<UnitedTableRowViewModel> TableRows { get; } = [];

    /// <summary>
    /// Коллекция автомобилей.
    /// </summary>
    public ObservableCollection<Record> Records { get; } = [];

    /// <summary>
    /// Обработчик события обновления коллекции автомобилей.
    /// </summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Аргументы события.</param>
    private void CarCollectionOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems == null)
        {
            return;
        }

        foreach (var newItem in e.NewItems.OfType<Record>())
        {
            if (TableRows.Any())
            {
                var lastRow = TableRows.Last();
                if (Math.Abs(lastRow.DateTime.Subtract(newItem.Time).TotalMilliseconds) < 100)
                {
                    switch (newItem)
                    {
                        case Car:
                            lastRow.Car = newItem.Value;
                            break;
                        case Driver:
                            lastRow.Driver = newItem.Value;
                            break;
                    }
                    _logger.LogInformation(string.Format("Водитель {0} и автомобиль {1} совпали по метке времени",
                        lastRow.Driver, lastRow.Car));
                    continue;
                }
            }

            TableRows.Add(new UnitedTableRowViewModel
            {
                DateTime = newItem.Time,
                Car = newItem is Car ? newItem.Value : string.Empty,
                Driver = newItem is Driver ? newItem.Value : string.Empty,
            });
        }
    }
    
    public TableDataService(ILogger logger)
    {
        _logger = logger;
        WeakEventManager<ObservableCollection<Record>, NotifyCollectionChangedEventArgs>
            .AddHandler(Records, nameof(Records.CollectionChanged), CarCollectionOnCollectionChanged);
    }
}