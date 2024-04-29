namespace TestCase.Models;

/// <summary>
/// Автомобиль.
/// </summary>
public class Car : Record
{
    /// <inheritdoc cref="Car"/>
    public Car()
    {
    }
    
    /// <inheritdoc cref="Car"/>
    /// <param name="time">Время создания.</param>
    /// <param name="value">Название.</param>
    public Car(DateTime time, string value) : base(time, value)
    {
    }
}