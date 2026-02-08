using ElectricFox.CobraUi;
using ElectricFox.CobraUi.Demo;
using ElectricFox.CobraUi.Ui;

namespace ElectrcFox.CobraUi.Demo.Screens
{
    public sealed class SplashScreen : Screen<object>
    {
        private readonly Picture _picture;

        public SplashScreen()
        {
            _picture = new Picture(ResourceManager.Images.CobraUi)
            {
                Position = new SixLabors.ImageSharp.Point(10, 10)
            };
        }

        protected override void OnInitialize()
        {
            AddChild(_picture);
        }
    }
}
