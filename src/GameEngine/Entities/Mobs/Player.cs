using System;

using GameEngine.Inputs;
using GameEngine.Graphics;
using GameEngine.Utilities;
using GameEngine.Entities.Projectiles;
using System.Numerics;
using Windows.Foundation;
using GameEngine.Levels;
using GameEngine.Entities.PickupAbles;
using System.Diagnostics;
using GameEngine.Sounds;

namespace GameEngine.Entities.Mobs
{
    class Player : Mob
    {
        static MobType type = MobType.PLAYER;
        static HitBox _hitBox = new HitBox(32, 64, 0, 0);
        static Rect renderBox = new Rect(0, 0, 32, 64);

        KeyBoard input;
        protected int fireRate;
        public int Ammo { get; private set; }

        protected Player(float x, float y) : base()
        {
            position.X = x;
            position.Y = y;
            fireRate = 0;
            Ammo = 100;
        }
        public Player(float x , float y, KeyBoard input) : base()
        {
            position.X = x;
            position.Y = y;
            this.input = input;
            fireRate = 0;
            Ammo = 100;
        }

        public override void Update()
        {
            //Debug.WriteLine(position);
            CheckHP();
            IsDead();
            int xChange = 0;
            int yChange = 0;
            if (fireRate > 0) fireRate--;
            if (input.up)
            {
                yChange--;
                xChange--;
            }
            if (input.down)
            {
                yChange++;
                xChange++;
            }
            if (input.left)
            {
                xChange--;
                yChange++;
            }
            if (input.right)
            {
                xChange++;
                yChange--;
            }

            prev_moving = moving;
            if (xChange == 0 && yChange == 0)
            {
                moving = false;
            }

            if (xChange != 0 || yChange != 0)
            {
                moving = true;
                attacking = false;
                Move(xChange, yChange);
            }
            prev_attacking = attacking;
            UpdateShooting();
            UpdateSprite();
        }

        void UpdateSprite()
        {
            if (previous_direction == direction && prev_moving == moving && prev_attacking == attacking)
            {
                return;
            }
            else
            {
                switch (direction)
                {
                    case Direction.East:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_east");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_east");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_east");
                            }
                            break;
                        }
                    case Direction.NorthEast:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_north_east");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_north_east");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_north_east");
                            }
                            break;
                        }
                    case Direction.North:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_north");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_north");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_north");
                            }
                            break;
                        }
                    case Direction.NorthWest:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_north_west");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_north_west");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_north_west");
                            }
                            break;
                        }
                    case Direction.West:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_west");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_west");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_west");
                            }
                            break;
                        }
                    case Direction.SouthWest:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_south_west");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_south_west");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_south_west");
                            }
                            break;
                        }
                    case Direction.South:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_south");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_south");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_south");
                            }
                            break;
                        }
                    case Direction.SouthEast:
                        {
                            if (moving)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pm_south_east");
                            }
                            else if (attacking)
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("pa_south_east");
                            }
                            else
                            {
                                sprite = AnimatedSprite.GetAnimatedSprite("p_south_east");
                            }
                            break;
                        }
                }
            }
        }

        public void Consume(PickupAble pickup_able)
        {
            switch (pickup_able.Type)
            {
                case PickupType.Arrows:
                    {
                        Ammo += pickup_able.Value;
                        break;
                    }
                case PickupType.HP_Potion:
                    {
                        currentHP += pickup_able.Value;
                        CheckHP();
                        break;
                    }
                case PickupType.Mana_Potion:
                    {
                        break;
                    }
            }
        }

        private void UpdateShooting()
        {
            if (Mouse.GetButton() == Mouse.Button.Left)
            {
                if (fireRate <= 0 && Ammo != 0 && !moving)
                {
                    Vector2 vec2 = Mouse.GetIsoCoordinate() - position;
                    double angle = Math.Atan2(vec2.Y, vec2.X);
                    Shoot(position + new Vector2(28, 28), angle);
                    angle += Math.PI;
                    if (angle < (Math.PI * 0.125) || angle > (Math.PI * 1.875)) direction = Direction.West;
                    else if (angle < (Math.PI * 0.375)) direction = Direction.NorthWest;
                    else if (angle < (Math.PI * 0.625)) direction = Direction.North;
                    else if (angle < (Math.PI * 0.875)) direction = Direction.NorthEast;
                    else if (angle < (Math.PI * 1.125)) direction = Direction.East;
                    else if (angle < (Math.PI * 1.375)) direction = Direction.SouthEast;
                    else if (angle < (Math.PI * 1.625)) direction = Direction.South;
                    else if (angle < (Math.PI * 1.875)) direction = Direction.SouthWest;
                    fireRate = BasicProjectile.GetRateOfFire();
                    Ammo--;
                    attacking = true;
                    Sound.PlaySound("shot.mp3");
                }
            }
            else
            {
                attacking = false;
            }    
        }

        public override void Render(Screen screen)
        {
            if (sprite == null) sprite = AnimatedSprite.GetAnimatedSprite("p_south_east");
            screen.RenderEntity(Coordinate.NormalToIso(position) / 2, renderBox, sprite.GetSprite());
        }

        public bool IsWithin(Vector2 coord)
        {
            float xTL = position.X - 40 * Map.tileSize;
            float yTL = position.Y - 40 * Map.tileSize;
            float xBR = position.X + 40 * Map.tileSize;
            float yBR = position.Y + 40 * Map.tileSize;

            float x = coord.X;
            float y = coord.Y;

            if (x > xTL && x < xBR && y > yTL && y < yBR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool IsHit(float x, float y)
        {
            return _hitBox.IsInside(x - position.X, y - position.Y);
        }

        public override MobType GetMobType()
        {
            return type;
        }
    }
}
