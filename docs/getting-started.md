# Getting Started

## Planning your application

Before writing code, it helps to sketch the flow of your app. For this guide, we'll build a simple pool controller that shows temperatures, lets you switch pumps on and off, and provides a few basic status screens.

A simple flow might look like this:

- Splash Screen
- Main Menu
  - Temperature Monitor
  - Thermostat
  - Pump Control

Once you know which screens your app needs, you can start building them and wiring them together with `ScreenManager` and `AppHost`.

## Resources

Cobra UI includes built-in controls, but you supply the fonts and images your app uses. Resource loading is handled by `IResourceProvider`, which exposes three methods:

- `LoadAsync()` for pre-loading and caching resources at startup
- `GetFont()` for retrieving BDF fonts
- `GetImage()` for retrieving images

The application host calls `LoadAsync()` once during startup so resources are ready before the first screen is shown.

Resources are identified by string keys. You can define these however you like. In the demo app, fonts and images are embedded resources, and the keys are stored as constants on `ResourceManager` for convenience.

Resources don't have to come from embedded files — you could also load them from disk or another source if needed. See `ResourceManager` in the demo app for a simple example of loading and caching assets.

## Application host

`AppHost` is the core of the application runtime. It owns the render loop, forwards touch events to the current screen, and keeps the active screen updated.

A typical app host setup looks like this:

1. Create a renderer for your display target.
2. Create a touch controller.
3. Load resources.
4. Create a `ScreenManager`.
5. Start the app host render loop.
6. Show your first screen.

The host runs continuously until the app is stopped. On each frame it updates the active screen, renders the UI tree, and flushes the result to the display.

`ScreenManager` sits above the host and handles screen flow. It lets you push screens onto a stack, pop them back off, or show a screen and wait for a result. This makes it easy to build wizard-style flows, dialogs, and menu-driven navigation.

A minimal startup sequence might look like this:

```csharp
await resourceProvider.LoadAsync();

var renderer = new GraphicsRenderer(target, loggerFactory.CreateLogger<GraphicsRenderer>());
var appHost = new AppHost(renderer, touchController, screenSize, resourceProvider, loggerFactory.CreateLogger<AppHost>());
var screenManager = new ScreenManager(appHost, loggerFactory.CreateLogger<ScreenManager>());

_ = Task.Run(() => appHost.RunAsync(cts.Token));
await screenManager.ShowAsync(new SplashScreen());
```

## Building screens
A screen owns its controls and its state. When a screen is active, Cobra UI renders it to the display and forwards touch events to its control tree. A screen remains active until it completes, at which point control returns to ScreenManager so the application can decide what to show next.

To create a screen, subclass Screen<T>. The type parameter is the value returned when the screen completes. If the screen doesn't need to return anything, use Screen<object>.

Here's a simple splash screen with an image and a label:

```csharp
public sealed class SplashScreen : Screen<object>
```

Add the controls as private fields:

```csharp
private readonly Picture _picture;
private readonly Label _label;
```

Initialize them in the constructor. Each control references a resource key, and the actual font or image is resolved through `IResourceProvider` when the control is rendered.

```csharp
public SplashScreen()
{
    _picture = new Picture(ResourceManager.Images.CobraUi)
    {
        Position = new Point(85, 20)
    };

    _label = new Label(
        "Pool Controller Demo",
        ResourceManager.BdfFonts.Tamzen8x15b,
        80,
        180,
        Color.White);
}
```

Override `OnInitialize()` and add the controls to the screen's child tree. In this case, both controls are direct children of the screen. More complex layouts can use container controls to group and arrange child elements.

```csharp
protected override void OnInitialize()
{
    AddChild(_picture);
    AddChild(_label);
}
```

Override `OnTouch()` if you want the screen to respond to input. In this example, tapping anywhere on the screen completes it and returns control to the caller.

```csharp
public override bool OnTouch(TouchEvent e)
{
    Complete(null!);
    return true;
}
```