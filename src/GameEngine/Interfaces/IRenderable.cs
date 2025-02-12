using GameEngine.Graphics;

namespace GameEngine.Interfaces
{
    interface IRenderable
    {
        /// <summary>
        /// Ezen methodussal tud rajzolni a canvas-re
        /// </summary>
        /// <param name="screen">Screen to render on</param>
        void Render(Screen screen);
    }
}
