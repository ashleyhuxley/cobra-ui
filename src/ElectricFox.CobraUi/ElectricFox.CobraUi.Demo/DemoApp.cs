using ElectrcFox.CobraUi.Demo.Screens;
using ElectricFox.CobraUi;
using ElectricFox.CobraUi.Demo;
using ElectricFox.CobraUi.Demo.Screens;
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

        // The Scanline Target is the interface to the device that the application will be rendered to. This is provided by the container application.
        // In the desktop host, this is an Avalonia control that is rendered to the screen. In the embedded host, this is a driver that renders to the display.
        // The Scanline Target tells us the size of the screen.

        var size = new Size(target.Width, target.Height);

        // The Graphics Renderer is the rendering engine that will be used to render the application. It provides low level graphics primitives that are used by the UI framework to render the application.
        var renderer = new GraphicsRenderer(target, loggerFactory.CreateLogger<GraphicsRenderer>());

        // The Resource Provider is used to load resources such as images and fonts. It provides a way to load resources from the file system or embedded resources.
        _resourceProvider = new ResourceManager();

        var appHostLogger = loggerFactory.CreateLogger<AppHost>();

        // The App Host is the main entry point for the application. It is responsible for initializing the application, starting the render loop, and managing the application lifecycle.
        _appHost = new AppHost(renderer, touchController, size, _resourceProvider, appHostLogger);
    }

    public async Task StartAsync()
    {
        // First, we load the resources that will be used by the application. This includes images and fonts that are embedded in the assembly.
        await _resourceProvider.LoadAsync();

        var screenManagerLogger = _loggerFactory.CreateLogger<ScreenManager>();

        // The Screen Manager is responsible for managing the screens that are displayed in the application. It provides a way to show screens and manage the screen stack.
        var screenManager = new ScreenManager(_appHost, screenManagerLogger);

        // Start the render loop in a background task. This will run until the application is stopped.
        _ = Task.Run(() => _appHost.RunAsync(_cts.Token));

        // Give the render loop time to start
        await Task.Delay(100);

        // Show the splash screen. This will be displayed until the user touches the screen or the maximum duration is reached.
        await screenManager.ShowAsync(new SplashScreen());

        // This is the main application loop. This controls the flow of the application and will continue until the application is stopped.
        while (!_cts.IsCancellationRequested)
        {
            var menuChoice = await screenManager.ShowAsync(new MainMenuScreen());
            switch (menuChoice)
            {
                case MainMenuOption.TemperatureMonitor:
                    //await screenManager.ShowAsync(new TemperatureMonitorScreen());
                    break;
                case MainMenuOption.PumpControl:
                    //await screenManager.ShowAsync(new PumpControlScreen());
                    break;
                case MainMenuOption.Thermostat:
                    //await screenManager.ShowAsync(new ThermostatScreen());
                    break;
                default:
                    _cts.Cancel();
                    break;
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
