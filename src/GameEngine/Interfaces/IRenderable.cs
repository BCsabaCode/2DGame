using GameEngine.Graphics;

namespace GameEngine.Interfaces
{
    interface IRenderable
    {
        /// <summary>
        /// With this function it can draw on the screen
        /// </summary>
        /// <param name="screen">Screen to render on</param>
        void Render(Screen screen);
    }
}
