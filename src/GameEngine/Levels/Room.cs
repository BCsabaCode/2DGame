using System.Diagnostics;

namespace GameEngine.Levels
{
    public enum BranchDir
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public enum State
    {
        None,
        Open,
        Closed,
        Allocated
    }

    class Room
    {
        static int next = 0;
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public int CenterX { get; }
        public int CenterY { get; }
        public Room Parent { get; }

        State north = State.None, east = State.None, south = State.None, west = State.None;
        Hallway northHallway, eastHallway, southHallway, westHallway;
        public Room(int x, int y, int width, int height, Room parent = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Id = next;
            next++;
            Parent = parent;
            CenterX = x + width / 2;
            CenterY = y + height / 2;
        }

        public int GetSize()
        {
            return Width * Height;
        }

        public bool IsInside(Room other)
        {
            int xTL = X - 2;
            int yTL = Y - 2;
            int xBR = X + Width + 2;
            int yBR = Y + Height + 2;

            int oXBR = other.X + other.Width;
            int oYBR = other.Y + other.Height;

            if (xTL > oXBR || yTL > oYBR) return false;

            if (xBR < other.X || yBR < other.Y) return false;

            return true;
        }

        public bool IsOpen(BranchDir dir)
        {
            switch (dir)
            {
                case BranchDir.North:
                    if (north == State.Open) return true; else return false;
                case BranchDir.East:
                    if (east == State.Open) return true; else return false;
                case BranchDir.South:
                    if (south == State.Open) return true; else return false;
                case BranchDir.West:
                    if (west == State.Open) return true; else return false;
                default:
                    return false;
            }
        }

        public Hallway GetHallway(BranchDir dir) {
            switch (dir)
            {
                case BranchDir.North:
                    if (north == State.Allocated) return northHallway;
                    return null;
                case BranchDir.East:
                    if (east == State.Allocated) return eastHallway;
                    return null;
                case BranchDir.South:
                    if (south == State.Allocated) return southHallway;
                    return null;
                case BranchDir.West:
                    if (west == State.Allocated) return westHallway;
                    return null;
                default:
                    return null;
            }
        }

        public bool IsAllocated(BranchDir dir)
        {
            switch (dir)
            {
                case BranchDir.North:
                    if (north == State.Allocated) return true;
                    return false;
                case BranchDir.East:
                    if (east == State.Allocated) return true;
                    return false;
                case BranchDir.South:
                    if (south == State.Allocated) return true;
                    return false;
                case BranchDir.West:
                    if (west == State.Allocated) return true;
                    return false;
                default:
                    return false;
            }
        }

        public bool Allocate(BranchDir dir, Hallway hallway)
        {
            switch (dir)
            {
                case BranchDir.North:
                    if (north == State.Allocated) return false;
                    north = State.Allocated;
                    northHallway = hallway;
                    return true;
                case BranchDir.East:
                    if (east == State.Allocated) return false;
                    east = State.Allocated;
                    eastHallway = hallway;
                    return true;
                case BranchDir.South:
                    if (south == State.Allocated) return false;
                    south = State.Allocated;
                    southHallway = hallway;
                    return true;
                case BranchDir.West:
                    if (west == State.Allocated) return false;
                    west = State.Allocated;
                    westHallway = hallway;
                    return true;
                default:
                    return false;
            }
        }

        public void Close(BranchDir dir)
        {
            switch (dir)
            {
                case BranchDir.North:
                    if (north != State.Open)
                    {
                        Debug.WriteLine("Something wrong tried to close not open branch in room: " + Id);
                    }
                    else
                    {
                        north = State.Closed;
                    }
                    break;
                case BranchDir.East:
                    if (east != State.Open)
                    {
                        Debug.WriteLine("Something wrong tried to close not open branch in room: " + Id);
                    }
                    else
                    {
                        east = State.Closed;
                    }
                    break;
                case BranchDir.South:
                    if (south != State.Open)
                    {
                        Debug.WriteLine("Something wrong tried to close not open branch in room: " + Id);
                    }
                    else
                    {
                        south = State.Closed;
                    }
                    break;
                case BranchDir.West:
                    if (west != State.Open)
                    {
                        Debug.WriteLine("Something wrong tried to close not open branch in room: " + Id);
                    }
                    else
                    {
                        west = State.Closed;
                    }
                    break;
            }
        }

        public bool OpenBranch(int i)
        {
            BranchDir dir = (BranchDir)i;
            switch (dir)
            {
                case BranchDir.North:
                    if (north != State.None) return false;
                    north = State.Open;
                    return true;
                case BranchDir.East:
                    if (east != State.None) return false;
                    east = State.Open;
                    return true;
                case BranchDir.South:
                    if (south != State.None) return false;
                    south = State.Open;
                    return true;
                case BranchDir.West:
                    if (west != State.None) return false;
                    west = State.Open;
                    return true;
                default:
                    return false;
            }
        }

        public bool HasOpenBranch()
        {
            if (north == State.Open || east == State.Open || south == State.Open || west == State.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
