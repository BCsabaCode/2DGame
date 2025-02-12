using System.Numerics;
using Windows.Foundation;
using Windows.UI.Input;

using GameEngine.Utilities;

namespace GameEngine.Inputs
{
    class Mouse
    {
        static Vector2 _mouseXY = new Vector2(-1.0f, -1.0f);
        static Button _mouseButton = Button.None;
        static Vector2 _offset;
        public enum Button : int
        {
            None = 0,
            Left = 1,
            Middle = 2,
            Right = 3,
        }

        public void SetOffset(Vector2 offset)
        {
            _offset = offset;
        }

        public static float GetX()
        {
            return _mouseXY.X;
        }

        public static float GetY()
        {
            return _mouseXY.Y;
        }

        public static Vector2 GetMouseXY()
        {
            return _mouseXY;
        }

        public static Vector2 GetIsoCoordinate()
        {
            return _offset + Coordinate.IsoToNormal(_mouseXY);
        }

        public static Button GetButton()
        {
            return _mouseButton;
        }

        public void MouseMoved(Point position)
        {
            _mouseXY.X = (float)position.X;
            _mouseXY.Y = (float)position.Y;
        }

        public void MousePressed(PointerPoint pointer)
        {
            if (pointer.Properties.IsLeftButtonPressed) _mouseButton = Button.Left;
            if (pointer.Properties.IsRightButtonPressed) _mouseButton = Button.Right;
            if (pointer.Properties.IsMiddleButtonPressed) _mouseButton = Button.Middle;
        }

        public void MouseReleased()
        {
            _mouseButton = Button.None;
        }
    }
}
