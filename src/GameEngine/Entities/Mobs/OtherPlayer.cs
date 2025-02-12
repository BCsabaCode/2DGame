using GameEngine.Inputs;

namespace GameEngine.Entities.Mobs
{
    class OtherPlayer : Player
    {
        ArtificialInput artificial;
        public OtherPlayer(float x, float y, ArtificialInput input) : base(x, y)
        {
            artificial = input;
        }

        public override void Update()
        {
            CheckHP();
            int xChange = 0;
            int yChange = 0;
            if (fireRate > 0) fireRate--;
            if (artificial.up)
            {
                yChange--;
                xChange--;
            }
            if (artificial.down)
            {
                yChange++;
                xChange++;
            }
            if (artificial.left)
            {
                xChange--;
                yChange++;
            }
            if (artificial.right)
            {
                xChange++;
                yChange--;
            }

            if (xChange == 0 && yChange == 0)
            {
                moving = false;
            }

            if (xChange != 0 || yChange != 0)
            {
                moving = true;
                Move(xChange, yChange);
            }
        }
    }
}
