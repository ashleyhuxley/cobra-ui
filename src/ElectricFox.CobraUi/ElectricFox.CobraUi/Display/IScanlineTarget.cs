using SixLabors.ImageSharp.PixelFormats;
using System;

namespace ElectricFox.CobraUi.Display
{
    public interface IScanlineTarget
    {
        int Width { get; }
        int Height { get; }

        void BeginFrame();
        void WriteScanline(int y, ReadOnlySpan<Rgba32> data);
        void EndFrame();
    }
}
