using System;
using System.Numerics;

using GameEngine.Graphics;

namespace GameEngine.Levels
{
    public enum WallOrientation
    {
        Horizontal,
        Vertical,
        HorizontalTop,
        VerticalTop
    }
    class Wall : IEquatable<Wall> , IComparable<Wall>
    {
        int Z;
        int[] Data;
        int Length;
        WallOrientation Orientation;

        public float X { get; private set; }
        public float Y { get; private set; }

        public Wall(float x, float y, int z, int length, int[] wall, WallOrientation orientation)
        {
            X = x;
            Y = y;
            Z = z;
            Length = length;
            Data = wall;
            Orientation = orientation;
        }

        public void AddSectorOffset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public void Render(Vector2 playerXY, Screen screen)
        {
            Vector2 offset = screen.GetOffset();
            if (Orientation == WallOrientation.Horizontal)
            {
                float opacity;
                screen.SetRenderMode(RenderMode.HorizontalIso);
                for (int i = 0; i < Length; i++)
                {
                    if (Data[i] == 0) continue;
                    opacity = 1.0f;
                    float isoX = X + i;
                    float isoY = Y - 1;
                    if (playerXY.X / Map.tileSize > (isoX - 3) && playerXY.X / Map.tileSize < (isoX + 3) && playerXY.Y / Map.tileSize > (isoY - 3) && playerXY.Y / Map.tileSize < isoY) opacity = 0.5f;
                    screen.RenderRectangle((i + X - Y) * Map.tileSize + offset.Y, (-Y + Z) * Map.tileSize + 2 * offset.Y, Map.tileSize, Sprite.GetSprite(Data[i]).GetBitmap(), opacity);
                }
            }
            else if (Orientation == WallOrientation.Vertical)
            {
                float opacity;
                screen.SetRenderMode(RenderMode.VerticalIso);
                for (int i = 0; i < Length; i++)
                {
                    if (Data[i] == 0) continue;
                    opacity = 1.0f;
                    float isoX = X - 1;
                    float isoY = Y + i;
                    if (playerXY.X / Map.tileSize > (isoX - 3) && playerXY.X / Map.tileSize < isoX && playerXY.Y / Map.tileSize > (isoY - 3) && playerXY.Y / Map.tileSize < (isoY + 3)) opacity = 0.5f;
                    screen.RenderRectangleSpecialBounds((-X + Z) * Map.tileSize + 2 * offset.X, (Y + i - X) * Map.tileSize + offset.X, Map.tileSize, Sprite.GetSprite(Data[i]).GetBitmap(), opacity);
                }
            } 
            else if (Orientation == WallOrientation.HorizontalTop)
            {
                float opacity;
                screen.SetRenderMode(RenderMode.Isometric);
                for (int i = 0; i < Length; i++)
                {
                    if (Data[i] == 0) continue;
                    opacity = 1.0f;
                    float isoX = X + i;
                    float isoY = Y - 1;
                    if (playerXY.X / Map.tileSize > (isoX - 3) && playerXY.X / Map.tileSize < (isoX + 3) && playerXY.Y / Map.tileSize > (isoY - 3) && playerXY.Y / Map.tileSize < isoY) opacity = 0.5f;
                    screen.RenderRectangle((i + X - Z) * Map.tileSize, (Y - Z) * Map.tileSize, Map.tileSize, Sprite.GetSprite(Data[i]).GetBitmap(), opacity);
                }
            }
            else if (Orientation == WallOrientation.VerticalTop)
            {
                float opacity;
                screen.SetRenderMode(RenderMode.Isometric);
                for (int i = 0; i < Length; i++)
                {
                    if (Data[i] == 0) continue;
                    opacity = 1.0f;
                    float isoX = X - 1;
                    float isoY = Y + i;
                    if (playerXY.X / Map.tileSize > (isoX - 3) && playerXY.X / Map.tileSize < isoX && playerXY.Y / Map.tileSize > (isoY - 3) && playerXY.Y / Map.tileSize < (isoY + 3)) opacity = 0.5f;
                    screen.RenderRectangle((X - Z) * Map.tileSize, (Y + i - Z) * Map.tileSize, Map.tileSize, Sprite.GetSprite(Data[i]).GetBitmap(), opacity);
                }
            }
        }

        public bool IsBehind(float x, float y)
        {
            if (Orientation == WallOrientation.Horizontal)
            {
                if ((Y * Map.tileSize) > y)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if ((X * Map.tileSize) > x)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(Wall other)
        {
            if (other == null) return false;
            return other.X == X && other.Y == Y;
        }

        public int CompareTo(Wall other)
        {
            if (other == null) return 1;
            
            if (Z == other.Z)
            {
                int dist = (int)(X * X + Y * Y);
                int distOther = (int)(other.X * other.X + other.Y * other.Y);

                if (dist == distOther) return 0;
                if (dist > distOther)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            if (Z > other.Z)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
