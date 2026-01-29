using ElectricFox.CobraUi.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.CobraUi.Ui
{
    public class Label : UiElement
    {
        private Size _size;

        private readonly string _text;

        public Color Color { get; set; }
        public string Font { get; set; }

        public override Size Size => _size;

        public Label(string text, string font, int x, int y, Color color)
        {
            _text = text;
            Font = font;
            Position = new Point(x, y);
            Color = color;
        }

        protected override void OnRender(IGraphicsRenderer renderer, IResourceProvider resourceProvider)
        {
            var bdfFont = resourceProvider.GetFont(Font);

            var rect = bdfFont.MeasureString(_text);
            _size = new Size(rect.Width, rect.Height);

            renderer.DrawText(_text, bdfFont, AbsolutePosition.X, AbsolutePosition.Y, Color);
        }
    }
}
