using System.IO;
using Microsoft.Extensions.Logging;

namespace TestCase.Services;

/// <summary>
/// Логгер в файл.
/// </summary>
public class FileLogger : ILogger
{
    /// <summary>
    /// Имя файла.
    /// </summary>
    private string fileName;
    
    /// <summary>
    /// Путь к файлу.
    /// </summary>
    private string fileDirectory;
    
    /// <summary>
    /// Объект синхронизации.
    /// </summary>
    private static object _lock = new object();

    /// <summary>
    /// Дата создания последнего файла.
    /// </summary>
    private DateTime _lastFileTime;

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
 
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }
 
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            lock (_lock)
            {
                if (DateTime.Now.Subtract(_lastFileTime).TotalMinutes >= 5)
                {
                    _lastFileTime = DateTime.Now;
                    fileName = $@"\log-{_lastFileTime:yy-MM-dd HH-mm}.log";
                }
                var n = Environment.NewLine;
                var exc = string.Empty;
                if (exception != null)
                {
                    exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                }

                File.AppendAllText(fileDirectory + fileName, logLevel + ": " + DateTime.Now + " " + formatter(state, exception) + n + exc);
            }
        }
    }
    
    /// <summary>
    /// Логгер в файл.
    /// </summary>
    public FileLogger()
    {
        fileDirectory = Environment.CurrentDirectory + "\\Logs";
        Directory.CreateDirectory(fileDirectory);
        _lastFileTime = DateTime.Now;
        fileName = $@"\log-{_lastFileTime:yy-MM-dd HH-mm}.log";
    }
    
    /// <summary>
    /// Логгер в файл.
    /// </summary>
    public FileLogger(string logDirectory)
    {
        fileDirectory = logDirectory;
        Directory.CreateDirectory(fileDirectory);
        _lastFileTime = DateTime.Now;
        fileName = $@"\log-{_lastFileTime:yy-MM-dd HH-mm}.log";
    }
}