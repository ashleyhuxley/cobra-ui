# Cobra UI

Cobra UI is a C# framework for building touch-driven applications for small SPI displays on embedded Linux devices (such as Raspberry Pi). It provides a screen-based UI model, touch input handling, and hardware abstractions for controllers like the ILI9341/ILI9488 and XPT2046, so you can focus on app behavior instead of low-level display plumbing.
## Cobra UI app architecture (overview)

A Cobra UI app is structured in layers so your app logic stays independent from display and touch hardware.

1. **Host layer** (`ConsoleHost`) wires up platform-specific input/output implementations (display + touch), logging, and starts the app. There is also a `WindowsHost` that can be used to debug and test the application in a desktop environment.
2. **Application host** (`AppHost`) runs the main update/render loop, forwards touch events, and renders the active screen.
3. **Screen flow** (`ScreenManager`) controls navigation with a stack of screens (`Push`, `Pop`, `ShowAsync`), so apps can model simple flows and modal results cleanly.
4. **UI composition** (`Screen` + `UiElement` tree) builds each screen from reusable controls (buttons, labels, pictures, charts, containers), with touch routed through the UI hierarchy.
5. **Rendering and resources** (`GraphicsRenderer` + `IResourceProvider`) draw the UI into a framebuffer and flush to the target display, while fonts/images are loaded through a resource provider.

In short: **hosts adapt hardware/platforms, `AppHost` drives the loop, screens define behavior, UI elements define layout, and the renderer/resource layer draws the final output.**

## Supported Hardware

### Display controllers
Cobra UI includes built-in support for two common SPI LCD controllers:

- **ILI9341** (typically **320×240**)
- **ILI9488** (typically **320×480**)

If your display uses a different controller, you can add support by implementing `ILcdDevice` and wiring it through a scanline target. The existing `Ili9341` and `Ili9488` implementations are good reference points.

### Touch controllers
Cobra UI currently supports the **XPT2046** SPI touch controller via `ITouchController`.

Additional touch devices can be added by implementing `ITouchController` and raising `TouchEvent` values for the app host.

Contributions for additional display or touch controller support are very welcome.