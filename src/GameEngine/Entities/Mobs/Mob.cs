using System.Numerics;

using GameEngine.Entities.Projectiles;
using GameEngine.Graphics;

namespace GameEngine.Entities.Mobs
{
    abstract class Mob : Entity
    {
		protected enum Direction
		{
			East,
			NorthEast,
			North,
			NorthWest,
			West,
			SouthWest,
			South,
			SouthEast
		}

		protected Direction direction = Direction.SouthEast;
		protected Direction previous_direction = Direction.SouthEast;
        protected int maxHP = 100;
        protected int currentHP = 100;

        protected bool moving = false;
		protected bool prev_moving = false;

		protected bool attacking = false;
		protected bool prev_attacking = false;

		protected AnimatedSprite sprite;

		MobType killer = MobType.NONE;

		protected Mob() : base()
		{

		}

        protected void Move(int xChange, int yChange)
        {
			previous_direction = direction;
			if (xChange == 0 && yChange > 0) direction = Direction.South;
			else if (xChange == 0 && yChange < 0) direction = Direction.North;
			else if (xChange > 0 && yChange == 0) direction = Direction.East;
			else if (xChange < 0 && yChange == 0) direction = Direction.West;
			else if (xChange < 0 && yChange < 0) direction = Direction.NorthWest;
			else if (xChange < 0 && yChange > 0) direction = Direction.SouthWest;
			else if (xChange > 0 && yChange < 0) direction = Direction.NorthEast;
			else if (xChange > 0 && yChange > 0) direction = Direction.SouthEast;

			if (!Collision(xChange, yChange))
			{
				position.X += xChange;
				position.Y += yChange;
			}
			else if (!Collision(0, yChange))
			{
				position.Y += yChange * 2;
			}
			else if (!Collision(xChange, 0))
			{
				position.X += xChange * 2;
			}
		}

		public int GetHP()
		{
			return currentHP;
		}

		public int GetMaxHP()
		{
			return maxHP;
		}

		public bool Damaged(int damage)
		{
			currentHP -= damage;
			if (currentHP <= 0) return true;
			return false;
		}

		public void SetKiller(MobType type)
		{
			if (killer == MobType.NONE) killer = type;
		}

		protected void Shoot(Vector2 projectileXY, double angle)
		{
			level.AddEntity(new BasicProjectile(projectileXY, 32, angle, this));
		}

		public abstract bool IsHit(float x, float y);

		public abstract MobType GetMobType();

		protected void CheckHP()
		{
			if (currentHP > maxHP) currentHP = maxHP;
		}

		public void IsDead()
		{
			if (currentHP <= 0) Remove();
		}

		bool Collision(int xChange, int yChange)
		{
			return level.TileCollision((int) position.X + xChange, (int) position.Y + yChange, 31, 31);
		}
    }
}
