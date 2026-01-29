using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace ElectricFox.CobraUi.Display
{
    public interface IPartialUpdateTarget
    {
        void BeginRegion(Rectangle region);
        void WriteScanline(int y, ReadOnlySpan<Rgba32> data);
        void EndRegion();
    }
}
