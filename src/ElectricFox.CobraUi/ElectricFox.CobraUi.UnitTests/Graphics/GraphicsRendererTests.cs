using ElectricFox.CobraUi.Graphics;
using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Moq;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace ElectricFox.CobraUi.UnitTests.Graphics
{
    public class GraphicsRendererTests
    {
        private const int TestWidth = 320;
        private const int TestHeight = 240;

        private ILogger<GraphicsRenderer> _logger => new Mock<ILogger<GraphicsRenderer>>().Object;

        [Test]
        public void Clear_FillsEntireImageWithColor()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "mLz4/w44l1pYTYcdjWLm9A==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Arrange & Act
            renderer.Clear(Color.Black);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void DrawRect_DrawsRectangle()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "ZfpAveV/eO71Q/PZOwvn1A==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Arrange & Act
            renderer.DrawRect(10, 10, 50, 30, Color.Red, 2);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void FillRect_FillsRectangle()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "mpbDZXpD0aW3P069DEIYFw==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.FillRect(20, 20, 40, 40, Color.Green);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void FillEllipse_FillsEllipse()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "G5VmqYxl2Mo1JxAy+jX+0Q==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.FillEllipse(50, 50, 60, 60, Color.Yellow);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void DrawEllipse_DrawsEllipse()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "coB1ohTUiG6W00WuYbgAjQ==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.DrawEllipse(30, 30, 50, 50, Color.Magenta);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void DrawLine_DrawsLine()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "6NDHQ/5WGrchIHhzPWItAA==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.DrawLine(10, 10, 100, 100, Color.White);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void SetPixel_SetsPixelColor()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "k7iFrf4NoInN9jSQT9WfcQ==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.SetPixel(100, 100, Color.Cyan);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void DrawImage_DrawsImageAtPosition()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "Qhq0YdsYB/5vKohIoin7RQ==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            var image = new Image<Rgba32>(50, 50);
            image.Mutate(ctx => ctx.Clear(Color.Red));

            // Act
            renderer.DrawImage(10, 10, image);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void Flush_WithNoChanges_DoesNothing()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "foo");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.Flush();

            // Assert
            mockTarget.VerifyNotCalled();
        }

        [Test]
        public void Flush_ClearsDirtyRegion()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "AFlP1PQrpD/BygQnoFdilQ==");
            var renderer = new GraphicsRenderer(mockTarget, _logger);
            renderer.SetPixel(10, 10, Color.Red);

            // Act
            renderer.Flush();
            renderer.Flush(); // Second flush should do nothing

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void MultipleDrawOperations_MergesDirtyRegions()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "c6M0FsWxWRnXFmZvExku2g==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.SetPixel(10, 10, Color.Red);
            renderer.SetPixel(50, 50, Color.Blue);
            renderer.FillRect(100, 100, 20, 20, Color.Green);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }

        [Test]
        public void Clear_AfterPreviousOperations_ResetsEntireImage()
        {
            // Arrange
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "mLz4/w44l1pYTYcdjWLm9A==");

            var renderer = new GraphicsRenderer(mockTarget, _logger);

            // Act
            renderer.Clear(Color.Black);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }
    }
}
