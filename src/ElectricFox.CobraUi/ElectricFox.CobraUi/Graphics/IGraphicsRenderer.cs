using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.CobraUi.Graphics
{
    public interface IGraphicsRenderer
    {
        void Flush();
        void Clear(Color color);
        void DrawText(string text, BdfFont font, int x, int y, Color color);
        void DrawGlyph(int i, BdfFont font, int x, int y, Color color);
        public void DrawImage(int x, int y, Image<Rgba32> image);
        void DrawRect(int x, int y, int w, int h, Color color, int thickness = 1);
        void FillRect(int x, int y, int w, int h, Color color);
        void FillEllipse(int x, int y, int w, int h, Color color);
        void DrawEllipse(int x, int y, int w, int h, Color color);
        void DrawLine(float x1, float y1, float x2, float y2, Color color);
        void SetPixel(int x, int y, Color color);
    }
}
