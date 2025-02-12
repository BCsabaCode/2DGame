using System.Numerics;

using GameEngine.Interfaces;
using GameEngine.Levels;
using GameEngine.Graphics;

namespace GameEngine.Entities
{
    abstract class Entity : IRenderable , IUpdateable
    {
        static long next = 0;
        public static bool GenID { get; set; }
        public long ID { get; protected set; }

        protected Vector2 position;
        protected Level level;
        bool removed = false;

        protected Entity()
        {
            if (GenID)
            {
                ID = next;
                next++;
            }
        }

        public void AddID(long id)
        {
            if (!GenID)
            {
                ID = id;
            }
        }

        public void Correct(float x, float y)
        {
            if (!GenID)
            {
                position.X = x;
                position.Y = y;
            }
        }

        public bool IsRemoved()
        {
            return removed;
        }

        public void Initalize(Level level)
        {
            this.level = level;
        }

        protected void Remove()
        {
            removed = true;
        }

        public float GetX()
        {
            return position.X;
        }

        public float GetY()
        {
            return position.Y;
        }

        public Vector2 GetXY()
        {
            return position;
        }

        public abstract void Render(Screen screen);
        public abstract void Update();

        public static int CompareByDistance(Entity x, Entity y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            } 
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int distX = (int)(x.position.X * x.position.X + x.position.Y * x.position.Y);
                    int distY = (int)(y.position.X * y.position.X + y.position.Y * y.position.Y);
                    if (distX == distY) return 0;
                    if (distX > distY)
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
    }
}
