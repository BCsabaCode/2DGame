using Windows.System;

namespace GameEngine.Inputs
{
    class KeyBoard
    {
        public bool up, down, left, right, reload;
        public static bool upArrow, downArrow, rightArrow, leftArrow, tab;
        public void Update(VirtualKey vk, bool value)
        {
            switch (vk)
            {
                case VirtualKey.W: up = value; break;
                case VirtualKey.S: down = value; break;
                case VirtualKey.A: left = value; break;
                case VirtualKey.D: right = value; break;
                case VirtualKey.F5: reload = value; break;
                case VirtualKey.Up: upArrow = value; break;
                case VirtualKey.Down: downArrow = value; break;
                case VirtualKey.Left: leftArrow = value; break;
                case VirtualKey.Right: rightArrow = value; break;
                case VirtualKey.Tab: tab = value; break;
            }
        }
    }
}
