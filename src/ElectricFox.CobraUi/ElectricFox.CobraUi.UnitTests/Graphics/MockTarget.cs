using ElectricFox.CobraUi.Display;
using SixLabors.ImageSharp;

namespace ElectricFox.CobraUi.UnitTests.Graphics
{
    internal class MockTarget : IPartialUpdateTarget, IScanlineTarget
    {
        private readonly int _width;
        private readonly int _height;

        private bool _isStarted;
        private bool isCompleted;

        private readonly List<byte[]> _scanlines = [];
        private readonly string _expectedHash;

        public MockTarget(int width, int height, string expectedHash)
        {
            _width = width;
            _height = height;
            _expectedHash = expectedHash;
        }

        public int Width => _width;

        public int Height => _height;

        public void BeginRegion(Rectangle region)
        {
            _isStarted = true;
        }

        public void EndRegion()
        {
            _isStarted = false;
            isCompleted = true;
        }

        public void WriteScanline(int y, ReadOnlySpan<byte> rgb565)
        {
            if (!_isStarted)
            {
                throw new InvalidOperationException("Region not started.");
            }

            byte[] line = new byte[rgb565.Length];
            rgb565.CopyTo(line);
            _scanlines.Add(line);
        }

        public void Verify()
        {
            using (Assert.EnterMultipleScope())
            {
                Convert.ToBase64String(_scanlines[0]);

                Assert.That(isCompleted, Is.True, "Region was not completed.");
                
                var hash = ComputeMd5Hash(_scanlines);

                Assert.That(hash, Is.EqualTo(_expectedHash), "Scanline data does not match expected hash.");
            }
        }

        public void VerifyNotCalled()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(isCompleted, Is.False, "Region completed when not expected");
                Assert.That(_scanlines.Count, Is.Zero, $"Found {_scanlines.Count} scanlines when none expected");
            }
        }

        public void BeginFrame()
        {
            throw new InvalidOperationException("BeginFrame should not be called on a partial target");
        }

        public void EndFrame()
        {
            throw new InvalidOperationException("EndFrame should not be called on a partial target");
        }

        private static string ComputeMd5Hash(List<byte[]> scanlines)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();

            foreach (var line in scanlines)
            {
                md5.TransformBlock(line, 0, line.Length, null, 0);
            }

            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return Convert.ToBase64String(md5.Hash!);
        }
    }
}
