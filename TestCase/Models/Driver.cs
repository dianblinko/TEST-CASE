namespace TestCase.Models;

/// <summary>
/// Водитель.
/// </summary>
public class Driver : Record
{
    /// <inheritdoc cref="Driver"/>
    public Driver()
    {

    }

    /// <inheritdoc cref="Driver"/>
    /// <param name="time">Время создания.</param>
    /// <param name="value">Название.</param>
    public Driver(DateTime time, string value) : base(time, value)
    {
    }
}