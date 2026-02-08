using ElectrcFox.CobraUi.Demo;
using ElectricFox.CobraUi.Display;
using ElectricFox.CobraUi.Touch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Setup logging
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

// SPI lock object locks SPI access between LCD and touch controller
var spiLock = new object();

// Initialize touch controller
var touchCal = new TouchCalibration(369, 3538, 332, 3900, true, true, false);
var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 17, touchCal, spiLock, loggerFactory.CreateLogger<Xpt2046>());
touch.Start();

// Initialize LCD
var lcd = new Ili9341(0, 0, spiLock);

// Create LCD scanline target
var target = new LcdScanlineTarget(lcd);

var app = new DemoApp(target, loggerFactory, touch);
await app.StartAsync();

// Keep the app running until a key is pressed
Console.WriteLine("Press any key to exit...");
Console.ReadKey();