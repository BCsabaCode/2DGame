using GameEngine.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Inputs
{
    class ArtificialInput
    {
        public bool up, down, left, right;

        public ArtificialInput()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }

        public void Update(Input input)
        {
            up = input.Up;
            down = input.Down;
            left = input.Left;
            right = input.Right;
        }
    }
}
