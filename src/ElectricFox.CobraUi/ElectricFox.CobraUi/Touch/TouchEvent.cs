using SixLabors.ImageSharp;

namespace ElectricFox.CobraUi.Touch
{
    public readonly struct TouchEvent
    {
        public Point Point { get; }

        public TouchEvent(Point point)
        {
            Point = point;
        }
    }
}
