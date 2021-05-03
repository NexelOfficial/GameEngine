using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.UI
{
    public class Canvas
    {
        public Vector2 size;
        public Vector2 position;

        public Canvas(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }
    }
}
