using GameEngine.Graphics;
using GameEngine.Utilities;

namespace GameEngine.Entities.PickupAbles
{
    class Arrows : PickupAble
    {
        public Arrows(int x, int y, int value) : base(x, y, value, PickupType.Arrows)
        {

        }

        public override void Render(Screen screen)
        {
            if (sprite == null) sprite = AnimatedSprite.GetAnimatedSprite("arrow");
            screen.RenderEntity(Coordinate.NormalToIso(Coordinate.VirtualZAxisReduction(position, -32)) / 2, renderBox, sprite.GetSprite());
        }
    }
}
