using ElectricFox.CobraUi;
using ElectricFox.CobraUi.Demo;
using ElectricFox.CobraUi.Touch;
using ElectricFox.CobraUi.Ui;

namespace ElectrcFox.CobraUi.Demo.Screens
{
    public sealed class SplashScreen : Screen<object>
    {
        private readonly Picture _picture;
        private TimeSpan _elapsed = TimeSpan.Zero;
        private readonly TimeSpan _minimumDuration = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _maximumDuration = TimeSpan.FromSeconds(30);

        public SplashScreen()
        {
            _picture = new Picture(ResourceManager.Images.CobraUi)
            {
                Position = new SixLabors.ImageSharp.Point(85, 20)
            };
        }

        protected override void OnInitialize()
        {
            AddChild(_picture);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            _elapsed += delta;

            // Auto-complete after maximum duration
            if (_elapsed >= _maximumDuration)
            {
                Complete(null!);
            }
        }

        public override bool OnTouch(TouchEvent e)
        {
            // Allow user to skip after minimum duration
            if (_elapsed >= _minimumDuration)
            {
                Complete(null!);
                return true;
            }
            return base.OnTouch(e);
        }
    }
}
