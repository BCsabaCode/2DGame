using GameEngine.Entities.Mobs;
using GameEngine.Graphics;
using GameEngine.Levels;
using Windows.Foundation;

namespace GameEngine.Entities.PickupAbles
{
    abstract class PickupAble : Entity
    {
        protected static Rect renderBox = new Rect(0, 0, 32, 32);
        int X, Y;
        public int Value { get; private set; }
        public PickupType Type { get; private set; }

        protected AnimatedSprite sprite;
        public PickupAble(int x, int y, int value, PickupType type) : base()
        {
            Type = type;
            Value = value;
            X = x / Map.tileSize;
            Y = y / Map.tileSize;
            position.X = x;
            position.Y = y;
        }

        public override void Update()
        {
            var players = level.GetPlayers();
            if (players == null) return;
            
            foreach (Player player in players)
            {
                int px = (int)player.GetX() / Map.tileSize;
                int py = (int)player.GetY() / Map.tileSize;

                if (px == X && py == Y)
                {
                    player.Consume(this);
                    Remove();
                    break;
                }
            }
        }
    }
}
