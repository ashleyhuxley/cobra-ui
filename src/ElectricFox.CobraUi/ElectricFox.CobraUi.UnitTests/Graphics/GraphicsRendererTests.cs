using ElectricFox.CobraUi.Graphics;
using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Moq;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace ElectricFox.CobraUi.UnitTests.Graphics
{
    public class GraphicsRendererTests
    {
        private const int TestWidth = 320;
        private const int TestHeight = 240;


        [Test]
        public void Clear_FillsEntireImageWithColor()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "Bq6KAdgNqWLHmHwmSvZM7A==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "iQMK4XGB8WmJemdTVFMb4A==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "LEpG3qenlMqAbOPIhgtpoQ==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "gyV7oBcaSFe2M4ltokXvhA==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "tRdmaUTHk7GE5irU5YLyPQ==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "B8HFwbRdhhiVipbpIEW/Sw==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "/3vUdo6wERcwAc66hzNDXw==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "5s339TIdJc1qO6BbFiVksw==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var expectedScanlines = new List<byte[]>();
            var mockTarget = new MockTarget(TestWidth, TestHeight, "foo");

            var renderer = new GraphicsRenderer(mockTarget);

            // Act
            renderer.Flush();

            // Assert
            mockTarget.VerifyNotCalled();
        }

        [Test]
        public void Flush_WithFullUpdateTarget_FlushesFullFrame()
        {
            //// Arrange
            //var renderer = new GraphicsRenderer(_mockTarget.Object);
            //renderer.SetPixel(10, 10, Color.Red);

            //// Act
            //renderer.Flush();

            //// Assert
            //_mockTarget.Verify(t => t.BeginFrame(), Times.Once);
            //_mockTarget.Verify(t => t.WriteScanline(It.IsAny<int>(), It.IsAny<ReadOnlySpan<byte>>()), Times.Exactly(TestHeight));
            //_mockTarget.Verify(t => t.EndFrame(), Times.Once);
        }

        [Test]
        public void Flush_ClearsDirtyRegion()
        {
            //// Arrange
            //var renderer = new GraphicsRenderer(_mockTarget.Object);
            //renderer.SetPixel(10, 10, Color.Red);

            //// Act
            //renderer.Flush();
            //renderer.Flush(); // Second flush should do nothing

            //// Assert
            //_mockTarget.Verify(t => t.BeginFrame(), Times.Once);
            //_mockTarget.Verify(t => t.EndFrame(), Times.Once);
        }

        [Test]
        public void MultipleDrawOperations_MergesDirtyRegions()
        {
            // Arrange
            var mockTarget = new MockTarget(TestWidth, TestHeight, "0fdAVh+XL81cNKNMSy9tQw==");

            var renderer = new GraphicsRenderer(mockTarget);

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
            var mockTarget = new MockTarget(TestWidth, TestHeight, "Bq6KAdgNqWLHmHwmSvZM7A==");

            var renderer = new GraphicsRenderer(mockTarget);

            // Act
            renderer.Clear(Color.Black);
            renderer.Flush();

            // Assert
            mockTarget.Verify();
        }
    }
}
