using ElectricFox.CobraUi.Graphics;
using SixLabors.ImageSharp;
using System;

namespace ElectricFox.CobraUi.Ui
{
    public class Canvas : UiElement
    {
        public override Size Size => new(Width, Height);

        public event Action<IGraphicsRenderer, IResourceProvider>? Rendered;

        public int Width { get; }

        public int Height { get; }

        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
        }

        protected override void OnRender(IGraphicsRenderer renderer, IResourceProvider resourceProvider)
        {
            Rendered?.Invoke(renderer, resourceProvider);
        }
    }
}
