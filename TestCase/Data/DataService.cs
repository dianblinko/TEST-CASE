using Dapper;
using Microsoft.Data.Sqlite;
using TestCase.Models;

namespace TestCase.Data;

/// <inheritdoc />
public class DataService : IDataService
{
    /// <summary>
    /// Объект синхронизации.
    /// </summary>
    private readonly object _locker = new object();
    
    /// <summary>
    /// Путь БД.
    /// </summary>
    private static string DbFile = Environment.CurrentDirectory + @"\Data\testCase.sqlite";

    /// <summary>
    /// Получить соединение с БД.
    /// </summary>
    /// <returns>Соединение.</returns>
    private static SqliteConnection GetDbConnection()
    {
        return new SqliteConnection("Data Source=" + DbFile);
    }
    
    /// <inheritdoc />
    public void Create(Record data)
    {
        using (var db = GetDbConnection())
        {
            var table = data is Car ? nameof(Car) : nameof(Driver);
            var sqlQuery = $"INSERT INTO {table} (DateTime, Name) VALUES(@Time, @Value)";
            lock (_locker)
            {
                db.Execute(sqlQuery, data);
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<Record> GetAll()
    {
        using (var db = GetDbConnection())
        {
            List<Record> cars;
            List<Record> drivers;
            lock (_locker)
            {
                cars = db.Query<Car>("SELECT DateTime as Time, Name as Value FROM Car").OfType<Record>().ToList();
                drivers = db.Query<Driver>("SELECT DateTime as Time, Name as Value FROM Driver").OfType<Record>()
                    .ToList();
            }

            return cars.Concat(drivers).OrderBy(x => x.Time);
        }
    }

    public DataService()
    {
        
    }

    public DataService(string path)
    {
        DbFile = Environment.CurrentDirectory + path;
    }
}