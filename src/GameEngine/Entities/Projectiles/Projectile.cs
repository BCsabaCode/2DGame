using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using GameEngine.Entities.Mobs;

namespace GameEngine.Entities.Projectiles
{
    abstract class Projectile : Entity
    {
		protected readonly Vector2 _origin;
		protected double _angle;
		protected Vector2 changeXY;
		protected double _speed, _range, _damage;
		protected Mob _owner;
		protected float _z;

		public Projectile(Vector2 origin,float z, double angle, Mob owner) : base()
		{
			position = origin;
			_owner = owner;
			_origin = origin;
			_angle = angle;
			_z = z;
		}

		protected bool EntityCollision(float x, float y)
		{
			bool collision = false;
			Mob mob;
			List<Mob> mobs = level.GetMobs();
			for (int i = 0; i < mobs.Count(); i++)
			{
				mob = mobs[i];
				if (mob.IsHit(x, y) && mob.GetMobType() != _owner.GetMobType())
				{
					if (mob.Damaged((int)_damage)) mob.SetKiller(_owner.GetMobType());
					collision = true;
				}
			}
			return collision;
		}

		protected double Distance()
		{
			return Math.Sqrt((position.X - _origin.X) * (position.X - _origin.X) + (position.Y - _origin.Y) * (position.Y - _origin.Y));
		}

		protected abstract void Move();
	}
}
