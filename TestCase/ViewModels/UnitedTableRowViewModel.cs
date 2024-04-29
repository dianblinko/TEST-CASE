namespace TestCase.ViewModels;

/// <summary>
/// Вью модель строки сводной таблицы.
/// </summary>
public class UnitedTableRowViewModel : BindableBase
{
    public DateTime DateTime { get; set; }

    private string _driver = string.Empty;

    public string Driver
    {
        get => _driver;
        set => Set(ref _driver, value);
    }

    private string _car = string.Empty;
    public string Car 
    { 
        get => _car;
        set => Set(ref _car, value); 
    }
}