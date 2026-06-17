using ElectricFox.CobraUi.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.CobraUi.Demo.Screens
{
    public sealed class MainMenuScreen : Screen<MainMenuOption>
    {
        private readonly Button _temperatureMonitorButton;
        private readonly Button _pumpControlButton;
        private readonly Button _thermostatButton;

        public MainMenuScreen()
        {
            _temperatureMonitorButton = new Button("Temperature Monitor", ResourceManager.BdfFonts.Tamzen8x15b)
            {
                Position = new Point(10, 10),
                Width = 220
            };

            _temperatureMonitorButton.Clicked += (sender) =>
            {
                Complete(MainMenuOption.TemperatureMonitor);
            };

            _pumpControlButton = new Button("Pump Control", ResourceManager.BdfFonts.Tamzen8x15b)
            {
                Position = new Point(10, 60),
                Width = 220
            };

            _pumpControlButton.Clicked += (sender) =>
            {
                Complete(MainMenuOption.PumpControl);
            };

            _thermostatButton = new Button("Thermostat", ResourceManager.BdfFonts.Tamzen8x15b)
            {
                Position = new Point(10, 110),
                Width = 220
            };

            _thermostatButton.Clicked += (sender) =>
            {
                Complete(MainMenuOption.Thermostat);
            };
        }


        protected override void OnInitialize()
        {
            AddChild(_temperatureMonitorButton);
            AddChild(_pumpControlButton);
            AddChild(_thermostatButton);
        }
    }
}
