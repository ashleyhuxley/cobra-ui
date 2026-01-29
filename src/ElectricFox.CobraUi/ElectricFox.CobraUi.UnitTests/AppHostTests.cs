using ElectricFox.CobraUi.Graphics;
using ElectricFox.CobraUi.Touch;
using Microsoft.Extensions.Logging;
using Moq;
using SixLabors.ImageSharp;

namespace ElectricFox.CobraUi.UnitTests
{
    public class AppHostTests
    {
        private Mock<IGraphicsRenderer> _mockRenderer;
        private Mock<ITouchController> _mockTouchController;
        private Mock<IResourceProvider> _mockResources;
        private Mock<ILogger<AppHost>> _mockLogger;
        private Mock<Screen> _mockScreen;
        private Size _testSize;

        [SetUp]
        public void Setup()
        {
            _mockRenderer = new Mock<IGraphicsRenderer>();
            _mockTouchController = new Mock<ITouchController>();
            _mockResources = new Mock<IResourceProvider>();
            _mockLogger = new Mock<ILogger<AppHost>>();
            _mockScreen = new Mock<Screen>();
            _testSize = new Size(320, 240);
        }

        [Test]
        public void Constructor_InitializesWithCorrectScreenSize()
        {
            // Act
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Assert
            Assert.That(appHost.ScreenSize, Is.EqualTo(_testSize));
        }

        [Test]
        public void Constructor_SubscribesToTouchEvents()
        {
            // Act
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Assert
            _mockTouchController.VerifyAdd(t => t.TouchEventReceived += It.IsAny<Action<TouchEvent>>(), Times.Once);
        }

        [Test]
        public void SetScreen_CallsOnExitOnPreviousScreen()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            var previousScreen = new Mock<Screen>();
            appHost.SetScreen(previousScreen.Object);

            // Act
            appHost.SetScreen(_mockScreen.Object);

            // Assert
            previousScreen.Verify(s => s.OnExit(), Times.Once);
        }

        [Test]
        public void SetScreen_CallsOnEnterOnNewScreen()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Act
            appHost.SetScreen(_mockScreen.Object);

            // Assert
            _mockScreen.Verify(s => s.OnEnter(), Times.Once);
        }

        [Test]
        public void SetScreen_WithNull_DoesNotCallOnEnter()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            appHost.SetScreen(_mockScreen.Object);

            // Act
            appHost.SetScreen(null);

            // Assert
            _mockScreen.Verify(s => s.OnExit(), Times.Once);
            _mockScreen.Verify(s => s.OnEnter(), Times.Once); // Only from first SetScreen
        }

        [Test]
        public void SetScreen_WithNull_LogsWarning()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Act
            appHost.SetScreen(null);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("null")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task RunAsync_WithNoScreen_DoesNotCrash()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                try
                {
                    await appHost.RunAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected
                }
            });
        }

        [Test]
        public async Task RunAsync_HandlesExceptionsInRenderLoop()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            _mockScreen.Setup(s => s.Update(It.IsAny<TimeSpan>()))
                .Throws(new InvalidOperationException("Test exception"));

            appHost.SetScreen(_mockScreen.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            try
            {
                await appHost.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            // Assert - exception should be logged but not crash the loop
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task RunAsync_StopsWhenCancellationRequested()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(50);

            // Act & Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await appHost.RunAsync(cts.Token);
            });
        }

        [Test]
        public void TouchEvent_ForwardedToCurrentScreen()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            appHost.SetScreen(_mockScreen.Object);

            var touchEvent = new TouchEvent(new Point(100, 100));

            // Act - Raise the touch event
            _mockTouchController.Raise(t => t.TouchEventReceived += null, touchEvent);

            // Assert
            _mockScreen.Verify(s => s.OnTouch(It.Is<TouchEvent>(e => 
                e.Point.X == 100 && e.Point.Y == 100)), Times.Once);
        }

        [Test]
        public void TouchEvent_WithNoScreen_DoesNotCrash()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            var touchEvent = new TouchEvent(new Point(100, 100));

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _mockTouchController.Raise(t => t.TouchEventReceived += null, touchEvent);
            });
        }

        [Test]
        public void Render_WithNoScreen_DoesNotCallFlush()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Act - Use reflection to call private Render method
            var renderMethod = typeof(AppHost).GetMethod("Render", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            renderMethod?.Invoke(appHost, null);

            // Assert
            _mockRenderer.Verify(r => r.Flush(), Times.Never);
        }

        [Test]
        public void Update_WithScreen_CallsScreenUpdate()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            appHost.SetScreen(_mockScreen.Object);
            var delta = TimeSpan.FromMilliseconds(16);

            // Act - Use reflection to call private Update method
            var updateMethod = typeof(AppHost).GetMethod("Update",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateMethod?.Invoke(appHost, new object[] { delta });

            // Assert
            _mockScreen.Verify(s => s.Update(delta), Times.Once);
        }

        [Test]
        public void SetScreen_LogsScreenTransition()
        {
            // Arrange
            var appHost = new AppHost(
                _mockRenderer.Object,
                _mockTouchController.Object,
                _testSize,
                _mockResources.Object,
                _mockLogger.Object);

            // Act
            appHost.SetScreen(_mockScreen.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }
}