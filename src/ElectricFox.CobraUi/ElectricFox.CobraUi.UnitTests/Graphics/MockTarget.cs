using ElectricFox.CobraUi.Display;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ElectricFox.CobraUi.UnitTests.Graphics
{
    internal class MockTarget : IPartialUpdateTarget, IScanlineTarget
    {
        private readonly int _width;
        private readonly int _height;

        private bool _isStarted;
        private bool _isCompleted;

        private readonly List<Rgba32[]> _scanlines = [];
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
            if (_isStarted)
            {
                throw new InvalidOperationException("Region already started.");
            }

            if (_isCompleted)
            {
                throw new InvalidOperationException("Region already completed.");
            }

            _isStarted = true;
        }

        public void EndRegion()
        {
            _isStarted = false;
            _isCompleted = true;
        }

        public void WriteScanline(int y, ReadOnlySpan<Rgba32> pixelData)
        {
            if (!_isStarted)
            {
                throw new InvalidOperationException("Region not started.");
            }

            Rgba32[] line = new Rgba32[pixelData.Length];
            pixelData.CopyTo(line);
            _scanlines.Add(line);
        }

        public void Verify()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_isCompleted, Is.True, "Region was not completed.");
                
                var hash = ComputeMd5Hash(_scanlines);

                Assert.That(hash, Is.EqualTo(_expectedHash), "Scanline data does not match expected hash.");
            }
        }

        public void VerifyNotCalled()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(_isCompleted, Is.False, "Region completed when not expected");
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

        private static string ComputeMd5Hash(List<Rgba32[]> scanlines)
        {
            using var md5 = MD5.Create();

            foreach (var line in scanlines)
            {
                byte[] data = MemoryMarshal.AsBytes(line).ToArray();
                md5.TransformBlock(data, 0, line.Length, null, 0);
            }

            md5.TransformFinalBlock([], 0, 0);

            return Convert.ToBase64String(md5.Hash!);
        }
    }
}
