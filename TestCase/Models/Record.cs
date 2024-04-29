namespace TestCase.Models;

/// <summary>
/// Запись.
/// </summary>
public abstract class Record
{
    /// <summary>
    /// Время.
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Значение.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Запись.
    /// </summary>
    protected Record()
    {
    }

    /// <summary>
    /// Запись.
    /// </summary>
    /// <param name="time">Время создания.</param>
    /// <param name="value">Текст.</param>
    protected Record(DateTime time, string value)
    {
        Time = time;
        Value = value;
    }
}