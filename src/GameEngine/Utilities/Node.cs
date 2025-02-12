using System;

namespace GameEngine.Utilities
{
    class Node : IEquatable<Node>, IComparable<Node>
    {
        public Node(Node parent, float x, float y, double g, double h)
        {
            Parent = parent;
            X = x;
            Y = y;
            G = g;
            H = h;
        }

        public double G { get; set; }
        public double H { get; set; }
        public Node Parent { get; }
        public float X { get; }
        public float Y { get; }

        //Compare by f value (g + h)
        public int CompareTo(Node other)
        {
            return (int)(G + H - (other.G + other.H));
        }

        //Compare by f value (g + h)
        public bool Equals(Node other)
        {
            if (other == null) return false;

            if ((G + H - (other.G + other.H)) == 0)
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
