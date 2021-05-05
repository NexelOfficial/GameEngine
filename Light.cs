using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Light
    {
        public Vector2 position;
        public int range;
        public float brightness;
        public bool rendered = false;

        public Light(Vector2 position, int range, float brightness = 1)
        {
            this.position = position;
            this.range = range;
            this.brightness = brightness;
        }

        public void Render()
        {
            int multiplier = 255 / range;

            for (int x = (int)position.X - range; x < position.X + range; x++)
                for (int y = (int)position.Y - range; y < position.Y + range; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), position);
                    if (distance < range)
                    {
                        Tile tile = GameDemo.GetTile(x, y);
                        tile.lightLevel += (int)((range - distance) * multiplier * brightness);
                    }
                }
        }
    }
}
