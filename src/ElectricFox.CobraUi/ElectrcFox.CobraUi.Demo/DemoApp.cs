using ElectrcFox.CobraUi.Demo.Screens;
using ElectricFox.CobraUi;
using ElectricFox.CobraUi.Demo;
using ElectricFox.CobraUi.Display;
using ElectricFox.CobraUi.Graphics;
using ElectricFox.CobraUi.Touch;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectrcFox.CobraUi.Demo;

public sealed class DemoApp : IDisposable
{
    private readonly AppHost _appHost;

    private readonly IResourceProvider _resourceProvider;

    private readonly ILoggerFactory _loggerFactory;

    private readonly CancellationTokenSource _cts = new();

    private bool disposedValue;

    public DemoApp(
        IScanlineTarget target, 
        ILoggerFactory loggerFactory,
        ITouchController touchController)
    {
        _loggerFactory = loggerFactory;

        var size = new Size(target.Width, target.Height);

        var renderer = new GraphicsRenderer(target, loggerFactory.CreateLogger<GraphicsRenderer>());
        _resourceProvider = new ResourceManager();

        var appHostLogger = loggerFactory.CreateLogger<AppHost>();
        _appHost = new AppHost(renderer, touchController, size, _resourceProvider, appHostLogger);
    }

    public async Task StartAsync()
    {
        await _resourceProvider.LoadAsync();

        var screenManagerLogger = _loggerFactory.CreateLogger<ScreenManager>();
        var screenManager = new ScreenManager(_appHost, screenManagerLogger);

        _ = Task.Run(() => _appHost.RunAsync(_cts.Token));

        await screenManager.ShowAsync(new SplashScreen());

        while (!_cts.IsCancellationRequested)
        {
            var menuChoice = await screenManager.ShowAsync(new MainMenuScreen());
            switch (menuChoice)
            {
            }
        }
    }

    public async Task StopAsync()
    {
        await _cts.CancelAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _cts.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
