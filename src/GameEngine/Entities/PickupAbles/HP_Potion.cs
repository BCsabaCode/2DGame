using GameEngine.Graphics;
using GameEngine.Utilities;

namespace GameEngine.Entities.PickupAbles
{
    class HP_Potion : PickupAble
    {
        public HP_Potion(int x, int y, int value) : base(x, y, value, PickupType.HP_Potion)
        {

        }

        public override void Render(Screen screen)
        {
            if (sprite == null) sprite = AnimatedSprite.GetAnimatedSprite("hpPotion");
            screen.RenderEntity(Coordinate.NormalToIso(Coordinate.VirtualZAxisReduction(position, -32)) / 2, renderBox, sprite.GetSprite());
        }
    }
}
