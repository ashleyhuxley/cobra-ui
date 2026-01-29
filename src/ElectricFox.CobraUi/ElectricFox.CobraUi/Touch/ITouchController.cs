using System;

namespace ElectricFox.CobraUi.Touch
{
    public interface ITouchController
    {
        event Action<TouchEvent> TouchEventReceived;
        void Start();
    }
}
