using GameEngine.Graphics;
using System;

using GameEngine.Utilities;
using System.Numerics;

namespace GameEngine.Entities.Particles
{
    class Particle : Entity
    {
        static Random random = new Random();
        float _z;
        double xChange, yChange, zChange;
        int _lifeTime;
        Sprite sprite;

        public Particle(float x, float y, float z, int lifeTime, string sprite_name) : base()
        {
            position.X = x;
            position.Y = y;
            _z = z;

            _lifeTime = random.Next(lifeTime);
            sprite = Sprite.GetSprite(Sprite.GetSpriteID(sprite_name));
            zChange = 0;

            xChange = NormalDistribution.NextGaussian();
            yChange = NormalDistribution.NextGaussian();
        }

        public override void Update()
        {
            _lifeTime--;
            if (_lifeTime < 0) Remove();

            zChange -= 0.1;
            if (_z < 0)
            {
                _z = 0;
                zChange *= -0.5;
                xChange *= 0.4;
                yChange *= 0.4;
            }

            Move();
        }

        void Move()
        {
            if (level.TileCollisionForParticles(position.X + xChange, position.Y))
            {
                xChange *= -1;
            }
            if (level.TileCollisionForParticles(position.X, position.Y + yChange))
            {
                yChange *= -1;
            }

            position.X += (float) xChange;
            position.Y += (float) yChange;
            _z += (float) zChange;
        }

        public override void Render(Screen screen)
        {
            Vector2 vec2 = Coordinate.VirtualZAxisReduction(position, _z);
            vec2.X -= sprite.GetWidth() / 2;
            vec2.Y -= sprite.GetHeight() / 2;
            screen.RenderParticle(vec2, sprite);
        }
    }
}
