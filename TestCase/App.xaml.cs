using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCase.Data;
using TestCase.Services;
using TestCase.ViewModels;

namespace TestCase;

public partial class App
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        var host = Host;
        base.OnStartup(e);
        await host.StartAsync().ConfigureAwait(false);
        Host.Services.GetRequiredService<ILogger>().LogInformation("Приложение запущено");
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        Host.Services.GetRequiredService<IThreadService>().AbortAllThread();
        Host.Services.GetRequiredService<ILogger>().LogInformation("Приложение остановлено");
        var host = Host;
        await host.StopAsync().ConfigureAwait(false);
        host.Dispose();
        _host = null;
        base.OnExit(e);
    }

    private static IHost? _host;

    public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        services.AddSingleton<IMainThreadService, UiService>();
        services.AddSingleton<ILogger, FileLogger>();
        services.AddSingleton<TableDataService>();
        services.AddSingleton<IThreadService, ThreadService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<AdditionalWindowViewModel>();
        services.AddSingleton<IDataService, DataService>();
    }
}