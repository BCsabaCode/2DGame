using GameEngine.Entities.Mobs;
using GameEngine.Graphics;
using System;
using System.Numerics;

using GameEngine.Entities.Spawners;
using GameEngine.Utilities;

namespace GameEngine.Entities.Projectiles
{
    class BasicProjectile : Projectile
    {
        static readonly int rateOfFire = 30;
        static readonly int particleLife = 30;
        static readonly int particleAmount = 10;
        static readonly int xOffsetSize = 8;
        static readonly int yOffsetSize = 8;
        static Sprite sprite = Sprite.GetSprite(Sprite.GetSpriteID("basic_projectile"));

        //static readonly HitBox hitBox = new HitBox(16, 16, 4, 0);

        readonly float sinA, cosA;

        public BasicProjectile(Vector2 origin,float z, double angle, Mob owner) : base(origin, z, angle, owner)
        {
            _range = 320;
            _speed = 3;
            _damage = 10;
            changeXY.X = (float)(_speed * Math.Cos(angle));
            changeXY.Y = (float)(_speed * Math.Sin(angle));
            sinA = (float)Math.Sin(angle + Math.PI / 2);
            cosA = (float)Math.Cos(angle + Math.PI / 2);
        }

        public static int GetRateOfFire()
        {
            return rateOfFire;
        }

        public override void Update()
        {
            Move();
        }

        protected override void Move()
        {
            if (level.TilePenetration(position +changeXY))
            {
                position += changeXY;
            }
            else
            {
                if (changeXY.X < 0 && changeXY.Y < 0)
                {
                    level.AddEntity(new ParticleSpawner(position.X, position.Y, _z, particleLife, particleAmount, "particle_normal"));
                } 
                else if (changeXY.X > 0 && changeXY.Y > 0)
                {
                    level.AddEntity(new ParticleSpawner(position.X, position.Y, _z, particleLife, particleAmount, "particle_normal"));
                }
                else if (changeXY.X < 0 && changeXY.Y > 0)
                {
                    level.AddEntity(new ParticleSpawner(position.X, position.Y, _z, particleLife, particleAmount, "particle_normal"));
                }
                else if (changeXY.X > 0 && changeXY.Y < 0)
                {
                    level.AddEntity(new ParticleSpawner(position.X, position.Y, _z, particleLife, particleAmount, "particle_normal"));
                }
                Remove();
            }
            if (Distance() > _range) Remove();
            if (EntityCollision(position.X, position.Y))
            {
                Remove();
                level.AddEntity(new ParticleSpawner(position.X, position.Y, _z, particleLife, particleAmount, "particle_red"));
            }
        }
        public override void Render(Screen screen)
        {
            Vector2 vec2 = Coordinate.VirtualZAxisReduction(position, _z);
            vec2.X -= xOffsetSize;
            vec2.Y -= yOffsetSize;
            Matrix4x4 matrix = new Matrix4x4(cosA, sinA, 0.0f, 0.0f,
                                             -sinA, cosA, 0.0f, 0.0f,
                                             0.0f, 0.0f, 1.0f, 0.0f,
                                             vec2.X - screen.GetOffset().X, vec2.Y - screen.GetOffset().Y, 0.0f, 1.0f);
            screen.RenderProjectile(vec2.X, vec2.Y, sprite, matrix, xOffsetSize, yOffsetSize);
        }
    }
}
