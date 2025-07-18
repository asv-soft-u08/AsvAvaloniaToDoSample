using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics.Metrics;
using System.Reflection;
using Asv.Avalonia;
using Asv.Cfg;
using Asv.Common;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace AsvAvaloniaToDoSample;

public class App : Application, IContainerHost, IShellHost
{
    private readonly CompositionHost _container;
    private readonly Subject<IShell> _onShellLoaded = new();

    private IShell _shell;

    public App()
    {
        var conventions = new ConventionBuilder();
        var containerCfg = new ContainerConfiguration();

        if (Design.IsDesignMode)
            containerCfg
                .WithExport(NullContainerHost.Instance)
                .WithExport<IConfiguration>(new InMemoryConfiguration())
                .WithExport(NullLoggerFactory.Instance)
                .WithExport(NullAppPath.Instance)
                .WithExport(NullLogService.Instance)
                .WithExport(NullAppInfo.Instance)
                .WithExport<IMeterFactory>(new DefaultMeterFactory())
                .WithExport(TimeProvider.System)
                .WithExport<IDataTemplateHost>(this)
                .WithExport<IShellHost>(this)
                .WithDefaultConventions(conventions);
        else
            containerCfg
                .WithExport<IContainerHost>(this)
                .WithExport(AppHost.Instance.GetService<IConfiguration>())
                .WithExport(AppHost.Instance.GetService<ILoggerFactory>())
                .WithExport(AppHost.Instance.GetService<ILogService>())
                .WithExport(AppHost.Instance.GetService<IAppPath>())
                .WithExport(AppHost.Instance.GetService<IAppInfo>())
                .WithExport(AppHost.Instance.GetService<IMeterFactory>())
                .WithExport(TimeProvider.System)
                .WithExport<IDataTemplateHost>(this)
                .WithExport<IShellHost>(this)
                .WithDefaultConventions(conventions);

        containerCfg = containerCfg.WithAssemblies(DefaultAssemblies.Distinct());

        _container = containerCfg.CreateContainer();
        DataTemplates.Add(new CompositionViewLocator(_container));
    }

    private IEnumerable<Assembly> DefaultAssemblies
    {
        get
        {
            yield return GetType().Assembly;
            yield return typeof(AppHost).Assembly;
        }
    }

    public IExportInfo Source => SystemModule.Instance;

    public T GetExport<T>()
        where T : IExportable
    {
        return _container.GetExport<T>();
    }

    public T GetExport<T>(string contract)
        where T : IExportable
    {
        return _container.GetExport<T>(contract);
    }

    public bool TryGetExport<T>(string id, out T value)
        where T : IExportable
    {
        return _container.TryGetExport(id, out value);
    }

    public void SatisfyImports(object value)
    {
        _container.SatisfyImports(value);
    }

    public IShell Shell
    {
        get => _shell;
        private set
        {
            _shell = value;
            _onShellLoaded.OnNext(value);
        }
    }

    public Observable<IShell> OnShellLoaded => _onShellLoaded;

    public TopLevel TopLevel { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Design.IsDesignMode)
        {
            Shell = DesignTimeShellViewModel.Instance;
        }
        else if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Shell = _container.GetExport<IShell>(DesktopShellViewModel.ShellId);
            if (desktop.MainWindow is TopLevel topLevel) TopLevel = topLevel;
        }
        else if (Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            Shell = _container.GetExport<IShell>(MobileShellViewModel.ShellId);
            if (singleViewPlatform.MainView is TopLevel topLevel) TopLevel = topLevel;
        }
        else
        {
            throw new Exception("Unknown platform");
        }

        base.OnFrameworkInitializationCompleted();
#if DEBUG
        this.AttachDevTools();
#endif
    }
}