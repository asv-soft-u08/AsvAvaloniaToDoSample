using Asv.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;

namespace AsvAvaloniaToDoSample;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = AppHost.CreateBuilder(args);

        builder
            .UseAvalonia(BuildAvaloniaApp)
            .UseLogToConsoleOnDebug()
            .UseLogToConsole()
            .SetLogLevel(LogLevel.Trace)
            .UseAppPath(opt => opt.WithRelativeFolder("data"))
            .UseAppInfo(opt => opt.FillFromAssembly(typeof(App).Assembly))
            .UseSoloRun(opt => opt.WithArgumentForwarding())
            .UseJsonUserConfig()
            .UseLogService();

        using var host = builder.Build();
        host.StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions { OverlayPopups = true }) // Windows
            .With(new X11PlatformOptions { OverlayPopups = true, UseDBusFilePicker = false }) // Unix/Linux
            .With(new AvaloniaNativePlatformOptions { OverlayPopups = true }) // Mac
            .LogToTrace()
            .UseR3();
    }
}