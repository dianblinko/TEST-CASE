using TestCase.Models;

namespace TestCase.Data;

/// <summary>
/// Сервис работы с данными.
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Записать данные.
    /// </summary>
    /// <param name="data">Данные.</param>
    void Create(Record data);

    /// <summary>
    /// Прочитать всех водителей.
    /// </summary>
    /// <returns>Коллекция водителей.</returns>
    IEnumerable<Record> GetAll();
}