namespace GameEngine.Levels
{
    public enum HallWayOrientation
    {
        Vertical,
        Horizontal
    }
    class Hallway
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public HallWayOrientation Orientation { get; private set; }

        public Hallway(int x, int y, int width, int height, HallWayOrientation orientation)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Orientation = orientation;
        }
    }
}
